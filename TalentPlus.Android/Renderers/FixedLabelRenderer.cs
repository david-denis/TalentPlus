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

[assembly: ExportRenderer(typeof(FixedLabel), typeof(FixedLabelRenderer))]
namespace TalentPlusAndroid
{
	public class FixedLabelRenderer : ViewRenderer
	{
		public TextView TextView;

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				var label = Element as FixedLabel;
				TextView = new TextView (Context);
				TextView.Text = label.Text;
				TextView.SetTextColor (label.TextColor.ToAndroid ());

				int height = ViewGroup.LayoutParams.WrapContent;
				int width = ViewGroup.LayoutParams.MatchParent;

				if (label.FixedWidth > 0) {
					width = (int)label.Width;
				}
				if (label.FixedHeight > 0) {
					height = (int)label.Height;
				}

				TextView.LayoutParameters = new LayoutParams (width, height);

				TextView.TextSize = label.FontSize;
				TextView.Gravity = ConvertXAlignment (label.XAlign) | ConvertYAlignment (label.YAlign);
				TextView.SetSingleLine (label.LineBreakMode != LineBreakMode.WordWrap);
				if (label.LineBreakMode == LineBreakMode.TailTruncation) {
					TextView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
				}

				SetNativeControl (TextView);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var element = (Element as FixedLabel);
			switch (e.PropertyName)
			{
				case "Text":
					TextView.Text = element.Text;
					break;

				case "FontSize":
					TextView.TextSize = element.FontSize;

					break;

				case "TextColor":
					TextView.SetTextColor(element.TextColor.ToAndroid());
					break;

				case "XAlign":

				case "YAlign":

					TextView.Gravity = ConvertXAlignment(element.XAlign) | ConvertYAlignment(element.YAlign);
					break;

				default:
					break;
			}

			base.OnElementPropertyChanged(sender, e);
		}

		static GravityFlags ConvertXAlignment(Xamarin.Forms.TextAlignment xAlign)
		{
			switch (xAlign)
			{
				case Xamarin.Forms.TextAlignment.Center:
					return GravityFlags.CenterHorizontal;
				case Xamarin.Forms.TextAlignment.End:
					return GravityFlags.End;
				default:
					return GravityFlags.Start;
			}
		}

		static GravityFlags ConvertYAlignment(Xamarin.Forms.TextAlignment yAlign)
		{
			switch (yAlign)
			{
				case Xamarin.Forms.TextAlignment.Center:
					return GravityFlags.CenterVertical;
				case Xamarin.Forms.TextAlignment.End:
					return GravityFlags.Bottom;
				default:
					return GravityFlags.Top;
			}
		}
	}
}