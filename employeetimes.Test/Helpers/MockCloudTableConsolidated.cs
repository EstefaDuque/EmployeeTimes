using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace employeetimes.Test.Helpers
{
    public class MockCloudTableConsolidated: CloudTable
    {
        public MockCloudTableConsolidated(Uri tableAddress) : base(tableAddress)
        {

        }

        public MockCloudTableConsolidated(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableConsolidated(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            //Para poder mockear todos los resultados de la tabla
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetConsolidatedEntity()
            });
        }
    }
}
