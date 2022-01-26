using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Core.DataConverter
{
    public class DataverseMetadataConverter
    {
        private const string ActivityPartySchemaName = "activityparty";
        private readonly DataverseConnector _dataverseConnector;
        private readonly string[] _selectedEntities;
        private readonly bool _throwOnEntityNotFound;

        public DataverseMetadataConverter(
            DataverseConnector dataverseConnector,
            bool throwOnEntityNotFound,
            string[] selectedEntities
        )
        {
            _selectedEntities = selectedEntities;

            _throwOnEntityNotFound = throwOnEntityNotFound;
            _dataverseConnector = dataverseConnector;
            WriteConverterSetting();
        }

        public MappingEntity[] GetMappedEntities()
        {
            Stopwatch stopper = Stopwatch.StartNew();
            _dataverseConnector.Connect();

            IEnumerable<EntityMetadata> selectedEntities =
                SelectedEntitiesMetaData();

            List<MappingEntity> mappedEntities = selectedEntities
                                                 .Select(MappingEntity.Parse)
                                                 .OrderBy(e => e.DisplayName)
                                                 .ToList();
            WriteInfo(@"Selected Entities Retrieved");
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
                    (RetrieveEntityResponse)_dataverseConnector.OrganizationService.Execute(req);
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
        )
        {
            bool isActivityPartyIncluded = false;
            HashSet<string> retrievedEntitiesSchemaNames =
                new HashSet<string>(_selectedEntities.Length + 10);

            foreach (string selectedEntity in _selectedEntities)
            {
                if (retrievedEntitiesSchemaNames.Contains(selectedEntity))
                {
                    ColorConsole.WriteWarning($"duplicate entity {selectedEntity} ignoring");
                    continue;
                }
                EntityMetadata entityEntityMetadata =
                    RetrieveEntityMetadata(selectedEntity);
                if (entityEntityMetadata == null)
                {
                    continue;
                }

                retrievedEntitiesSchemaNames.Add(entityEntityMetadata.LogicalName);
                yield return entityEntityMetadata;
                if (isActivityPartyIncluded
                    || entityEntityMetadata.IsActivity != true
                    && entityEntityMetadata.IsActivityParty != true)
                {
                    continue;
                }

                isActivityPartyIncluded = true;
                if (retrievedEntitiesSchemaNames.Contains(ActivityPartySchemaName))
                {
                    continue;
                }

                EntityMetadata activityPartyMetadata =
                    RetrieveEntityMetadata(ActivityPartySchemaName);
                if (activityPartyMetadata == null)
                {
                    continue;
                }

                retrievedEntitiesSchemaNames.Add(ActivityPartySchemaName);
                yield return activityPartyMetadata;
            }
        }

        private void WriteConverterSetting()
        {
            WriteInfo($@"Selected entities: {string.Join(", ", _selectedEntities)}");
            WriteInfo($"Throw error if entity not found={_throwOnEntityNotFound}");
        }
    }
}