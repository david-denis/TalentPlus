using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Graphics;
using TalentPlus.Android;
using TalentPlus.Shared;

namespace TalentPlusAndroid
{
	[BroadcastReceiver]
	public class AlarmReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var message = intent.GetStringExtra("message");
			var title = intent.GetStringExtra("title");
			string activityId = intent.GetStringExtra("id");
			int alarmId = intent.GetIntExtra("alarmid", 0);

			var notIntent = new Intent(context, typeof(MainActivity));
			var contentIntent = PendingIntent.GetActivity(context, alarmId, notIntent, PendingIntentFlags.CancelCurrent);
			var manager = NotificationManagerCompat.From(context);

			var style = new NotificationCompat.BigTextStyle();
			style.BigText(message);

			TalentPlusApp.AddPendingFeedback(activityId, DateTime.Now);

			//Generate a notification with just short text and small icon
			var builder = new NotificationCompat.Builder(context)
							.SetContentIntent(contentIntent)
							.SetSmallIcon(Resource.Drawable.ic_launcher)
							.SetContentTitle(title)
							.SetContentText(message)
							.SetStyle(style)
							.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
							.SetAutoCancel(true);


			var notification = builder.Build();
			manager.Notify(0, notification);
		}
	}
}