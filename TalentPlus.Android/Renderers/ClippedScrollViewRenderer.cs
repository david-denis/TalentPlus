using Android.Graphics;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;
using Android.Text.Util;
using TalentPlus.Shared.Helpers;
using TalentPlusAndroid;
using System.ComponentModel;
using Android.Views;
using TalentPlus.Shared;

[assembly: ExportRenderer(typeof(ClippedScrollView), typeof(ClippedScrollViewRenderer))]
namespace TalentPlusAndroid
{
	public class ClippedScrollViewRenderer : ScrollViewRenderer
	{
		public override void Draw(Canvas canvas)
		{
			base.Draw(canvas);
			//canvas.ClipRect(canvas.ClipBounds);
		}
	}
}