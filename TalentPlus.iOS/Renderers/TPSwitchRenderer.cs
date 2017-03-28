using System;
using TalentPlus.Shared;
using TalentPlus.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(TPSwitch), typeof(TPSwitchRenderer))]
namespace TalentPlus.iOS
{
	public class TPSwitchRenderer : SwitchRenderer
	{
		public TPSwitchRenderer():base()
		{

		}

		protected override void OnElementChanged (ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				MessagingCenter.Subscribe<string> (this, "ReloadActionBar", (message) => {
					SetColor ();
				});
				SetColor ();
			} else if (e.NewElement == null) {
				MessagingCenter.Unsubscribe<string> (this, "ReloadActionBar");
			}
		}

		private void SetColor()
		{
			if (Control != null) {
				Control.OnTintColor = TalentPlus.Shared.Helpers.Color.Primary.ToUIColor ();
			}
		}

	}
}
