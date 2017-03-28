using System;
using TalentPlus.Shared;
using TalentPlus.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using UIKit;

[assembly: ExportRenderer(typeof(TPButton), typeof(TPButtonRenderer))]
namespace TalentPlus.iOS
{
	public class TPButtonRenderer : ButtonRenderer
	{
		public TPButtonRenderer():base()
		{

		}
			
		protected override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged (e);

			var current = this.Control.ContentEdgeInsets;
			var edgeInsets = new UIEdgeInsets(current.Top, 15, current.Bottom, 15);
			this.Control.ContentEdgeInsets = edgeInsets;

		}
	}
}
