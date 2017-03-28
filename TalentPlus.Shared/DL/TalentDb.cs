using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Auth;
using Xamarin;


#if __ANDROID__
using Gcm.Client;
#endif

namespace TalentPlus.Shared
{
	public static class TalentDb
	{
		const int PAGE_SIZE = 5;
		public static MobileServiceClient client;
		public static bool isNeedActivityAgain;

		public static async Task<bool> InitializeAsync(bool isRequireLogin = true)
		{
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			client = new MobileServiceClient(
				@"https://talentplus.azure-mobile.net/",
				//@"http://192.168.18.47:63196/",
				@"SHjxAphWNJNDAkOBYmgWghzGTgCkfb32",
				new ReAuthHandler()
			);

			
			try
			{
				if (!client.SyncContext.IsInitialized)
				{
					var store = new MobileServiceSQLiteStore("TalentPlus.db");
					store.DefineTable<Activity>();
					store.DefineTable<ActivityTheme>();
					store.DefineTable<ActivityArchive>();
					store.DefineTable<ActivityDeclinedOption>();
					store.DefineTable<ActivityStatistics>();
					store.DefineTable<ActivityInvitation>();
					store.DefineTable<FeedbackInvitation>();
					store.DefineTable<FeedbackPost>();
					store.DefineTable<NegativeFeedbackPost>();
					store.DefineTable<PendingFeedback>();
					store.DefineTable<SelectedActivity>();
					store.DefineTable<TeamMember>();
					store.DefineTable<User>();
					store.DefineTable<UserSettings>();
					store.DefineTable<UserActivityOrder>();
					await client.SyncContext.InitializeAsync(store);
				}

                if (isRequireLogin == true)
                    await SetUserOrLogin();
                else
                    return true;

				if (client.CurrentUser == null)
				{
					return false;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(@"Sync Failed: {0}", e.ToString());
				Insights.Report(e, new Dictionary<string, string>
				{
					{ "Where", "TalentDb.InitializeAsync()" }
				});
				return false;
			}

			return true;
		}

		public static async Task SyncDatabase() {
			//do a retry of nameresolve error (weird bug, happens to people, no one knows how to solve it)
			var policy = Policy
				.Handle<System.Net.WebException>()
				.WaitAndRetryAsync(new[]
				  {
					TimeSpan.FromMilliseconds(100),
					TimeSpan.FromMilliseconds(200)
				  });
			try {
				await policy.ExecuteAsync( async () => await SyncTables() );
			}
			catch (Exception ex) {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "TalentDb.SyncDatabase()" }
				});
			}
		}

		private static async Task SyncTables()
        {
			await PushOrDiscardAsync();
			await client.GetSyncTable<Activity>().PullAsync("getAllActivity", client.GetSyncTable<Activity>().CreateQuery());
			await client.GetSyncTable<ActivityTheme>().PullAsync("getAllActivityTheme", client.GetSyncTable<ActivityTheme>().CreateQuery());
			await client.GetSyncTable<ActivityArchive>().PullAsync("getAllActivityArchive", client.GetSyncTable<ActivityArchive>().CreateQuery());
			await client.GetSyncTable<ActivityDeclinedOption>().PullAsync("getAllActivityDeclinedOption", client.GetSyncTable<ActivityDeclinedOption>().CreateQuery());
			await client.GetSyncTable<ActivityStatistics>().PullAsync("getAllActivityStatistics", client.GetSyncTable<ActivityStatistics>().CreateQuery());
			await client.GetSyncTable<NegativeFeedbackPost>().PullAsync("getAllNegativeFeedbackPost", client.GetSyncTable<NegativeFeedbackPost>().CreateQuery());
			await client.GetSyncTable<FeedbackPost>().PullAsync("getAllFeedbackPost", client.GetSyncTable<FeedbackPost>().CreateQuery());
			await client.GetSyncTable<PendingFeedback>().PullAsync("getAllPendingFeedback", client.GetSyncTable<PendingFeedback>().CreateQuery());
			await client.GetSyncTable<SelectedActivity>().PullAsync("getAllSelectedActivity", client.GetSyncTable<SelectedActivity>().CreateQuery());
			await client.GetSyncTable<ActivityInvitation>().PullAsync("getAllActivityInvitation", client.GetSyncTable<ActivityInvitation>().CreateQuery());
			await client.GetSyncTable<FeedbackInvitation>().PullAsync("getAllFeedbackInvitation", client.GetSyncTable<FeedbackInvitation>().CreateQuery());
			await client.GetSyncTable<TeamMember>().PullAsync("getAllTeamMember", client.GetSyncTable<TeamMember>().CreateQuery());
			await client.GetSyncTable<User>().PullAsync("getAllUser", client.GetSyncTable<User>().CreateQuery());
			await client.GetSyncTable<UserSettings>().PullAsync("getAllUserSettings", client.GetSyncTable<UserSettings>().CreateQuery());
			await client.GetSyncTable<UserActivityOrder>().PullAsync("getAllUserActivityOrder", client.GetSyncTable<UserActivityOrder>().CreateQuery());
        }

		public static async Task SyncFeedback()
		{
			var policy = Policy
			.Handle<System.Net.WebException>()
			.WaitAndRetryAsync(new[]
				{
				TimeSpan.FromMilliseconds(100),
				TimeSpan.FromMilliseconds(200)
				});
			try
			{
				await policy.ExecuteAsync(async () => {
					await PushOrDiscardAsync();
					await client.GetSyncTable<FeedbackPost>().PullAsync("getAllFeedbackPost", client.GetSyncTable<FeedbackPost>().CreateQuery());
					await client.GetSyncTable<ActivityStatistics>().PullAsync("getAllActivityStatistics", client.GetSyncTable<ActivityStatistics>().CreateQuery());
				});
			}
			catch (Exception ex)
			{
				Insights.Report(ex, new Dictionary<string, string>
			{
				{ "Where", "TalentDb.SyncInvitations()" }
			});
			}
		}

		public static async Task SyncInvitations()
		{
			var policy = Policy
			.Handle<System.Net.WebException>()
			.WaitAndRetryAsync(new[]
				{
				TimeSpan.FromMilliseconds(100),
				TimeSpan.FromMilliseconds(200)
				});
			try
			{
				await policy.ExecuteAsync(async () => await SyncInvitationsTables());
			}
			catch (Exception ex)
			{
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "TalentDb.SyncInvitations()" }
				});
			}
		}

		private static async Task SyncInvitationsTables()
		{
			await PushOrDiscardAsync();
			await client.GetSyncTable<ActivityInvitation>().PullAsync("getAllActivityInvitation", client.GetSyncTable<ActivityInvitation>().CreateQuery());
			await client.GetSyncTable<FeedbackInvitation>().PullAsync("getAllFeedbackInvitation", client.GetSyncTable<FeedbackInvitation>().CreateQuery());
			await client.GetSyncTable<TeamMember>().PullAsync("getAllTeamMember", client.GetSyncTable<TeamMember>().CreateQuery());
			await client.GetSyncTable<User>().PullAsync("getAllUser", client.GetSyncTable<User>().CreateQuery());
		}

		public static async Task PurgeData()
		{
			if (TalentPlusApp.CurrentUser.NeedPurging)
			{
				TalentPlusApp.CurrentUser.NeedPurging = false;
				await SaveOrUpdateItem<User>(TalentPlusApp.CurrentUser);
			}

		#if __IOS__
			var accountStore = AccountStore.Create(); // Xamarin.IOs
			TalentPlusApp.UIContext = UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController;
		#elif __ANDROID__
			var accountStore = AccountStore.Create(TalentPlusApp.UIContext); // Xamarin.Android
		#endif

			var accounts = accountStore.FindAccountsForService("MsAccount").ToList();
			foreach (var account in accounts)
			{
				accountStore.Delete(account, "MsAccount");
			}

			await client.GetSyncTable<Activity>().PurgeAsync("getAllActivity", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<ActivityTheme>().PurgeAsync("getAllActivityTheme", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<ActivityArchive>().PurgeAsync("getAllActivityArchive", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<ActivityDeclinedOption>().PurgeAsync("getAllActivityDeclinedOption", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<ActivityStatistics>().PurgeAsync("getAllActivityStatistics", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<ActivityInvitation>().PurgeAsync("getAllNegativeFeedbackPost", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<FeedbackInvitation>().PurgeAsync("getAllFeedbackPost", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<FeedbackPost>().PurgeAsync("getAllPendingFeedback", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<NegativeFeedbackPost>().PurgeAsync("getAllSelectedActivity", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<PendingFeedback>().PurgeAsync("getAllActivityInvitation", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<SelectedActivity>().PurgeAsync("getAllFeedbackInvitation", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<TeamMember>().PurgeAsync("getAllTeamMember", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<User>().PurgeAsync("getAllUser", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<UserSettings>().PurgeAsync("getAllUserSettings", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);
			await client.GetSyncTable<UserActivityOrder>().PurgeAsync("getAllUserActivityOrder", client.GetSyncTable<Activity>().CreateQuery(), CancellationToken.None);

			System.Environment.Exit(0);
		}

		public static async Task<MobileServiceUser> SetUserOrLogin(bool mustLogin = false) {
			if (client.CurrentUser != null && !mustLogin)
			{
				return client.CurrentUser;
			}
			#if __IOS__
				var accountStore = AccountStore.Create(); // Xamarin.IOs
				TalentPlusApp.UIContext = UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController;
			#elif __ANDROID__
				var accountStore = AccountStore.Create(TalentPlusApp.UIContext); // Xamarin.Android
			#endif
			
			var accounts = accountStore.FindAccountsForService("MsAccount").ToArray();
			if (accounts.Count() != 0 && !mustLogin)
			{
				client.CurrentUser = new MobileServiceUser(accounts[0].Username);
				client.CurrentUser.MobileServiceAuthenticationToken = accounts[0].Properties["token"];
				TalentPlusApp.TalentApp.SetLoadingPage();
			}
			else
			{
                try
                {
					//user = await Helpers.UserHelper.CustomLoginAsync("test@playgen.com", "test"); //return null if unauthorized
					//user = await client.LoginAsync(TalentPlusApp.UIContext, MobileServiceAuthenticationProvider.Google);
					client.CurrentUser = null;
					client.CurrentUser = await Helpers.UserHelper.LoginView();
					if (client.CurrentUser != null)
                    {
						var account = new Account(client.CurrentUser.UserId, new Dictionary<string, string> { { "token", client.CurrentUser.MobileServiceAuthenticationToken } });

						foreach (Account acc in accounts) { accountStore.Delete(acc, "MsAccount"); }
                        accountStore.Save(account, "MsAccount");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
					Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "TalentDb.SetuserOrLogin()" },
						{ "MustLogin", mustLogin.ToString() }
					});
                }
			}
			if (string.IsNullOrEmpty(TalentPlusApp.CurrentUserId))
			{
				TalentPlusApp.CurrentUserId = client.CurrentUser.UserId;
				TalentPlusApp.CurrentUser = await TalentDb.GetItem<User>(TalentPlusApp.CurrentUserId);
			}
			TalentPlusApp.CurrentUserId = client.CurrentUser.UserId;
			Insights.Identify(TalentPlusApp.CurrentUserId, "id", TalentPlusApp.CurrentUserId);
			return client.CurrentUser;
		}

		//TODO: check how long this takes and maybe try to optimize and make all db calls run in parallel, or select everything and use linq to join
		public static async Task<IList<Activity>> GetActivities()
		{
			IList<Activity> activities = await client.GetSyncTable<Activity>().ToListAsync();
			foreach(Activity activity in activities) {
				await PopulateActivity(activity);
			}
			activities = activities.Where(a => !a.Order.Hidden).OrderBy(a => a.Order.Order).ToList();

			return activities;
		}

		public static async Task<IList<PendingFeedback>> GetPendingFeedback()
		{
			IList<PendingFeedback> feeds = await client.GetSyncTable<PendingFeedback>().ToListAsync();
			//TODO: if slow - get all relevant activities and then assign
			foreach (PendingFeedback feed in feeds)
			{
				feed.Activity = await GetActivity(feed.ActivityId);
				if (feed.InvolvedUsers != null)
				{
					feed.InvolvedUsers = await client.GetSyncTable<User>().Where(u => feed.InvolvedUserIds.Contains(u.Id)).ToListAsync();
				}
			}
			return feeds;
		}

		public static async Task<IList<ActivityArchive>> GetActivityArchives()
		{
			IList<ActivityArchive> activityArchives = await client.GetSyncTable<ActivityArchive>().ToListAsync();
			foreach (ActivityArchive activityArchive in activityArchives)
			{
				activityArchive.Activity = await GetActivity(activityArchive.ActivityId);
				if (activityArchive.InvolvedUsers != null)
				{
					activityArchive.InvolvedUsers = await client.GetSyncTable<User>().Where(u => activityArchive.InvolvedUserIds.Contains(u.Id)).ToListAsync();
				}
			}
			return activityArchives;
		}

		public static async Task<IList<SelectedActivity>> GetSelectedActivities()
		{
			IList<SelectedActivity> selectedActivities = await client.GetSyncTable<SelectedActivity>().ToListAsync();
			foreach (SelectedActivity selectedActivity in selectedActivities)
			{
				selectedActivity.Activity = await GetActivity(selectedActivity.ActivityId);
				if (selectedActivity.InvolvedUsers != null)
				{
					selectedActivity.InvolvedUsers = await client.GetSyncTable<User>().Where(u => selectedActivity.InvolvedUserIds.Contains(u.Id)).ToListAsync();
				}
			}
			return selectedActivities;
		}

		public static async Task<IList<ActivityInvitation>> GetActivityInvitations()
		{
			IList<ActivityInvitation> activityInvitations = await client.GetSyncTable<ActivityInvitation>().ToListAsync();
			foreach (ActivityInvitation activityInvitation in activityInvitations)
			{
				activityInvitation.Activity = await GetActivity(activityInvitation.ActivityId);
				activityInvitation.SenderUser = await client.GetSyncTable<User>().LookupAsync(activityInvitation.SenderUserId);
				activityInvitation.TargetUser = await client.GetSyncTable<User>().LookupAsync(activityInvitation.TargetUserId);
			}
			return activityInvitations;
		}

		public static async Task<IList<FeedbackInvitation>> GetFeedbackInvitations()
		{
			IList<FeedbackInvitation> feedbackInvitations = await client.GetSyncTable<FeedbackInvitation>().ToListAsync();
			foreach (FeedbackInvitation feedbackInvitation in feedbackInvitations)
			{ 
				feedbackInvitation.Activity = await GetActivity(feedbackInvitation.ActivityId);
				feedbackInvitation.SenderUser = await client.GetSyncTable<User>().LookupAsync(feedbackInvitation.SenderUserId);
				feedbackInvitation.TargetUser = await client.GetSyncTable<User>().LookupAsync(feedbackInvitation.TargetUserId);
			}
			return feedbackInvitations;
		}

		public static async Task<IList<TeamMember>> GetTeamMembers()
		{
			IList<TeamMember> teamUsers = await client.GetSyncTable<TeamMember>().OrderByDescending(tm => tm.LastInteraction).ToListAsync();
			foreach (TeamMember tUser in teamUsers) { tUser.TeamUser = await client.GetSyncTable<User>().LookupAsync(tUser.TeamUserId); }
			return teamUsers;
		}

		public static async Task<IList<TeamMember>> GetTeamMember(string id)
		{
			IList<TeamMember> teamUsers = await client.GetSyncTable<TeamMember>().Where(tm => tm.TeamUserId == id).ToListAsync();
			TeamMember tUser = teamUsers.FirstOrDefault();
			if (tUser != null)
			{
				tUser.TeamUser = await client.GetSyncTable<User>().LookupAsync(tUser.TeamUserId);
			}
			return teamUsers;
		}

		public static async Task<Activity> GetActivity(string id)
		{
			Activity activity = await client.GetSyncTable<Activity>().LookupAsync(id);
			await PopulateActivity(activity);
			return activity;
		}

		public static async Task<IList<FeedbackPost>> GetFeedbackPosts(string activityId, int page = 0)
		{
			return await client.GetSyncTable<FeedbackPost>().Where(a => a.ActivityId == activityId).OrderByDescending(fp => fp.VIPContent).ThenByDescending(fp => fp.Date).Skip(page * PAGE_SIZE).Take(PAGE_SIZE).ToListAsync();
		}

		public static async Task<UserSettings> GetSettings()
		{
			var allSettings = await client.GetSyncTable<UserSettings>().Take(1).ToListAsync();
			return allSettings.FirstOrDefault();
		}

		private static async Task PopulateActivity(Activity activity)
		{
			activity.Theme = await client.GetSyncTable<ActivityTheme>().LookupAsync(activity.ThemeId);

			var stats = await client.GetSyncTable<ActivityStatistics>().Take(1).Where(a => a.ActivityId == activity.Id).ToCollectionAsync();
			activity.ActivityStatistics = stats.FirstOrDefault();
			activity.ActivityStatistics.Activity = activity;

			activity.ActivityDeclinedOptions = await client.GetSyncTable<ActivityDeclinedOption>().Where(a => a.ActivityId == activity.Id).OrderBy(ado => ado.Order).ToListAsync();
			foreach (var post in activity.ActivityDeclinedOptions) { post.Activity = activity; }

			activity.FeedbackPosts = await client.GetSyncTable<FeedbackPost>().Where(a => a.ActivityId == activity.Id).OrderByDescending(fp => fp.VIPContent).ThenByDescending(fp => fp.Date).Take(PAGE_SIZE).ToListAsync();
			foreach (var post in activity.FeedbackPosts) { post.Activity = activity; }

			var orderList = await client.GetSyncTable<UserActivityOrder>().Take(1).Where(uao => uao.ActivityId == activity.Id).ToListAsync();
			var order = orderList.FirstOrDefault();
			if (order == null)
			{
				order = new UserActivityOrder { ActivityId = activity.Id, Order = 0, Hidden = false };
			}
			activity.Order = order;

		}

		public static async Task<IList<T>> GetItems<T>() where T : TalentPlus.Shared.IBaseEntity, new()
		{
			return await client.GetSyncTable<T>().ToListAsync();
		}

		public static async Task<T> GetItem<T>(string id) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			return await client.GetSyncTable<T>().LookupAsync(id);
		}

		public static async Task<bool> SaveOrUpdateItem<T>(T item) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			if (!string.IsNullOrWhiteSpace(item.Id))
			{
				await client.GetSyncTable<T>().UpdateAsync(item);
			}
			else
			{
				item.Id = Guid.NewGuid().ToString();
				await client.GetSyncTable<T>().InsertAsync(item);
			}
			return await PushOrDiscardAsync();
		}

		public static void SaveOrUpdateItemInThread<T>(Object stateInfo) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(CallSaveOrUpdateItem<T>), stateInfo);
		}

		private static async void CallSaveOrUpdateItem<T>(Object stateInfo) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			await TalentDb.SaveOrUpdateItem<T>((T)stateInfo);
		}

		public static async Task SaveItems<T>(IEnumerable<T> items) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			List<Task> tasks = new List<Task>();
			foreach (T item in items)
			{
				tasks.Add(SaveOrUpdateItem(item));
			}
			await Task.WhenAll(tasks);
		}

		public static async Task DeleteItem<T>(T item) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			try{
				await client.SyncContext.PushAsync();
				await client.GetSyncTable<T>().DeleteAsync(item);
				await client.SyncContext.PushAsync();
			}
			catch (Exception ex) {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "TalentDb.DeleteItem()" },
					{ "What", item.GetType().ToString() },
					{ "Id", item.Id	}
				});
			}
		}

		public static void DeleteItemInThread<T>(Object stateInfo) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(CallDeleteItem<T>), stateInfo);
		}

		private static async void CallDeleteItem<T>(Object stateInfo) where T : TalentPlus.Shared.IBaseEntity, new()
		{
			await TalentDb.DeleteItem<T>((T)stateInfo);
		}

		public static async Task<bool> PushOrDiscardAsync()
		{
			bool pushFailed = false;
			MobileServiceTableOperationError error = null;
			try
			{
				await client.SyncContext.PushAsync();
			}
			catch (MobileServicePushFailedException ex)
			{
				error = ex.PushResult.Errors.FirstOrDefault();
				if (error != null && error.Status == HttpStatusCode.Forbidden)
				{
					pushFailed = true;
				}
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "TalentDb.PushOrDiscardAsync()" }
				});
			}

			if (pushFailed)
			{
				await error.CancelAndDiscardItemAsync();
				return false;
			}
			return true;
		}

		public static async Task<int> GetTotalFeedbackCount()
		{

			var allFeedbacks = await client.GetSyncTable<ActivityStatistics>().Select(a => a.FeedbackCount).ToListAsync();
			return allFeedbacks.Sum();
		}
	}
}
