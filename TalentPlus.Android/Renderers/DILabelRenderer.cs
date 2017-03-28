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

[assembly: ExportRenderer(typeof(DILabel), typeof(DILabelRenderer))]
namespace TalentPlusAndroid
{
    public class DILabelRenderer : LabelRenderer
    {
        public DILabelRenderer()
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

			if (e.NewElement != null) {
				var view = (DILabel)Element;
				var control = Control;

				control.Clickable = !view.IsDefaultLabel;

				control.Click += delegate {
					if (view.Tapped != null)
						view.Tapped.Invoke ();
				};

				if (view.Lines != 0)
					control.SetLines (view.Lines);
			}
        }
    }
}
