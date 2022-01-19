using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            CrmServiceClient connection = new CrmServiceClient(_connectionString);
            if (!connection.IsReady)
            {
                Console.WriteLine(@"Waiting for connection...");
                System.Threading.Thread.Sleep(1000);
            }

            if (!string.IsNullOrWhiteSpace(connection.LastCrmError))
            {
                throw new Exception($"Connection did not connect with {_connectionString}. LastCrmError: {connection.LastCrmError}");
            }
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Default,
                RetrieveAsIfPublished = true,
            };
            Console.WriteLine(@"Retrieving All Entities");
            RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)connection.Execute(request);
            EntityMetadata[] allEntities = response.EntityMetadata;
            Console.WriteLine(@"All Entities  Retrieved");
            Console.WriteLine(@"Retrieving Selected Entities");
            List<EntityMetadata> selectedEntities = SelectedEntitiesMetaData(allEntities, connection).ToList();
            Console.WriteLine(@"All Selected Entities Retrieved");
            List<MappingEntity> mappedEntities = selectedEntities.Select(MappingEntity.Parse).OrderBy(e => e.DisplayName).ToList();
            ExcludeRelationshipsNotIncluded(mappedEntities);
            foreach (MappingEntity ent in mappedEntities)
            {
                foreach (MappingRelationship1N rel in ent.RelationshipsOneToMany)
                {
                    rel.ToEntity = mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }
                foreach (MappingRelationshipN1 rel in ent.RelationshipsManyToOne)
                {
                    rel.ToEntity = mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }
                foreach (MappingRelationshipMn rel in ent.RelationshipsManyToMany)
                {
                    rel.ToEntity = mappedEntities.Find(e => e.LogicalName.Equals(rel.Attribute.ToEntity));
                }
            }

            MappingEntity[] result = mappedEntities.ToArray();
            stopper.Stop();
            Console.WriteLine($@"Read data from cds in: {stopper.Elapsed:g}");
            return result;
        }

        private static void ExcludeRelationshipsNotIncluded(IReadOnlyCollection<MappingEntity> mappedEntities)
        {
            foreach (MappingEntity ent in mappedEntities)
            {
                ent.RelationshipsOneToMany = ent.RelationshipsOneToMany.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
                ent.RelationshipsManyToOne = ent.RelationshipsManyToOne.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
                ent.RelationshipsManyToMany = ent.RelationshipsManyToMany.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
            }
        }

        private IEnumerable<EntityMetadata> SelectedEntitiesMetaData(EntityMetadata[] allEntities, IOrganizationService service)
        {
            IEnumerable<EntityMetadata> selected = GetEntitiesToRetrieve(allEntities);
            foreach (EntityMetadata entity in selected)
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

        private IEnumerable<EntityMetadata> GetEntitiesToRetrieve(EntityMetadata[] allEntities)
        {
            bool isActivityPartyIncluded = false;


            foreach (string selectedEntity in _selectedEntities)
            {
                EntityMetadata foundEntity =
                    allEntities.SingleOrDefault(p => p.LogicalName == selectedEntity);
                if (foundEntity == null)
                {
                    Console.WriteLine($@"!@!@!@ entity not found{selectedEntity}");
                    yield break;
                    
                }

                if (!isActivityPartyIncluded &&
                    (foundEntity.IsActivity == true  || foundEntity.IsActivityParty == true))
                {
                    
                    isActivityPartyIncluded = true;
                    yield return allEntities.Single(r => r.LogicalName.Equals("activityparty"));

                }

                Console.WriteLine($@"found entity: {selectedEntity},metadata-id:{foundEntity.MetadataId}");
                yield return foundEntity;
            }
        }
    }
}