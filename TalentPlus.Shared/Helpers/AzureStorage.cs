using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Globalization;

namespace TalentPlus.Shared.Helpers
{
	public class RemoteBlobAccess
	{

		public static class AzureStorageConstants
		{
			public static string Account = "talentplus";
			public static string SharedKeyAuthorizationScheme = "SharedKey";
			public static string BlobEndPoint = "https://talentplus.blob.core.windows.net/";
			public static string Key = "2ff8nU4pyccm5LlcfFi7xeVppgRuD/pusGTlhQh2oSe/+Jhgbg+ab9Kx79xjmv3zSEoHMYDwkH8h0D2zq3nZ5Q==";
			public static string ContainerName = "videos";
			public static string FileLocation = BlobEndPoint + ContainerName;
		}


		public static async Task<string> uploadToBlobStorage_async(Byte[] blobContent, string fileName)
		{

			string containerName = AzureStorageConstants.ContainerName;
			return await PutBlob_async(containerName, fileName, blobContent);
		}


		private static async Task<string> PutBlob_async(String containerName, String blobName, Byte[] blobContent)
		{
			String requestMethod = "PUT";

			//String content = "The Name of This Band is Talking Heads";
			//UTF8Encoding utf8Encoding = new UTF8Encoding();
			//Byte[] blobContent = utf8Encoding.GetBytes(content);
			Int32 blobLength = blobContent.Length;

			const String blobType = "BlockBlob";

			String urlPath = String.Format("{0}/{1}", containerName, blobName);
			String msVersion = "2009-09-19";
			String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

			String canonicalizedHeaders = String.Format("x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}", blobType, dateInRfc1123Format, msVersion);
			String canonicalizedResource = String.Format("/{0}/{1}", AzureStorageConstants.Account, urlPath);
			String stringToSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}", requestMethod, blobLength, canonicalizedHeaders, canonicalizedResource);
			Debug.WriteLine("StringToSign=" + stringToSign);
			String authorizationHeader = CreateAuthorizationHeader(stringToSign);
			Debug.WriteLine("Authorization Header=" + authorizationHeader);

			string uri = AzureStorageConstants.BlobEndPoint + urlPath;
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("x-ms-blob-type", blobType);
			client.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
			client.DefaultRequestHeaders.Add("x-ms-version", msVersion);
			Debug.WriteLine("Added all headers except authorisation");
			client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
			Debug.WriteLine("Added authorisation header");
			//logRequest(requestContent, uri);

			Debug.WriteLine("created new http client");
			HttpContent requestContent = new ByteArrayContent(blobContent);
			HttpResponseMessage response = await client.PutAsync(uri, requestContent);
			Debug.WriteLine("sent request");
			if (response.IsSuccessStatusCode == true)
			{
				return uri;
			}
			//SystemLogManager.AddErrorToLog(mLocalDataConnection, BL.SystemLogLocation.Library, "Failed to uploadBlob: Response.StatusCode=" + response.StatusCode + "/n Reason: " + response.ReasonPhrase, false);
			Debug.WriteLine("Response =/n" + response.ToString());
			return null;

		}

		private static String CreateAuthorizationHeader(String canonicalizedString)
		{
			if (String.IsNullOrEmpty(canonicalizedString))
			{
				throw new ArgumentNullException("canonicalizedString");
			}

			String signature = CreateHmacSignature(canonicalizedString, Convert.FromBase64String(AzureStorageConstants.Key));
			String authorizationHeader = String.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", AzureStorageConstants.SharedKeyAuthorizationScheme, AzureStorageConstants.Account, signature);

			return authorizationHeader;
		}

		private static String CreateHmacSignature(String unsignedString, Byte[] key)
		{
			if (String.IsNullOrEmpty(unsignedString))
			{
				throw new ArgumentNullException("unsignedString");
			}

			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(unsignedString);
			using (HMACSHA256 hmacSha256 = new HMACSHA256(key))
			{
				return Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
			}
		}
		private static async void logRequest(HttpContent content, string requestURI)
		{
			Debug.WriteLine("RequestURI: " + requestURI);
			foreach (var aHeader in content.Headers)
			{
				Debug.WriteLine("Header: " + aHeader.Key + " : " + aHeader.Value.ToString());
			}

			Task<string> getContent = content.ReadAsStringAsync();
			Debug.WriteLine("Content:" + await getContent);

		}

	}
}