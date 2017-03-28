using System.Text;
using System;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using System.Threading;
using System.Threading.Tasks;
using Android.Media;
using System.Collections.Generic;
using DSoft.Messaging;
using Android.Support.V4.App;
using Microsoft.WindowsAzure.MobileServices;
using TalentPlus.Shared;
using Xamarin;
using Xamarin.Forms;

namespace TalentPlusAndroid
{
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "com.playgen.talentplus" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "com.playgen.talentplus" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "com.playgen.talentplus" })]

	public class GcmBroadcastReceiver: GcmBroadcastReceiverBase<PushHandlerService>
	{
		public static string[] SENDER_IDS = new string[]{TalentPlus.Shared.Config.GCM_SENDER_ID};

		public const string TAG = "PushSharp-GCM";
	}

	[Service]
	public class PushHandlerService : GcmServiceBase
	{
		public static string RegistrationID { get; private set; }

		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS){
		}

		const string TAG = "TalentPlus";

		protected override void OnRegistered(Context context, string registrationId){
			System.Diagnostics.Debug.WriteLine("The device has been registered with GCM.", "Success!");

			// Get the MobileServiceClient from the current activity instance.
			MobileServiceClient client = TalentDb.client;
			var push = client.GetPush();

			List<string> tags = null;

			//// (Optional) Uncomment to add tags to the registration.
			//var tags = new List<string>() { "myTag" }; // create tags if you want

			try
			{
				// Make sure we run the registration on the same thread as the activity, 
				// to avoid threading errors.
				MainActivity.CurrentActivity.RunOnUiThread(
					async () => {
						try
						{
							await push.RegisterNativeAsync(registrationId, tags);
						} catch(Exception e){

						}
					});
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					string.Format("Error with Azure push registration: {0}", ex.Message));
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "GCMService.OnRegistered()" },
					{ "Id", registrationId }
				});
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId){
			Log.Verbose (TAG, "GCM Unregistered : " + registrationId);
		}

		protected override void OnMessage(Context context, Intent intent){
            Dictionary<string, string> parts = new Dictionary<string, string>();
			try{
                Log.Info(TAG, "GCM Message Received!");
                var msg = new StringBuilder();
                
				if (intent != null && intent.Extras != null) {
					foreach(var key in intent.Extras.KeySet()){
						parts[key.ToString()] = intent.Extras.Get(key).ToString();
					}
				}

				string message = parts["message"];
				/*string title = parts["title"];
				string subtitle = parts["subtitle"];
				string tickerText = parts["tickerText"];
				string vibrate = parts["vibrate"];
				string sound = parts["sound"];
				string largeIcon = parts["largeIcon"];
				string smallIcon = parts["smallIcon"];

				createNotification(title, subtitle, message, tickerText, vibrate, sound, largeIcon, smallIcon);*/
				if(message == "purge_data")
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						await TalentPlusApp.PurgeData();
					});
				}
				else if (message == "feedback_update")
				{
					ViewModelLocator.ActivitiesViewModel.RefreshActivitiesFeedbacksCommand.Execute(null);
				}
				else
				{
					createNotification("TalentPlus", "TalentPlus", message, message, "", "", "", "");
					ViewModelLocator.ActivitiesViewModel.LoadInvitationsCommand.Execute(null);
				}
			}catch(Exception e){
				Console.WriteLine (e.Message);
				Insights.Report(e, new Dictionary<string, string>
				{
					{ "Where", "GcmService.OnMessage()" },
					{ "What", parts.ToString() }
				});
			}
		}

		protected override bool OnRecoverableError(Context context, string errorId){
			Log.Warn (TAG, "Recoverable Error:" + errorId);
			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError(Context context, string errorId){
			Log.Error (TAG, "GCM Error:" + errorId);
		}

		void createNotification (string title, string subtitle, string message, string tickertext, string vibrate, string sound, string largeIcon, string smallIcon)
		{


			Intent uiIntent = new Intent (this, typeof(EmptyActivity));

			/*Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create (this);
			stackBuilder.AddParentStack (Java.Lang.Class.FromType(typeof(EmptyActivity)));
			stackBuilder.AddNextIntent (uiIntent);


			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent (0, (int)PendingIntentFlags.UpdateCurrent);*/

			const int pendingIntentId = 0;

			PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, pendingIntentId, uiIntent, PendingIntentFlags.OneShot);

			NotificationCompat.BigTextStyle textStyle = new NotificationCompat.BigTextStyle ();
			textStyle.BigText (message);

			NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
				.SetAutoCancel (true)
				.SetContentIntent (resultPendingIntent)
				.SetContentTitle (title)
				.SetTicker (tickertext)
			    //.SetSubText (subtitle)
				.SetContentText (message)
				.SetDefaults (NotificationCompat.DefaultSound | NotificationCompat.DefaultVibrate)
				.SetSmallIcon(TalentPlus.Android.Resource.Drawable.ic_launcher);

			builder.SetStyle (textStyle);

			if (!String.IsNullOrEmpty(vibrate) && vibrate.Equals ("1")) {
				builder.SetVibrate(new long[] { 500, 500, 500, 500, 500, 500, 500, 500, 500 });
			}

			if (!String.IsNullOrEmpty (sound) && sound.Equals ("1")) {
				builder.SetSound (RingtoneManager.GetDefaultUri (RingtoneType.Notification));
			}

			//Create notification
			NotificationManager notificationManager = GetSystemService (Context.NotificationService) as NotificationManager;

			notificationManager.Notify (1, builder.Build());
		}

	}
}

