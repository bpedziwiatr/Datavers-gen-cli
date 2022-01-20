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

namespace DataverseGen.Core.DataConverter
{
    public class DataverseConnector
    {
        private readonly string _connectionString;
        private readonly string[] _selectedEntities;

        public DataverseConnector(string connectionString, string[] selectedEntities)
        {
            _connectionString = connectionString;
            _selectedEntities = selectedEntities;
        }

        public MappingEntity[] GetMappedEntities()
        {
            Stopwatch stopper = Stopwatch.StartNew();
            ColorConsole.WriteInfo(@"Waiting for connection...");
            CrmServiceClient connection = new CrmServiceClient(_connectionString);
            if (!connection.IsReady)
            {
                ColorConsole.WriteInfo(@"Waiting for connection...1000ms");
                Thread.Sleep(1000);
            }

            if (!string.IsNullOrWhiteSpace(connection.LastCrmError))
            {
                string exceptionMessage =
                    $"Connection did not connect with {_connectionString}. LastCrmError: {connection.LastCrmError}";
                ColorConsole.WriteError(exceptionMessage);
                throw new Exception(exceptionMessage);
            }

            EntityMetadata[] allEntities = RetrieveAllEntitiesMetaData(connection);
            ColorConsole.WriteInfo(@"Retrieving Selected Entities");
            IEnumerable<EntityMetadata> selectedEntities =
                SelectedEntitiesMetaData(allEntities, connection);

            List<MappingEntity> mappedEntities = selectedEntities
                                                 .Select(MappingEntity.Parse)
                                                 .OrderBy(e => e.DisplayName)
                                                 .ToList();
            ColorConsole.WriteInfo(@"All Selected Entities Retrieved");
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
            ColorConsole.WriteInfo($@"Read data from Dataverse in: {stopper.Elapsed:g}");
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

        private static EntityMetadata[] RetrieveAllEntitiesMetaData(CrmServiceClient connection)
        {
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Default,
                RetrieveAsIfPublished = true
            };
            ColorConsole.WriteInfo(@"Retrieving All Entities");
            RetrieveAllEntitiesResponse response =
                (RetrieveAllEntitiesResponse)connection.Execute(request);
            ColorConsole.WriteInfo($@"All Entities. Retrieved count: {response.EntityMetadata.Length}");
            return response.EntityMetadata;
        }

        private IEnumerable<EntityMetadata> GetEntitiesToRetrieve(EntityMetadata[] allEntities)
        {
            bool isActivityPartyIncluded = false;


            foreach (string selectedEntity in _selectedEntities)
            {
                EntityMetadata foundEntity =
                    allEntities.SingleOrDefault(p => p.LogicalName == selectedEntity);
                if (foundEntity == null)
                {
                    ColorConsole.WriteError($@"!@!@!@ entity not found{selectedEntity}");
                    yield break;
                }

                if (!isActivityPartyIncluded &&
                    (foundEntity.IsActivity == true || foundEntity.IsActivityParty == true))
                {
                    isActivityPartyIncluded = true;
                    yield return allEntities.Single(r => r.LogicalName.Equals("activityparty"));
                }

                ColorConsole.WriteSuccess(
                    $@"found entity: {selectedEntity},metadata-id:{foundEntity.MetadataId}");
                yield return foundEntity;
            }
        }

        private IEnumerable<EntityMetadata> SelectedEntitiesMetaData(
            EntityMetadata[] allEntities,
            IOrganizationService service)
        {
            IEnumerable<EntityMetadata> selectedEntities = GetEntitiesToRetrieve(allEntities);
            foreach (EntityMetadata entity in selectedEntities)
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = entity.LogicalName,
                    RetrieveAsIfPublished = true
                };
                RetrieveEntityResponse res = (RetrieveEntityResponse)service.Execute(req);
                yield return res.EntityMetadata;
            }
        }
    }
}