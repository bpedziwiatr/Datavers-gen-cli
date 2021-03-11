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

        public DataverseConnector(string connectionString, string selectedEntities)
        {
            _connectionString = connectionString;
            _selectedEntities = selectedEntities.Split(';');
        }

        public MappingEntity[] GetMappedEntities()
        {
            Stopwatch stopper= Stopwatch.StartNew();
            CrmServiceClient connection = new CrmServiceClient(_connectionString);
            if (string.IsNullOrWhiteSpace(connection.LastCrmError))
            {
                throw new Exception($"Connectiuon did not connect with {_connectionString}");
            }
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Default,
                RetrieveAsIfPublished = true,
            };
            Console.WriteLine("Retriving All Entities");
            RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)connection.Execute(request);
            EntityMetadata[] allEntities = response.EntityMetadata;
            Console.WriteLine("All Entities  Retrived");
            var entities = allEntities;
            Console.WriteLine("Retriving Selected Entities");
            var selectedEntities = SelectedEntitiesMEtaData(allEntities, connection).ToList();
            Console.WriteLine("All Selected Entities Retrived");
            var mappedEntities = selectedEntities.Select(e => MappingEntity.Parse(e)).OrderBy(e => e.DisplayName).ToList();
            ExcludeRelationshipsNotIncluded(mappedEntities);
            foreach (var ent in mappedEntities)
            {
                foreach (var rel in ent.RelationshipsOneToMany)
                {
                    rel.ToEntity = mappedEntities.Where(e => e.LogicalName.Equals(rel.Attribute.ToEntity)).FirstOrDefault();
                }
                foreach (var rel in ent.RelationshipsManyToOne)
                {
                    rel.ToEntity = mappedEntities.Where(e => e.LogicalName.Equals(rel.Attribute.ToEntity)).FirstOrDefault();
                }
                foreach (var rel in ent.RelationshipsManyToMany)
                {
                    rel.ToEntity = mappedEntities.Where(e => e.LogicalName.Equals(rel.Attribute.ToEntity)).FirstOrDefault();
                }
            }

            var result = mappedEntities.ToArray();
            stopper.Stop();
            Console.WriteLine($"Read data from crm took: {stopper.Elapsed:g}");
            return result;
        }

        private static void ExcludeRelationshipsNotIncluded(List<MappingEntity> mappedEntities)
        {
            foreach (var ent in mappedEntities)
            {
                ent.RelationshipsOneToMany = ent.RelationshipsOneToMany.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
                ent.RelationshipsManyToOne = ent.RelationshipsManyToOne.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
                ent.RelationshipsManyToMany = ent.RelationshipsManyToMany.ToList().Where(r => mappedEntities.Select(m => m.LogicalName).Contains(r.Type)).ToArray();
            }
        }

        private  IEnumerable<EntityMetadata> SelectedEntitiesMEtaData(EntityMetadata[] allEntities, IOrganizationService service)
        {
            var selected = allEntities.Where(p => _selectedEntities.Any(pp=>pp == p.LogicalName)).ToList();
            if (selected.Any(r => r.IsActivity == true || r.IsActivityParty == true))
            {
                if (!selected.Any(r => r.LogicalName.Equals("activityparty")))
                    selected.Add(allEntities.Single(r => r.LogicalName.Equals("activityparty")));
            }
            var results = new List<EntityMetadata>();
            foreach (var entity in selected)
            {
                var req = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = entity.LogicalName,
                    RetrieveAsIfPublished = true
                };
                var res = (RetrieveEntityResponse)service.Execute(req);
                yield return res.EntityMetadata;
            }
        }
    }
}