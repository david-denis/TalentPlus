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
using System.ComponentModel;

[assembly: ExportRenderer(typeof(RoundedBox), typeof(RoundedBoxRenderer))]

namespace TalentPlusAndroid
{
	public class RoundedBoxRenderer : BoxRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				SetWillNotDraw (false);
				Invalidate ();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == RoundedBox.CornerRadiusProperty.PropertyName)
			{
				Invalidate();
			}
		}

		public override void Draw(Canvas canvas)
		{
			var box = Element as RoundedBox;
			var rect = new Rect();
			var paint = new Paint()
			{
				Color = box.BackgroundColor.ToAndroid(),
				AntiAlias = true,
			};

			GetDrawingRect(rect);

			var radius = (float)(rect.Width() / box.Width * box.CornerRadius);

			if (box.RoundedSide == RoundedBox.RoundedSideType.Top)
			{
				canvas.DrawRect(new RectF(rect.Left, rect.Top + Convert.ToInt32(Math.Floor(box.CornerRadius)) * 2, rect.Right, rect.Bottom), paint);
			}
			else if (box.RoundedSide == RoundedBox.RoundedSideType.Bottom)
			{
				canvas.DrawRect(new RectF(rect.Left, rect.Top, rect.Right, rect.Bottom - Convert.ToInt32(Math.Floor(box.CornerRadius)) * 2), paint);
			}
			canvas.DrawRoundRect(new RectF(rect), radius, radius, paint);
		}
	}
}