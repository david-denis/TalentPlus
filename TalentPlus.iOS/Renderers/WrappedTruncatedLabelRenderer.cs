using Xamarin.Forms;
using UIKit;
using TalentPlus.Shared.Helpers;
using TalentPlus.iOS;
using XLabs.Forms.Controls;
using TalentPlus.Shared;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WrappedTruncatedLabel), typeof(WrappedTruncatedLabelRenderer))]
namespace TalentPlus.iOS
{
	public class WrappedTruncatedLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.LineBreakMode = UILineBreakMode.TailTruncation;
				Control.Lines = 2;
			}
		}
	}
}