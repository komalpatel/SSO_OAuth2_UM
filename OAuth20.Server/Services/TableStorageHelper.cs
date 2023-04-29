using OAuth20.Server.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

namespace OAuth20.Server.Services
{
    public class TableStorageHelper
    {

        public static TableStorageHelper Instance { get { return _instance; } }
        private static TableStorageHelper _instance = new TableStorageHelper();

        private CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StaticData.Configuration["StorageConnectionString"]);
        public TableStorageHelper()
        {

        }

        public async Task<Guid> CreateCustomerEntity(string Email, string Password, string Name, string Partner = null)
        {
            try
            {
                string email = Email.ToLower();
                //TBD: Security, Replication Protection etc.
                CustomerEntity customerEntity = await GetCustomerEntityByEmail(email);
                if (customerEntity != null)
                {
                    customerEntity.Email = email;
                    customerEntity.Name = Name;
                    customerEntity.Password = Password;
                    customerEntity.Partner = Partner;
                    customerEntity.CreateHash();
                }
                else
                {
                    customerEntity = new CustomerEntity();
                    customerEntity.Email = email;
                    customerEntity.Name = Name;
                    customerEntity.Password = Password;
                    customerEntity.Guid = Guid.NewGuid();
                    customerEntity.Partner = Partner;
                    customerEntity.CreateHash();
                }
                await AzureManager.Instance.AzureSaveCustomer(customerEntity);
                return customerEntity.Guid;
            }
            catch (Exception err)
            {
                return Guid.Empty;
            }
        }

        public async Task<Guid> UpdateCustomerEntity(string Email, string Password, string Name)
        {
            try
            {
                string email = Email.ToLower();
                //TBD: Security, Replication Protection etc.
                CustomerEntity customerEntity = await GetCustomerEntityByEmail(email);
                if (customerEntity != null)
                {
                    customerEntity.Email = email;
                    customerEntity.Name = Name;
                    customerEntity.Password = Password;
                    customerEntity.CreateHash();
                    await AzureManager.Instance.AzureUpdateCustomer(customerEntity);
                }
                return customerEntity.Guid;
            }
            catch (Exception err)
            {
                return Guid.Empty;
            }
        }



        public async Task<Guid> GetCafCustomerByEmail(string email)
        {
            CustomerEntity customerEntity = await GetCustomerEntityByEmail(email);
            return customerEntity != null ? customerEntity.Guid : Guid.Empty;
        }

        public async Task<CustomerEntity> GetCustomerEntityByEmail(string email)
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("CafCustomer");
            string[] emailFields = email.ToLower().Split('@');
            if (emailFields.Length != 2) { return null; }
            string filter = "(" + TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, emailFields[1]) + ")";
            filter += " and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, emailFields[0]) + ")";
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(
                        filter
                    ).Take(1);

            List<CustomerEntity> results = new List<CustomerEntity>();
            TableContinuationToken token = null;

            var seg = await table.ExecuteQuerySegmentedAsync(query, token);
            results.AddRange(seg.Results);
            if (results.Count == 1)
            {
                if (StaticData.UserAccountGuids.ContainsKey(email))
                {
                    StaticData.UserAccountGuids[email] = results[0].Guid;
                }
                else
                {
                    StaticData.UserAccountGuids.Add(email, results[0].Guid);
                }
                return results[0];
            }
            return null;
        }

        public async Task<CustomerEntity> GetCustomer(string UserGuid)
        {
            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("CafCustomer");

            string filter = "(" + TableQuery.GenerateFilterConditionForGuid("Guid", QueryComparisons.Equal, new Guid(UserGuid)) + ")";
            TableQuery<CustomerEntity> customerquery = new TableQuery<CustomerEntity>().Where(filter).Take(1);

            List<CustomerEntity> customerResults = new List<CustomerEntity>();
            TableContinuationToken token = null;

            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync(customerquery, token);
                token = seg.ContinuationToken;
                customerResults.AddRange(seg.Results);
            }
            while (token != null);
            CustomerEntity CustomerEntity = customerResults.FirstOrDefault();
            return CustomerEntity;
        }


    }

}
