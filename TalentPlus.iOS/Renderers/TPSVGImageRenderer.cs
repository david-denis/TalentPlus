using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using TalentPlus.iOS;
using SVG.Forms.Plugin.Abstractions;
using UIKit;
using System.IO;
using System.Collections.Generic;
using NGraphics;
using NGraphics.Parsers;
using System.Linq;

[assembly: ExportRenderer (typeof(SVG.Forms.Plugin.Abstractions.SvgImage), typeof(TPSVGImageRenderer))]
namespace TalentPlus.iOS
{
	public class TPSVGImageRenderer : ImageRenderer
	{
		private readonly nint VIEW_TAG = 9999;

        private const int BLUE_KEY= -16022072;

		protected Dictionary<int, IImage> _hoverImages;
		protected IImage _bitmap;
		protected UIImage _viewImage;
			
		protected readonly int[] Color_Keys = new int [] {
			-7790494,
			-3138959,
			-2413527,
            BLUE_KEY,
			//-16022072,
			//-13946741,
			-16731255,
			-1275100,
			-9722813,
			-3285975,

		};

		public TPSVGImageRenderer () : base ()
		{


		}

		protected SvgImage _formsControl {
			get {
				return Element as SvgImage;
			}
		}

		private UIImage ImageFromUIImageView (UIImageView imageView)
		{
			UIGraphics.BeginImageContextWithOptions (imageView.Bounds.Size, true, 0.0f);
			imageView.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		private int GetPixelColor (System.Drawing.PointF myPoint, UIImage myImage)
		{
			var rawData = new byte[4];
			var handle = System.Runtime.InteropServices.GCHandle.Alloc (rawData);
			int colorInt = 0;
			try {
				using (var colorSpace = CoreGraphics.CGColorSpace.CreateDeviceRGB ()) {
					using (var context = new CoreGraphics.CGBitmapContext (rawData, 1, 1, 8, 4, colorSpace, CoreGraphics.CGImageAlphaInfo.PremultipliedLast)) {
						context.DrawImage (new System.Drawing.RectangleF (-myPoint.X, myPoint.Y - (float)myImage.Size.Height, (float)myImage.Size.Width, (float)myImage.Size.Height), myImage.CGImage);
//						float red = (rawData [0]) / 255.0f;
//						float green = (rawData [1]) / 255.0f;
//						float blue = (rawData [2]) / 255.0f;
//						float alpha = (rawData [3]) / 255.0f;
						//resultColor = UIColor.FromRGBA (red, green, blue, alpha);

						colorInt = (rawData [3] << 24) + (rawData [0] << 16) + (rawData [1] << 8) + rawData [2];
							
					}
				}
			} finally {
				handle.Free ();
			}
			return colorInt;
		}

		protected virtual void CustomizeImageView (UIImageView imgView)
		{
			UITapGestureRecognizer tapGesture;

			if (_formsControl.TouchCancel != null || _formsControl.TouchIn != null) {
				tapGesture = new UITapGestureRecognizer (new Action<UITapGestureRecognizer> (delegate(UITapGestureRecognizer recognizer) {

					switch (recognizer.State) {

					case UIGestureRecognizerState.Ended:
						
					case UIGestureRecognizerState.Began:
						
						var pixelPoint = recognizer.LocationInView (imgView);
						if (_viewImage == null) {
							_viewImage = ImageFromUIImageView (imgView);
						}

						var color = GetPixelColor (new System.Drawing.PointF ((float)pixelPoint.X, (float)pixelPoint.Y), _viewImage);

						if (Color_Keys.Contains (color)) {
							_formsControl.TouchIn (color);
						}

						break;

					case UIGestureRecognizerState.Cancelled:
						if (this._formsControl.TouchCancel != null) {
							this._formsControl.TouchCancel ();
						}
						break;
					}

				})) {
					NumberOfTapsRequired = 1,
					NumberOfTouchesRequired = 1,
				};

				imgView.Tag = VIEW_TAG;

			} else {
				tapGesture = new UITapGestureRecognizer (new Action<UITapGestureRecognizer> (delegate(UITapGestureRecognizer recognizer) {

					var view = recognizer.View.Superview.Superview.ViewWithTag (VIEW_TAG);

					var pixelPoint = recognizer.LocationInView (view);
					if (_viewImage == null) {
						_viewImage = ImageFromUIImageView (view as UIImageView);
					}

					Console.WriteLine ("Point X:{0}, Point Y: {1}", pixelPoint.X, pixelPoint.Y);

					var color = GetPixelColor (new System.Drawing.PointF ((float)pixelPoint.X, (float)pixelPoint.Y), _viewImage);

					if (Color_Keys.Contains (color)) {
						var renderer = view.Superview as TPSVGImageRenderer;
						((SvgImage)renderer.Element).TouchIn (color);
					}

					Console.WriteLine ("You touch color {0}", color);
				})) {
					NumberOfTapsRequired = 1,
					NumberOfTouchesRequired = 1,
				};

			}

			imgView.AddGestureRecognizer (tapGesture);
			imgView.UserInteractionEnabled = true;
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsVisible") {
				LoadSVGImage ();
			}
			else if(e.PropertyName.Equals("ColorCode"))
			{
				LoadSVGImage ();
			}

			base.OnElementPropertyChanged (sender, e);
		}

		private void LoadSVGImage ()
		{
			var svgImage = this.Element as SvgImage;
			if (_hoverImages.ContainsKey (svgImage.ColorCode)) {
				var image = _hoverImages [svgImage.ColorCode];
				var uiImage = image.GetUIImage ();
				Control.Image = uiImage;
			}
			else {
				Control.Image = null;
			}
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged (e);

			if (_formsControl != null) {
				
				IImage image;
				if (_formsControl.IsHoverImages) {
					BuildHoverImages ();
					var svgImage = Element as SvgImage;
                    if (svgImage.ColorCode != 0 && _hoverImages.ContainsKey(svgImage.ColorCode)) {
						image = _hoverImages [svgImage.ColorCode];
					} else {
                        image = _hoverImages[BLUE_KEY];
					}
					//LoadSVGImage ();
				} else {
					var svgStream = _formsControl.SvgAssembly.GetManifestResourceStream (_formsControl.SvgPath);

					if (svgStream == null) {
						throw new Exception (string.Format ("Error retrieving {0} make sure Build Action is Embedded Resource",
							_formsControl.SvgPath));
					}

					var r = new SvgReader (new StreamReader (svgStream), new StylesParser (new ValuesParser ()), new ValuesParser ());

					var graphics = r.Graphic;

					var width = _formsControl.WidthRequest == 0 ? 100 : _formsControl.WidthRequest;
					var height = _formsControl.HeightRequest == 0 ? 100 : _formsControl.HeightRequest;

					var scale = 1.0;

					if (height >= width) {
						scale = height / graphics.Size.Height;
					} else {
						scale = width / graphics.Size.Width;
					}

					var scaleFactor = UIScreen.MainScreen.Scale;

					var canvas = new ApplePlatform ().CreateImageCanvas (graphics.Size, scale * scaleFactor);
					graphics.Draw (canvas);
					image = canvas.GetImage ();
				}

				var uiImage = image.GetUIImage ();
				Control.Image = uiImage;

				CustomizeImageView (Control);
			}
		}

		private void BuildHoverImages ()
		{
			_hoverImages = new Dictionary<int, IImage> ();
			string svgName = _formsControl.SvgPath;

			for (int i = 0; i < Color_Keys.Length; i++) {
				var image = BuildImage (svgName, i + 1);
				_hoverImages.Add (Color_Keys [i], image);
			}
		}

		private IImage BuildImage (string svgName, int i)
		{
			string imageName = svgName.Replace ("hoverName", string.Format ("piece{0}_hover", i));
			var svgStream = _formsControl.SvgAssembly.GetManifestResourceStream (imageName);

			if (svgStream == null) {
				throw new Exception (string.Format ("Error retrieving {0} make sure Build Action is Embedded Resource",
					_formsControl.SvgPath));
			}

			var r = new SvgReader (new StreamReader (svgStream), new StylesParser (new ValuesParser ()), new ValuesParser ());

			var graphics = r.Graphic;

			var width = _formsControl.WidthRequest == 0 ? 100 : _formsControl.WidthRequest;
			var height = _formsControl.HeightRequest == 0 ? 100 : _formsControl.HeightRequest;

			var scale = 1.0;

			if (height >= width) {
				scale = height / graphics.Size.Height;
			} else {
				scale = width / graphics.Size.Width;
			}

			var scaleFactor = UIScreen.MainScreen.Scale;

			var canvas = new ApplePlatform ().CreateImageCanvas (graphics.Size, scale * scaleFactor);
			graphics.Draw (canvas);
			var image = canvas.GetImage ();

			return image;
		}

	}
}

