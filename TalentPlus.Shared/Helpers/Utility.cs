using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TalentPlus.Shared.Helpers
{
	public static class Utility
	{
		public static void SetScreenTitle(string title)
		{
			MessagingCenter.Send<string> (title, "UpdatePageTitle");
		}

        public static void RefreshTabBar()
        {
            MessagingCenter.Send<string>(string.Empty, "ReloadTabBar");
        }

		public static void RefreshActionBar()
		{
			MessagingCenter.Send<string>(string.Empty, "ReloadActionBar"); //TODO: i think this should not be called this often
		}

		public static void UpdateColorWheel()
		{
			MessagingCenter.Send<string>(string.Empty, "UpdateColorWheel");
		}


		public static void ForceHideBackButton()
		{
			MessagingCenter.Send<string>(string.Empty, "ForceHideBackButton");
		}

		public static async Task<string> UploadPhoto(byte[] photoBytes)
		{
			return await UploadPhotoWithSize(photoBytes, 600, 600);			
		}

		public static async Task<string> UploadPhotoWithSize(byte[] photoBytes, float width, float height)
		{
			photoBytes = ImageResizer.ResizeImage(photoBytes, width, height);
			var content = new MultipartFormDataContent();
			var fileContent = new ByteArrayContent(photoBytes);
			fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = Guid.NewGuid() + ".jpg"
			};
			content.Add(fileContent);

			HttpResponseMessage message = await TalentDb.client.InvokeApiAsync("photo", content, HttpMethod.Put, null, null);
			string url = await message.Content.ReadAsStringAsync();
			return url.Trim('"');
		}

		public static async Task<long> UploadVideo(byte[] videoBytes)
		{
			var azureUrl = await RemoteBlobAccess.uploadToBlobStorage_async(videoBytes, Guid.NewGuid() + ".mp4");
			if (!string.IsNullOrWhiteSpace(azureUrl))
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters.Add("azureUrl", azureUrl);
				return await TalentDb.client.InvokeApiAsync<long>("video", HttpMethod.Post, parameters);
			}
			return 0;
		}

		public static string TimeSpanToText(TimeSpan requiredTime)
		{
			if (requiredTime.TotalMinutes < 60)
			{
				return requiredTime.TotalMinutes + " minutes";
			}
			else if (requiredTime.TotalHours < 24)
			{
				return requiredTime.TotalHours + " hours";
			}
			else
			{
				return requiredTime.TotalDays + " days";
			}
		}

		//public static async Task<long> UploadVideo(byte[] photoBytes)
		//{
		//	var content = new MultipartFormDataContent();
		//	var fileContent = new ByteArrayContent(photoBytes);
		//	fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
		//	{
		//		FileName = Guid.NewGuid() + ".mp4"
		//	};
		//	content.Add(fileContent);

		//	using (var client = new HttpClient())
		//	{
		//		HttpResponseMessage message = await TalentDb.client.InvokeApiAsync("video", content, HttpMethod.Put, null, null);
		//		string response = await message.Content.ReadAsStringAsync();
		//		return long.Parse(response.Trim('"'));
		//	}
		//}

	}
}
