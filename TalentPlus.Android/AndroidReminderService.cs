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
using TalentPlus.Shared;
using Xamarin.Forms;
using Java.Util;

[assembly: Xamarin.Forms.Dependency(typeof(TalentPlusAndroid.AndroidReminderService))]

namespace TalentPlusAndroid
{
	public class AndroidReminderService : IReminderService
	{
        public Context alarmContext { get; set; }
		#region IReminderService implementation

		public void Remind(DateTime dateTime, string title, string message, string activityId, int alarmId)
		{
            if (alarmContext == null)
                alarmContext = Forms.Context;

			Intent alarmIntent = new Intent(Forms.Context, typeof(AlarmReceiver));
			alarmIntent.PutExtra("message", message);
			alarmIntent.PutExtra("title", title);
			alarmIntent.PutExtra("id", activityId);
			alarmIntent.PutExtra("alarmid", alarmId);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(alarmContext, alarmId, alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)alarmContext.GetSystemService(Context.AlarmService);

			//TODO: For demo set after 5 seconds.
			alarmManager.Set(AlarmType.Rtc, DateTimeExtensions.ToAndroidTimestamp(dateTime), pendingIntent);
		}

		public void Remind(int delayTime, string title, string message, string activityId, int alarmId)
		{
		}
		#endregion
	}
}