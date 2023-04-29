using OAuth20.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.AspNetCore.Builder;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace OAuth20.Server.Models
{
	public class AzureManager
	{
		public static AzureManager Instance { get; } = new AzureManager();
		public static CloudTableClient tableClient;
		public static CloudStorageAccount storageAccount;
		public static string chargePointsUrl()
		{
			return StorageSASchargepointsUri.ToString().Split('?')[0];
		}

		public AzureManager()
		{
			StorageSASchargepointsUri = new Uri(StaticData.Configuration["StorageSASchargepoints"]);
			storageAccount = CloudStorageAccount.Parse(StaticData.Configuration["StorageConnectionString"]);
			tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
		}

		private static Uri StorageSASchargepointsUri;
		private static Uri SASdocumentsUri;

		private static string StorageAccountName = StaticData.Configuration["StorageAccountName"];
		private static string StorageAccountKey = StaticData.Configuration["StorageAccountKey"];
		private static string StorageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", StorageAccountName, StorageAccountKey);
		#region Blobs
		public async Task<MemoryStream> GetFile(string filename, string subfolder)
		{
			string path = getPath(filename, subfolder);

			Azure.Storage.Blobs.BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(StorageConnectionString, "documents", path);

			MemoryStream stream = new MemoryStream();
			var response = await blobClient.DownloadToAsync(stream);

			stream.Position = 0;
			return stream;
			//            return new MemoryStream(blob.DownloadToByteArray()));
		}
		public async Task<MemoryStream> GetFile(string filename, string subfolder, string mainfolder)
		{
			string path = getPath(filename, subfolder);

			Azure.Storage.Blobs.BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(StorageConnectionString, mainfolder, path);

			MemoryStream stream = new MemoryStream();
			var response = await blobClient.DownloadToAsync(stream);

			stream.Position = 0;
			return stream;
			//            return new MemoryStream(blob.DownloadToByteArray()));
		}
		public async Task<bool> Exists(string filename, string subfolder)
		{
			string path = getPath(filename, subfolder);
			try
			{
				Azure.Storage.Blobs.BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(StorageConnectionString, "documents", path);
				return await blobClient.ExistsAsync();

			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
			}
			return false;
		}
		public async Task<bool> Exists(string filename, string subfolder, string mainFolder)
		{
			string path = getPath(filename, subfolder);
			try
			{
				Azure.Storage.Blobs.BlobClient blobClient = new Azure.Storage.Blobs.BlobClient(StorageConnectionString, mainFolder, path);
				return await blobClient.ExistsAsync();

			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
			}
			return false;
		}
		//public static async Task<bool> Exists(string filename, string subfolder)
		//{
		//	string path = getPath(filename, subfolder);
		//	CloudBlobContainer objContainer;
		//	objContainer = new CloudBlobContainer(SASdocumentsUri);

		//	var blobReference = objContainer.GetBlobReference(getPath(path, null));
		//	return await blobReference.ExistsAsync();

		//}

		private static string getPath(string filename, string subfolder)
		{
			return "" + (string.IsNullOrWhiteSpace(subfolder) ? "" : (subfolder + "/")) + filename;
		}

		public static async Task<bool> SaveFile(byte[] bytes, string filename, string subfolder, bool versioning)
		{
			string path = getPath(filename, subfolder);

			//CloudBlobContainer objContainer;
			//objContainer = new CloudBlobContainer(SASUri);
			BlobContainerClient blobContainer = new BlobContainerClient(StorageSASchargepointsUri);

			//CloudBlockBlob objBlob = objContainer.GetBlockBlobReference(path);
			BlobClient blob = blobContainer.GetBlobClient(path);

			var blobHttpHeader = new BlobHttpHeaders();
			string[] paths = path.Split('.');
			switch (paths[paths.Length - 1])
			{
				case "png": blobHttpHeader.ContentType = "image/png"; break;
				case "jpg": blobHttpHeader.ContentType = "image/jpeg"; break;
				case "css": blobHttpHeader.ContentType = "text/css"; break;
				case "json": blobHttpHeader.ContentType = "application/json"; break;
				case "svg": blobHttpHeader.ContentType = "image/svg+xml"; break;
				case "pdf": blobHttpHeader.ContentType = "application/pdf"; break;
				case "docx": blobHttpHeader.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
				default: blobHttpHeader.ContentType = "application/octet-stream"; break;
			}
			blobHttpHeader.ContentType = "image/png";


			if (versioning)
			{
				if (blob.Exists())
				{
					blob.CreateSnapshot();
				}
			}
			var uploadedBlob = await blob.UploadAsync(new MemoryStream(bytes), blobHttpHeader);
			return true;
		}
		#endregion

		#region Table Storage
		public async Task AzureSaveCustomer(CustomerEntity customer)
		{
			CloudTable table = AzureManager.tableClient.GetTableReference("CafCustomer");
			await table.CreateIfNotExistsAsync();

			string filter = "(PartitionKey eq '" + customer.PartitionKey + "') ";
			filter += " and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customer.RowKey) + ")";
			TableQuery<CustomerEntityExt> query = new TableQuery<CustomerEntityExt>().Where(
						filter
					).Take(100);

			List<CustomerEntityExt> results = new List<CustomerEntityExt>();
			TableContinuationToken token = null;

			do
			{
				var seg = await table.ExecuteQuerySegmentedAsync(query, token);
				token = seg.ContinuationToken;
				results.AddRange(seg.Results);
			}
			while (token != null);
			CustomerEntityExt CustomerEntity = results.FirstOrDefault();
			if (CustomerEntity == null)
			{
				customer.CreateHash();
				TableOperation insertOperation = TableOperation.InsertOrReplace(customer);
				await table.ExecuteAsync(insertOperation);
			}
			else
			{

				if (string.IsNullOrEmpty(CustomerEntity.PasswordHash))
				{
					CustomerEntity.Salt = customer.Salt;
					CustomerEntity.PasswordHash = customer.PasswordHash;
					TableOperation insertOperation = TableOperation.Replace(CustomerEntity);
					await table.ExecuteAsync(insertOperation);
				}
			}

		}
		public async Task AzureUpdateCustomer(CustomerEntity customer)
		{
			CloudTable table = AzureManager.tableClient.GetTableReference("CafCustomer");
			await table.CreateIfNotExistsAsync();

			string filter = "(PartitionKey eq '" + customer.PartitionKey + "') ";
			filter += " and (" + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, customer.RowKey) + ")";
			TableQuery<CustomerEntityExt> query = new TableQuery<CustomerEntityExt>().Where(
						filter
					).Take(100);

			List<CustomerEntityExt> results = new List<CustomerEntityExt>();
			TableContinuationToken token = null;

			do
			{
				var seg = await table.ExecuteQuerySegmentedAsync(query, token);
				token = seg.ContinuationToken;
				results.AddRange(seg.Results);
			}
			while (token != null);
			CustomerEntityExt CustomerEntity = results.FirstOrDefault();
			if (CustomerEntity != null)
			{
				CustomerEntity.Salt = customer.Salt;
				CustomerEntity.PasswordHash = customer.PasswordHash;
				TableOperation insertOperation = TableOperation.Replace(CustomerEntity);
				await table.ExecuteAsync(insertOperation);
			}

		}

		#endregion
	}

}
