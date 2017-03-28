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
using DSoft.Messaging;

using Thread = Java.Lang.Thread;

namespace TalentPlusAndroid
{
	[Activity(Label = "TalentPlus", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan)]
	public class EmptyActivity: Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			Intent intent = new Intent (this, typeof (MainActivity));
			intent.AddFlags(intent.Flags | ActivityFlags.BroughtToFront | ActivityFlags.ReorderToFront | ActivityFlags.NewTask);
			StartActivity (intent);

			Finish ();
        }
    }
}

