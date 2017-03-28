using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;
using TalentPlus.iOS;
using CoreGraphics;
using System.Drawing;
using CoreImage;
using Foundation;

[assembly: ExportRenderer (typeof(DarkIceImage), typeof(DarkIceImageRenderer))]

namespace TalentPlus.iOS
{
	public class DarkIceImageRenderer : ImageRenderer
	{
		bool _disposed;

		UITapGestureRecognizer TappedRecognizer;

		protected DarkIceImage ActiveElement {
			get {
				return this.Element as DarkIceImage;
			}
		}

		public DarkIceImageRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged (e);

			var imageElement = Element as DarkIceImage;

			if (ActiveElement.FilterColor != Xamarin.Forms.Color.Default) {
				LoadImageWithFilter ();
			}

			// Get a reference to the native control
			var nativeImage = (UIImageView)Control; 
			nativeImage.UserInteractionEnabled = true;

			// TODO: Implement animated glow
			//			var glow = new AnimatedGlow.Binding.AnimatedGlow(new RectangleF(0,0,200,200));
			//			this.Add (glow);
			//

			if (imageElement.IsDefaultImage == false) {
				TappedRecognizer = new UITapGestureRecognizer ();
				TappedRecognizer.AddTarget (() => {
					Console.WriteLine ("Dark Ice Image Tapped");

					if (imageElement.TappedWithInfo != null)
						imageElement.TappedWithInfo.Invoke (imageElement.TagInfo);
					else if (imageElement.TappedWithId != null)
						imageElement.TappedWithId.Invoke (imageElement.Tag.ToString());
					else if (imageElement.Tapped != null)
						imageElement.Tapped.Invoke ();
				});

				nativeImage.AddGestureRecognizer (TappedRecognizer);
			}

			if (imageElement.ShouldUseInset && nativeImage.Image != null)
				nativeImage.Image = nativeImage.Image.CreateResizableImage (new UIEdgeInsets (imageElement.TopInset, imageElement.LeftInset, imageElement.BottomInset, imageElement.RightInset), UIImageResizingMode.Stretch);



		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {
			case "Source":
				LoadImageWithFilter ();
				break;

			case "ShouldDisposeImage":
				{
					Dispose (true);
				}
				break;

				#if __IOS__
			case "FilterColor":
				LoadImageWithFilter ();
				break;
				#endif
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

				var nativeImage = (UIImageView)Control; 

				if (disposing) {
					Console.WriteLine ("Disposing Image: {0}", disposing);
					// free other managed objects that implement
					// IDisposable only

					nativeImage.RemoveGestureRecognizer(TappedRecognizer);
					//if (nativeImage != null)
					//	nativeImage.Image.DisposeIfNotNull ();

					Control.Dispose();
					//nativeImage.DisposeIfNotNull ();
				}

				// release any unmanaged objects
				// set the object references to null

				// Brute force, remove everything
				foreach (var view in Subviews)
					view.RemoveFromSuperview ();

				if (nativeImage != null)
					nativeImage.Image = null;


			});

			_disposed = true;
		}

		private void LoadImageWithFilter ()
		{
			if (Control.Image != null) {
				Control.Image = GetColoredImage (Control.Image, this.ActiveElement.FilterColor);
			}
		}
	
		private UIImage GetColoredImage(UIImage image, Xamarin.Forms.Color color)
		{
			UIImage coloredImage = null;
			UIGraphics.BeginImageContext(image.Size);
			using(CGContext context = UIGraphics.GetCurrentContext())
			{

				context.TranslateCTM(0, image.Size.Height);
				context.ScaleCTM(1.0f, -1.0f);

				var rect = new CGRect(0, 0, image.Size.Width, image.Size.Height);

				// draw image, (to get transparancy mask)
				context.SetBlendMode(CGBlendMode.Normal);
				context.DrawImage(rect, image.CGImage);

				// draw the color using the sourcein blend mode so its only draw on the non-transparent pixels
				context.SetBlendMode(CGBlendMode.SourceIn);
				context.SetFillColor(color.ToUIColor().CGColor);
				context.FillRect(rect);

				coloredImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}
			return coloredImage;
		}


		private UIImage GetColoredImage(UIImage image, UIColor color)
		{
			var bwCGImage = image.CGImage;
			var bwCIImage = CIImage.FromCGImage (bwCGImage);

			var filter = CIFilter.FromName ("CIHueAdjust");
			filter.SetValueForKey (NSNumber.FromFloat (0.5f), new NSString("inputAngle"));

			filter.SetValueForKey (bwCIImage, CoreImage.CIFilterInputKey.Image);
			var hueImage = (CIImage)filter.ValueForKey(CoreImage.CIFilterOutputKey.Image);

			var context = CIContext.FromOptions(null);
			var cgImage = context.CreateCGImage (hueImage, hueImage.Extent);
				
			var coloredImage = UIImage.FromImage (cgImage);
			cgImage.Dispose ();

			return coloredImage;
		}
		#endregion
	}
}

