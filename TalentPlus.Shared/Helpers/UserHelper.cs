using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace TalentPlus.Shared.Helpers
{
    public static class UserHelper
    {
		public static async Task<AddUserToTeamResult> AddTeamUserByEmail(string email)
		{
			Dictionary<string, string> parameters = new Dictionary<string,string>();
			parameters.Add("email", email);

            try
            {
                AddUserToTeamResult result = await TalentDb.client.InvokeApiAsync<AddUserToTeamResult>("teamMembersByEmail", System.Net.Http.HttpMethod.Put, parameters);
                if (result == AddUserToTeamResult.Added)
                {
                    await SyncUsers();
                }
                return result;
            }
            catch (Exception ex)
            {

            }

            return AddUserToTeamResult.EmailSendingError;
		}

		public static async Task<List<UserSuggestion>> SearchUsers(string search)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("search", search);

			try
			{
				List<UserSuggestion> result = await TalentDb.client.InvokeApiAsync<List<UserSuggestion>>("userSearch", System.Net.Http.HttpMethod.Get, parameters);
				return result;
			}
			catch (Exception ex)
			{

			}

			return null;
		}

		private static async Task SyncUsers()
		{
			await TalentDb.PushOrDiscardAsync();
			await TalentDb.client.GetSyncTable<TeamMember>().PullAsync("getAllTeamMember", TalentDb.client.GetSyncTable<TeamMember>().CreateQuery());
			await TalentDb.client.GetSyncTable<User>().PullAsync(null, TalentDb.client.GetSyncTable<User>().CreateQuery());
		}

		public static async Task<MobileServiceUser> CustomLoginAsync(string email, string password)
		{
			var param = Newtonsoft.Json.JsonConvert.SerializeObject(new { email = email, password = password });
			HttpContent content = new StringContent(param, Encoding.UTF8, "application/json");

			try
			{
				HttpResponseMessage message = await TalentDb.client.InvokeApiAsync("customLogin", content, HttpMethod.Post, null, null);
				string response = await message.Content.ReadAsStringAsync();

				if (!string.IsNullOrEmpty(response))
				{
					JToken authToken = JToken.Parse(response);
					// Get the Mobile Services auth token and user data

					TalentDb.client.CurrentUser = new MobileServiceUser((string)authToken["user"]["userId"]);
					TalentDb.client.CurrentUser.MobileServiceAuthenticationToken = (string)authToken["authenticationToken"];
				}

				return TalentDb.client.CurrentUser;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static async Task<MobileServiceUser> LoginView()
		{
			if (TalentPlusApp.TalentApp.LoadingPageView != null)
			{
				TalentPlusApp.TalentApp.LoadingPageView.StopAnimateLoading();
			}
			var continueClicked = new TaskCompletionSource<object>();
			var loginPage = new LoginPage(continueClicked);

			TalentPlusApp.TalentApp.SetLoginPage(loginPage);

			MobileServiceUser user = null;

			user = await TryLogin(continueClicked, loginPage);

			TalentPlusApp.TalentApp.SetLoadingPage();

			return user;
		}

		private static async Task<MobileServiceUser> TryLogin(TaskCompletionSource<object> continueClickedTask, LoginPage loginPage)
		{
			MobileServiceUser user = null;

			await continueClickedTask.Task;
			loginPage.AnimateLoading();
			user = await CustomLoginAsync(loginPage.Username, loginPage.Password);
			loginPage.StopAnimateLoading();

			if (user == null)
			{
				var continueClicked = new TaskCompletionSource<object>();
				loginPage.ButtonClicked = false;
				loginPage.ChangeCompletionTask(continueClicked);
				user = await TryLogin(continueClicked, loginPage);
			}

			return user;
		}
    }
}
