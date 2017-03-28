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

[assembly: ExportRenderer (typeof(WrappedTruncatedLabel), typeof(WrappedTruncatedLabelRenderer))]

namespace TalentPlusAndroid
{
	class WrappedTruncatedLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				if (Control != null) {
					Control.LayoutChange += (s, args) => {
						Control.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
						Control.SetMaxLines (((WrappedTruncatedLabel)Element).Lines);
					};
				}
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {
			case "Lines":
				if (Control != null)
					Control.SetMaxLines (((WrappedTruncatedLabel)Element).Lines);
				break;
			}
		}
	}
}