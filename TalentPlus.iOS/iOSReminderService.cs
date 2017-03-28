using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TalentPlus.Shared;
using Xamarin.Forms;
using UIKit;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(TalentPlusiOS.iOSReminderService))]

namespace TalentPlusiOS
{
	public class iOSReminderService : IReminderService
	{
		#region IReminderService implementation

		public void Remind(int delayTime, string title, string message, string activityId, int alarmId)
		{
			UILocalNotification localNotification = new UILocalNotification ();
			NSMutableDictionary userInfo = new NSMutableDictionary ();
			userInfo.Add(NSObject.FromObject("id"), NSObject.FromObject(activityId));
			//userInfo.SetValueForKey (NSObject.FromObject(alarmId), new NSString("alarmid"));
			localNotification.FireDate = NSDate.FromTimeIntervalSinceNow (delayTime);
			localNotification.AlertAction = title;
			localNotification.AlertBody = message;
			localNotification.UserInfo = userInfo;

			localNotification.ApplicationIconBadgeNumber = 1;

			UIApplication.SharedApplication.ScheduleLocalNotification (localNotification);
		}

		public void Remind(DateTime time, string title, string message, string activityId, int alarmId)
		{
		}
		#endregion
	}
}