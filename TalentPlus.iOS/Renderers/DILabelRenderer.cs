using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;
using TalentPlus.iOS;
using Foundation;

[assembly: ExportRenderer (typeof(DILabel), typeof(DILabelRenderer))]

namespace TalentPlus.iOS
{
	public class DILabelRenderer : LabelRenderer
	{
		bool _disposed;
		UITapGestureRecognizer TappedRecognizer;

		public DILabelRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);

			var nativeControl = (UILabel)Control; 
			var element = (DILabel)Element;
			if (element.Lines > 0) {
				nativeControl.Lines = element.Lines;
				nativeControl.LineBreakMode = UILineBreakMode.TailTruncation;
			}

			if (element.IsDefaultLabel == false) {
				nativeControl.UserInteractionEnabled = true;

				TappedRecognizer = new UITapGestureRecognizer ();
				TappedRecognizer.AddTarget (() => {
					Console.WriteLine ("Dark Ice Image Tapped");

					// TODO: Implement parental gate

					// Invoke tapped event if it's been set
					if (element.Tapped != null)
						element.Tapped.Invoke ();

					// TODO: Add in event handler for the common code

				});

				nativeControl.AddGestureRecognizer (TappedRecognizer);
			}

			if (element.IsUnderline == true)
				nativeControl.AttributedText = new NSMutableAttributedString (element.Text, underlineStyle: NSUnderlineStyle.Single);

			// Quick Hack: Just need to adjust the line spacing on the story pages so tweaking here.
			// For future versions of the DILabel this should probably be made to be a bit more robust.
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {

			case "ShouldDispose":
				{
					Dispose (true);
				}
				break;
			default:
				//System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}

		}

		#region IDisposable implementation

		protected override void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			// Ensure UI operations invoked on main thread
			InvokeOnMainThread (() => {

				var nativeControl = (UILabel)Control; 

				if (disposing) {
					Console.WriteLine ("Disposing Label: {0}", disposing);
					// free other managed objects that implement
					// IDisposable only

					// Implement if adding tapped to this control
					//					nativeControl.RemoveGestureRecognizer(TappedRecognizer);

					Control.Dispose();
				}

				// release any unmanaged objects
				// set the object references to null

				// Brute force, remove everything
				foreach (var view in Subviews)
					view.RemoveFromSuperview ();

				Control.RemoveFromSuperview();


			});

			_disposed = true;
		}

		#endregion
	}
}

