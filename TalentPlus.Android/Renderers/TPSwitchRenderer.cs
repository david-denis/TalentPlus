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
using TalentPlus.Shared;
using Android.Graphics.Drawables;

[assembly: ExportRenderer (typeof(TPSwitch), typeof(TPSwitchRenderer))]
namespace TalentPlusAndroid
{
	public class TPSwitchRenderer : SwitchRenderer
	{
		public TPSwitchRenderer () : base ()
		{
            
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Switch> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				MessagingCenter.Subscribe<string> (this, "UpdateColorWheel", (message) => {

					if (Control != null) {
						SetColor ();
						bool isSelected = this.Control.Checked;
						this.Control.Checked = !isSelected;
						this.Control.Checked = isSelected;
						this.Invalidate ();
					}
				});
			
				SetColor ();
				this.Control.Checked = this.Element.IsToggled;
			}
		}

		protected override void Dispose (bool disposing)
		{
			MessagingCenter.Unsubscribe<string> (this, "UpdateColorWheel");
			base.Dispose (disposing);
		}

		private void SetColor ()
		{
			if (Control != null) {
				var colorOn = TalentPlus.Shared.Helpers.Color.Primary.ToAndroidColor ();
				var colorOff = TalentPlus.Shared.Helpers.Color.UniLeverMidGray.ToAndroidColor ();
				//			Android.Graphics.Color colorDisabled = Android.Graphics.Color.Gray;
				//
				var drawable = new StateListDrawable ();
				drawable.AddState (new int[] { Android.Resource.Attribute.StateChecked }, new ColorDrawable (colorOn));
				//			drawable.AddState(new int[] { -Android.Resource.Attribute.StateEnabled }, new ColorDrawable(colorDisabled));
				drawable.AddState (new int[] { }, new ColorDrawable (colorOff));
				//
				Control.ThumbDrawable = drawable;
			}
		}
	}
}
