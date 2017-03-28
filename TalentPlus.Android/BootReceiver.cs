using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin;
using Android.Locations;
using Android.Content;

using Thread = Java.Lang.Thread;
using TalentPlus.Shared;
using System.Collections.Generic;

namespace TalentPlusAndroid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted }, Categories = new[] { Android.Content.Intent.CategoryDefault })]
    partial class BootReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            if ((intent.Action != null) && (intent.Action == Android.Content.Intent.ActionBootCompleted))
            {
                try
                {
                    bool result = await TalentDb.InitializeAsync(false);
                    if (result == true)
                    {
                        var SelectedActivityList = await TalentDb.GetSelectedActivities();
                        foreach (SelectedActivity activity in SelectedActivityList)
                        {
                            if (activity.FinishTime < DateTime.Now)
                            {
                                //Toast.MakeText(context, "Removed", ToastLength.Long).Show();

                                try
                                {
                                    await TalentDb.DeleteItem<SelectedActivity>(activity);
                                }
                                catch (Exception ex)
                                {
									Insights.Report(ex, new Dictionary<string, string>
									{
										{ "Where", "TalentPlusAndroid.BootReceiver.OnReceive()" }
									});
                                }
                            }
                            else
                            {
                                //Toast.MakeText(context, "Alarm Set", ToastLength.Long).Show();

                                try
                                {
                                    AndroidReminderService reminder = new AndroidReminderService();
                                    reminder.alarmContext = context;
                                    reminder.Remind(activity.FinishTime, "Activity Reminder", activity.Activity.ShortDescription, activity.ActivityId, activity.Activity.AlarmId);
                                }
                                catch (Exception ex)
                                {
									Insights.Report(ex, new Dictionary<string, string>
									{
										{ "Where", "TalentPlusAndroid.BootReceiver.OnReceive()" }
									});
                                }
                            }
                        }
                    }
                    else
                    {
                        //Toast.MakeText(context, "DB Initialization failed", ToastLength.Long).Show();
                    }
                }
                catch (Exception ex)
                {
					Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "TalentPlusAndroid.BootReceiver.OnReceive()" }
					});
                    //Toast.MakeText(context, ex.ToString(), ToastLength.Long).Show();
                }
            }
        }   
    }
}