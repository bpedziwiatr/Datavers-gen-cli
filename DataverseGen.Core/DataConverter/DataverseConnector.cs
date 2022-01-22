using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Core.DataConverter
{
    public class DataverseConnector
    {
        private readonly string _connectionString;
        private readonly string[] _selectedEntities;
        private readonly bool _throwOnEntityNotFound;

        public DataverseConnector(
            string connectionString,
            string[] selectedEntities,
            bool throwOnEntityNotFound)
        {
            _connectionString = connectionString;
            _selectedEntities = selectedEntities;
            _throwOnEntityNotFound = throwOnEntityNotFound;
            WriteConnectorInfo();
        }

        public MappingEntity[] GetMappedEntities()
        {
            Stopwatch stopper = Stopwatch.StartNew();
            WriteInfo(@"Waiting for connection...");
            CrmServiceClient connection = new CrmServiceClient(_connectionString);
            if (!connection.IsReady)
            {
                WriteInfo(@"Waiting for connection... 1000ms");
                Thread.Sleep(1000);
            }

            if (!string.IsNullOrWhiteSpace(connection.LastCrmError))
            {
                string exceptionMessage =
                    $"Connection did not connect with {_connectionString}. LastCrmError: {connection.LastCrmError}";
                WriteError(exceptionMessage);
                throw new Exception(exceptionMessage);
            }

            IEnumerable<EntityMetadata> selectedEntities =
                SelectedEntitiesMetaData(connection);

            List<MappingEntity> mappedEntities = selectedEntities
                                                 .Select(MappingEntity.Parse)
                                                 .OrderBy(e => e.DisplayName)
                                                 .ToList();
            WriteInfo(@"All Selected Entities Retrieved");
            ExcludeRelationshipsNotIncluded(mappedEntities);
            foreach (MappingEntity ent in mappedEntities)
            {
                foreach (MappingRelationship1N rel in ent.RelationshipsOneToMany)
                {
                    rel.ToEntity =
                        mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }

                foreach (MappingRelationshipN1 rel in ent.RelationshipsManyToOne)
                {
                    rel.ToEntity =
                        mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }

                foreach (MappingRelationshipMn rel in ent.RelationshipsManyToMany)
                {
                    rel.ToEntity =
                        mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }
            }

            MappingEntity[] result = mappedEntities.ToArray();
            stopper.Stop();
            WriteInfo($@"Read data from Dataverse in: {stopper.Elapsed:g}");
            return result;
        }

        private static void ExcludeRelationshipsNotIncluded(
            IReadOnlyCollection<MappingEntity> mappedEntities)
        {
            foreach (MappingEntity ent in mappedEntities)
            {
                ent.RelationshipsOneToMany = ent.RelationshipsOneToMany
                                                .Where(r =>
                                                    mappedEntities.Select(m => m.LogicalName)
                                                                  .Contains(r.Type))
                                                .ToArray();
                ent.RelationshipsManyToOne = ent.RelationshipsManyToOne
                                                .Where(r =>
                                                    mappedEntities.Select(m => m.LogicalName)
                                                                  .Contains(r.Type))
                                                .ToArray();
                ent.RelationshipsManyToMany = ent.RelationshipsManyToMany
                                                 .Where(r =>
                                                     mappedEntities.Select(m => m.LogicalName)
                                                                   .Contains(r.Type))
                                                 .ToArray();
            }
        }
        private EntityMetadata RetrieveEntityMetadata(
            IOrganizationService organizationService,
            string selectedEntity)
        {
            try
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = selectedEntity,
                    RetrieveAsIfPublished = true
                };
                RetrieveEntityResponse response =
                    (RetrieveEntityResponse)organizationService.Execute(req);
                WriteSuccess(
                    $@"Found entity: {selectedEntity}, metadata-id: {response.EntityMetadata.MetadataId}");
                return response.EntityMetadata;
            }
            catch (Exception ex)
            {
                WriteError(
                    $@"!@!@!@ entity not found: {selectedEntity} | {ex.Message}");
                if (_throwOnEntityNotFound)
                {
                    throw;
                }

                return null;
            }
        }

        private IEnumerable<EntityMetadata> SelectedEntitiesMetaData(
            IOrganizationService organizationService)
        {
            bool isActivityPartyIncluded = false;


            foreach (string selectedEntity in _selectedEntities)
            {
                EntityMetadata entityEntityMetadata =
                    RetrieveEntityMetadata(organizationService, selectedEntity);
                if (entityEntityMetadata == null)
                {
                    continue;
                }

                yield return entityEntityMetadata;
                if (isActivityPartyIncluded
                    || entityEntityMetadata.IsActivity != true
                    && entityEntityMetadata.IsActivityParty != true)
                {
                    continue;
                }

                isActivityPartyIncluded = true;
                EntityMetadata activityPartyMetadata =
                    RetrieveEntityMetadata(organizationService, "activityparty");
                if (activityPartyMetadata != null)
                {
                    yield return activityPartyMetadata;
                }
            }
        }

        private void WriteConnectorInfo()
        {
            WriteInfo($@"Selected entities: {string.Join(",", _selectedEntities)}");
            WriteInfo($"Throw error if entity not found={_throwOnEntityNotFound}");
        }
    }
}