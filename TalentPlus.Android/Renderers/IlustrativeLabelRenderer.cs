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
using Xamarin.Forms;
using TalentPlus.Shared;
using TalentPlusAndroid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(IlustrativeLabel), typeof(IlustrativeLabelRenderer))]

namespace TalentPlusAndroid
{
	class IlustrativeLabelRenderer : LabelRenderer
	{
		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			var label = (TextView)Control; // for example
			Typeface font = Typeface.CreateFromAsset(Forms.Context.Assets, "UnileverIllustrativeTypeBold.ttf");
			label.Typeface = font;
		}
	}
}