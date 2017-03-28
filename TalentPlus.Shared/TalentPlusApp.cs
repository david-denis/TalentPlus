using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Services;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services;
using System.Threading.Tasks;
using Xamarin;

#if __IOS__
using UIContext = UIKit.UIViewController;

#elif __ANDROID__
using UIContext = global::Android.Content.Context;
using Gcm.Client;
using TalentPlusAndroid;
#endif

namespace TalentPlus.Shared
{
	public class TalentPlusApp : Application
	{
		#if __ANDROID__
		public static bool JustResume { get; set; }
		#endif

		public static TalentPlusApp TalentApp { get; set; }

		public static UIContext UIContext { get; set; }

		public static UserSettings Settings { get; set; }

		public static bool BackBlocked { get; set; }

		public LoadingPage LoadingPageView { get; set; }

		public DisclaimerPage DisclaimerPageView { get; set; }

		private static HomeTabbedView homeView;

		public static HomeTabbedView RootPage {
			get { return homeView ?? (homeView = new HomeTabbedView ()); }
		}

		public static Page SplashPage { get; set; }

		public static bool PendingNotification { get; set; }

		public static ActivitiesViewModel Vm;

		public static ObservableCollection<PendingFeedback> PendingFeedback {
			get { return Vm.PendingFeedbacks; }
		}

		public static string CurrentUserId { get; set; }

		public static void AddPendingFeedback (string activityId, DateTime receiveTime)
		{
			PendingNotification = true;
			Vm.AddPendingFeedback (activityId, receiveTime);
		}

		public static User CurrentUser { get; set; }

		private CarouselPage introcarousel { get; set; }
		private List<IntroView> introViewArr;

		public static Page GetMainPage ()
		{
			//return new TestView();
			SplashPage = new SplashView ();
			return SplashPage;
		}

		public static Page GetRootPage ()
		{
			ViewFactory.Register<CameraPage, CameraViewModel> ();
			ViewFactory.Register<VideoPage, VideoViewModel> ();

			return RootPage;
		}

		public static bool Init ()
		{
#if __IOS__
            SQLitePCL.CurrentPlatform.Init ();
#endif
			var app = Resolver.Resolve<IXFormsApp> ();
			Vm = ViewModelLocator.ActivitiesViewModel;
			//System.Threading.Tasks.Task.Run(async () => await TalentDb.InitializeAsync()).Wait();

			return true;
		}

		public static async Task PurgeData()
		{
			TalentApp.SetLoadingPage(); //TODO: make a custom message saying that we are resetting data, and app will turn off
			await TalentDb.PurgeData();
		}

		public static async Task<bool> LoginAndSync ()
		{
			bool bResult = await TalentDb.InitializeAsync ();

			if (bResult) {
				await TalentDb.SyncDatabase ();
				var sett = await TalentDb.GetSettings ();
				if (sett != null) {
					Settings = sett;
					Helpers.Color.Primary = sett.ColorPrimary;
					Helpers.Color.Secondary = sett.ColorSecondary;
				}
				TalentPlusApp.CurrentUser = await TalentDb.GetItem<User> (TalentPlusApp.CurrentUserId);
				if (TalentPlusApp.CurrentUser != null && TalentPlusApp.CurrentUser.NeedPurging)
				{
					await TalentPlusApp.PurgeData();
					return false;
				}
				Insights.Identify (TalentPlusApp.CurrentUserId, Insights.Traits.Name, TalentPlusApp.CurrentUser.Name);
#if __ANDROID__
				RegisterGCM ();
#endif
#if __IOS__
				UIKit.UIApplication.SharedApplication.RegisterForRemoteNotifications();
#endif
				//if (RootPage != null) {
				//	((HomeView)RootPage).RefreshCurrentPage ();
				//}

				await PopulatePendingFeedbacks ();
			}

			if (bResult == false) {
				return false;
			}                                                                                                 

			return true;
		}

		private static async Task PopulatePendingFeedbacks()
		{
			var selectedActivities = await TalentDb.client.GetSyncTable<SelectedActivity> ().Where (sa => sa.FinishTime < DateTime.Now).ToListAsync ();
			if (selectedActivities.Count < 1)
			{
				return;
			}
				
			foreach (SelectedActivity sAct in selectedActivities)
			{
				var pendingFeedback = await TalentDb.client.GetSyncTable<PendingFeedback> ().Where (pf => pf.ActivityId == sAct.ActivityId).Take (1).ToListAsync ();
				if (pendingFeedback.Count < 1) //if pending feedback doesn't exist already
				{
					await TalentDb.SaveOrUpdateItem<PendingFeedback> (new TalentPlus.Shared.PendingFeedback {
						ActivityId = sAct.ActivityId,
						ReceiveTime = DateTime.Now,
						InvolvedUserIds = sAct.InvolvedUserIds
					});
					await TalentDb.DeleteItem<SelectedActivity>(sAct);
				}
			}
		}

		public void SetMainPage ()
		{
			if(LoadingPageView != null)
				LoadingPageView.StopAnimateLoading();
			MainPage = GetRootPage ();
		}

		public void SetLoginPage (Page loginPage)
		{
			MainPage = loginPage;
		}

		public void SetLoadingPage ()
		{
			if (LoadingPageView == null)
				LoadingPageView = new LoadingPage ();

			LoadingPageView.AnimateLoading ();
			MainPage = LoadingPageView;
		}

		public async void SetIntroPage()
		{
			if (LoadingPageView != null)
				LoadingPageView.StopAnimateLoading ();

			Action<bool> pageMoveEvent = new Action<bool> (IntroPageMove_Clicked);

			introViewArr = new List<IntroView>();
			introViewArr.Add(new IntroView ("intro1background.jpg", "Developing yourself and others", "", pageMoveEvent));
			introViewArr.Add(new IntroView ("intro2background.jpg", "Step 1: Select an Activity", "", pageMoveEvent));
			introViewArr.Add(new IntroView ("intro3background.jpg", "Step 2: Define your timeline","",  pageMoveEvent));
			introViewArr.Add( new IntroView ("intro4background.jpg", "Step 3: Invite colleagues to join","",  pageMoveEvent));
			introViewArr.Add(new IntroView ("intro5background.jpg", "Step 4: Do the activity", "", pageMoveEvent));
			introViewArr.Add(new IntroView ("intro6background.jpg", "Step 5: Share your insight", "", pageMoveEvent));

			introcarousel = new CarouselPage () {
				Children = {
					introViewArr[0],
					introViewArr[1],
					introViewArr[2],
					introViewArr[3],
					introViewArr[4],
					introViewArr[5],
				},
				Padding = 0,
			};

			MainPage = introcarousel;
		}

		private void IntroPageMove_Clicked(bool bMoveForward)
		{
			if (introcarousel != null) {
				IntroView curView = introcarousel.CurrentPage as IntroView;
				if (curView != null) {
					int idx = introViewArr.IndexOf (curView);

					if (bMoveForward) {
						if (idx >= 0 && idx < introViewArr.Count - 1) {
							introcarousel.SelectedItem = introViewArr [idx + 1];
						} else {
							SetMainPage ();
						}
					} else {
						if (idx >= 1 && idx < introViewArr.Count)
							introcarousel.SelectedItem = introViewArr [idx - 1];
					}
				}
			}
		}

		public void StopPotentialLoadings ()
		{
			if (LoadingPageView != null) {
				LoadingPageView.StopAnimateLoading ();
			}
			if (DisclaimerPageView != null) {
				DisclaimerPageView.StopAnimateLoading ();
			}
		}

		public void SetDisclaimerPage (TaskCompletionSource<object> clickedTask)
		{
			LoadingPageView.StopAnimateLoading ();
			DisclaimerPageView = new DisclaimerPage (clickedTask);
			MainPage = DisclaimerPageView;
		}

		public TalentPlusApp ()
		{
			TalentApp = this;

			Settings = new UserSettings 
			{ 
				Sound = true, 
				ColorPrimary = 0x0091D1, 
				ColorSecondary = 0xEC8B24 
			};
					
			//var color = Helpers.Color.FromHex (-16022072);
			//Console.WriteLine ("R:{0}, G:{1}, B:{2}", color.R * 255, color.G * 255, color.B * 255);

			BackBlocked = false;

			Init ();

			MainPage = GetMainPage ();
		}

		#if __ANDROID__
		private static void RegisterGCM ()
		{
			try {
				GcmClient.CheckDevice (UIContext);
				GcmClient.CheckManifest (UIContext);
				String id = GcmClient.GetRegistrationId (UIContext);
				if (String.IsNullOrEmpty (id))
					GcmClient.Register (UIContext, GcmBroadcastReceiver.SENDER_IDS);
			} catch (Exception e) {
				Console.WriteLine ("Register GCM Failed : " + e.Message);
				Insights.Report (e, new Dictionary<string, string> {
					{ "Where", "RegisterGCM" }
				});
			}
		}
		#endif
	}
}

