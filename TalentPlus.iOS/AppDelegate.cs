using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using XLabs.Ioc;
using XLabs.Platform.Mvvm;
using XLabs.Forms;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using TalentPlus.Shared;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace TalentPlus.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Foundation.Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate // superclass new in 1.3
	{
		// class-level declarations
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//global::Xamarin.Forms.Forms.Init();

			//// create a new window instance based on the screen size
			//window = new UIWindow(UIScreen.MainScreen.Bounds);
			//UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
			//	{
			//		TextColor = UIColor.White
			//	});
			//Forms.Init();
			//page = TalentPlus.Shared.TalentPlusApp.RootPage;
			//window.RootViewController = page.CreateViewController ();
			//// If you have defined a root view controller, set it here:
			//// window.RootViewController = myViewController;
			
			//// make the window visible
			//window.MakeKeyAndVisible ();
			
			//return true;

			if (!Resolver.IsSet)
			{
				this.SetIoc();
			}
			else
			{
				var appRoot = Resolver.Resolve<IXFormsApp>() as IXFormsApp<XFormsApplicationDelegate>;
				appRoot.AppContext = this;
			}

			global::Xamarin.Forms.Forms.Init();

			Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
			{
				/*if (!string.IsNullOrWhiteSpace(e.View.StyleId))
				{
					e.NativeView.ContentDescription = e.View.StyleId;
				}*/
			};

			//UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge;
			//UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			Version version = new Version (UIDevice.CurrentDevice.SystemVersion);

			if (version >= new Version(8, 0)) {
				var settings = UIUserNotificationSettings.GetSettingsForTypes(
					UIUserNotificationType.Alert
					| UIUserNotificationType.Badge
					| UIUserNotificationType.Sound,
					new NSSet());
				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
			} 
			else {
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);

			}

			var theApp = new TalentPlus.Shared.TalentPlusApp ();
			LoadApplication(theApp);  // method is new in 1.3

			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

			if (options != null) {
				if (options.ContainsKey (UIApplication.LaunchOptionsLocalNotificationKey)) {
					var localNotification = options [UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
					if (localNotification != null) {
						new UIAlertView (localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show ();

						// Do Activity action
						NSObject activityId;
						localNotification.UserInfo.TryGetValue (NSObject.FromObject ("id"), out activityId);

						TalentPlusApp.AddPendingFeedback(activityId.ToString(), DateTime.Now);
					}
				}
			}

			return base.FinishedLaunching(app, options);
		}

		/// <summary>
		/// Sets the IoC.
		/// </summary>
		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();

			var app = new XFormsAppiOS();

			app.Init(this);

			resolverContainer.Register<IDevice>(t => AppleDevice.CurrentDevice)
				.Register<IDisplay>(t => t.Resolve<IDevice>().Display)
				//.Register<IMediaPicker>(t => new TalentPlusAndroid.Services.Media.MediaPicker())
				//.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>()
				.Register<IDependencyContainer>(resolverContainer)
				.Register<IXFormsApp>(app)
				.Register<ISecureStorage, SecureStorage>();

			Resolver.SetResolver(resolverContainer.GetResolver());
		}

		// Callback for Local Notification
		public override async void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			new UIAlertView (notification.AlertAction, notification.AlertBody, null, "OK", null).Show ();

			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

			// Do Activity action
			NSObject activityId;
			notification.UserInfo.TryGetValue (NSObject.FromObject ("id"), out activityId);

			TalentPlusApp.AddPendingFeedback(activityId.ToString(), DateTime.Now);
		}

		public string DeviceToken { get; set; }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
			//string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");
			// Modify device token
			DeviceToken = deviceToken.Description;
			DeviceToken = DeviceToken.Trim('<', '>').Replace(" ", "");

			//if (!DeviceToken.Equals(lastDeviceToken))
			//{
				// Get Mobile Services client
				MobileServiceClient client = TalentDb.client;

				// Register for push with Mobile Services
				IEnumerable<string> tag = new List<string>();
				var push = client.GetPush();
				push.RegisterNativeAsync(DeviceToken, tag);
			//	NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "deviceToken");
			//}
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
        }

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			//This method gets called whenever the app is already running and receives a push notification
			// YOU MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
			// this includes setting the badge, playing a sound, etc.
			processNotification(userInfo, false);
		}

		void processNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;
				string sound = string.Empty;
				int badge = -1;

				//Extract the alert text
				//NOTE: If you're using the simple alert by just specifying "  aps:{alert:"alert msg here"}  "
				//      this will work fine.  But if you're using a complex alert with Localization keys, etc., your "alert" object from the aps dictionary
				//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps[new NSString("alert")] as NSString).ToString();

				//Extract the sound string
				if (aps.ContainsKey(new NSString("sound")))
					sound = (aps[new NSString("sound")] as NSString).ToString();

				//Extract the badge
				if (aps.ContainsKey(new NSString("badge")))
				{
					string badgeStr = (aps[new NSString("badge")] as NSObject).ToString();
					int.TryParse(badgeStr, out badge);
				}

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually set the badge in case this came from a remote notification sent while the app was open
					if (badge >= 0)
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
					//Manually play the sound
					if (!string.IsNullOrEmpty(sound))
					{
						//This assumes that in your json payload you sent the sound filename (like sound.caf)
						// and that you've included it in your project directory as a Content Build type.
						var soundObj = AudioToolbox.SystemSound.FromFile(sound);
						soundObj.PlaySystemSound();
					}

					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						if (alert == "purge_data")
						{
							Device.BeginInvokeOnMainThread (async () => 
							{
								await TalentPlusApp.PurgeData();		
							});
						}
						else if (alert == "feedback_update") {
							ViewModelLocator.ActivitiesViewModel.RefreshActivitiesFeedbacksCommand.Execute (null);
						} else {
							UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
							avAlert.Show();
							ViewModelLocator.ActivitiesViewModel.LoadInvitationsCommand.Execute (null);
						}
					}
				}
			}

			//You can also get the custom key/value pairs you may have sent in your aps (outside of the aps payload in the json)
			// This could be something like the ID of a new message that a user has seen, so you'd find the ID here and then skip displaying
			// the usual screen that shows up when the app is started, and go right to viewing the message, or something like that.
			if (null != options && options.ContainsKey(new NSString("customKeyHere")))
			{
				var launchWithCustomKeyValue = (options[new NSString("customKeyHere")] as NSString).ToString();

				//You could do something with your customData that was passed in here
			}
		}
	}
}

