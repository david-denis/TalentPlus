using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;

using Xamarin.Forms;
using TalentPlus.Shared.Helpers;
using TalentPlusAndroid;
using XLabs.Forms.Extensions;

#if __ANDROID__
using Xamarin.Forms.Platform.Android;

#elif __IOS__
using Xamarin.Forms.Platform.iOS;

#elif WINDOWS_PHONE
using Xamarin.Forms.Platform.WinPhone;
#endif

[assembly: ExportRenderer (typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace TalentPlusAndroid
{

	//using XLabs.Forms.Extensions;

	/// <summary>
	/// Draws a button on the Android platform with the image shown in the right 
	/// position with the right size.
	/// </summary>
	public partial class ImageButtonRenderer : ButtonRenderer
	{
		/// <summary>
		/// Gets the underlying control typed as an <see cref="ImageButton"/>.
		/// </summary>
		private ImageButton ImageButton {
			get { return (ImageButton)Element; }
		}

		/// <summary>
		/// Sets up the button including the image. 
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected async override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged (e);
			
			if (e.NewElement != null) {
				var targetButton = this.Control;
				if (targetButton != null) {
					targetButton.SetOnTouchListener (TouchListener.Instance.Value);
				}

				if (this.Element != null && this.Element.Font != Font.Default && targetButton != null) {
					targetButton.Typeface = Element.Font.ToExtendedTypeface (Context);
				}

				if (this.Element != null && this.ImageButton.Source != null) {
					await this.SetImageSourceAsync (targetButton, this.ImageButton);
				}
			}
		}

		/// <summary>
		/// Sets the image source.
		/// </summary>
		/// <param name="targetButton">The target button.</param>
		/// <param name="model">The model.</param>
		/// <returns>A <see cref="Task"/> for the awaited operation.</returns>
		private async Task SetImageSourceAsync (Android.Widget.Button targetButton, ImageButton model)
		{
			const int Padding = 10;
			var source = model.Source;

			using (var bitmap = await this.GetBitmapAsync (source)) {
				if (bitmap != null) {
					Drawable drawable = new BitmapDrawable (bitmap);
					var scaledDrawable = GetScaleDrawable (drawable, GetWidth (model.ImageWidthRequest),
						                     GetHeight (model.ImageHeightRequest));

					Drawable left = null;
					Drawable right = null;
					Drawable top = null;
					Drawable bottom = null;
					targetButton.CompoundDrawablePadding = Padding;
					switch (model.Orientation) {
					case ImageOrientation.ImageToLeft:
						targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
						left = scaledDrawable;
						targetButton.SetCompoundDrawables (left, top, right, bottom);
						break;
					case ImageOrientation.ImageToRight:
						targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
						right = scaledDrawable;
						targetButton.SetCompoundDrawables (left, top, right, bottom);
						break;
					case ImageOrientation.ImageOnTop:
						top = scaledDrawable;
						targetButton.SetCompoundDrawables (left, top, right, bottom);
						break;
					case ImageOrientation.ImageOnBottom:
						bottom = scaledDrawable;
						targetButton.SetCompoundDrawables (left, top, right, bottom);
						break;
					case ImageOrientation.ImageOnCenter:
						targetButton.Gravity = GravityFlags.Center;
						targetButton.SetBackgroundDrawable (scaledDrawable);
						break;
					}


				}
			}
		}

		/// <summary>
		/// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
		/// </summary>
		/// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
		/// <returns>A loaded <see cref="Bitmap"/>.</returns>
		private async Task<Bitmap> GetBitmapAsync (ImageSource source)
		{
			var handler = GetHandler (source);
			var returnValue = (Bitmap)null;

			returnValue = await handler.LoadImageAsync (source, this.Context);

			return returnValue;
		}

		/// <summary>
		/// Called when the underlying model's properties are changed.
		/// </summary>
		/// <param name="sender">The Model used.</param>
		/// <param name="e">The event arguments.</param>
		protected async override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == ImageButton.SourceProperty.PropertyName) {
				var targetButton = Control;
				await SetImageSourceAsync (targetButton, this.ImageButton);
			}
		}

		/// <summary>
		/// Returns a <see cref="Drawable"/> with the correct dimensions from an 
		/// Android resource id.
		/// </summary>
		/// <param name="drawable">An android <see cref="Drawable"/>.</param>
		/// <param name="width">The width to scale to.</param>
		/// <param name="height">The height to scale to.</param>
		/// <returns>A scaled <see cref="Drawable"/>.</returns>
		private Drawable GetScaleDrawable (Drawable drawable, int width, int height)
		{
			var returnValue = new ScaleDrawable (drawable, 0, RequestToPixels (width), RequestToPixels (height)).Drawable;
			returnValue.SetBounds (0, 0, width, height);
			return returnValue;
		}

		/// <summary>
		/// Returns a drawable dimension modified according to the current display DPI.
		/// </summary>
		/// <param name="sizeRequest">The requested size in relative units.</param>
		/// <returns>Size in pixels.</returns>
		public int RequestToPixels (int sizeRequest)
		{
			return (int)(sizeRequest * Resources.DisplayMetrics.Density);
		}

		//Hot fix for the layout positioning issue on Android as described in http://forums.xamarin.com/discussion/20608/fix-for-button-layout-bug-on-android
		private class TouchListener : Java.Lang.Object, IOnTouchListener
		{
			/// <summary>
			/// Make TouchListener a singleton.
			/// </summary>
			private TouchListener ()
			{

			}

			public static readonly Lazy<TouchListener> Instance =
				new Lazy<TouchListener> (() => new TouchListener ());

			public bool OnTouch (Android.Views.View v, MotionEvent e)
			{
				var buttonRenderer = v.Tag as ButtonRenderer;
				if (buttonRenderer != null && e.Action == MotionEventActions.Down) {
					buttonRenderer.Control.Text = buttonRenderer.Element.Text;
				}

				return false;
			}
		}

		private int GetWidth (int requestedWidth)
		{
			const int DefaultWidth = 50;
			return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
		}

		/// <summary>
		/// Gets the height based on the requested height, if request less than 0, returns 50.
		/// </summary>
		/// <param name="requestedHeight">The requested height.</param>
		/// <returns>The height to use.</returns>
		private int GetHeight (int requestedHeight)
		{
			const int DefaultHeight = 50;
			return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
		}

		/// <summary>
		/// Returns the proper <see cref="IImageSourceHandler"/> based on the type of <see cref="ImageSource"/> provided.
		/// </summary>
		/// <param name="source">The <see cref="ImageSource"/> to get the handler for.</param>
		/// <returns>The needed handler.</returns>
		private static IImageSourceHandler GetHandler (ImageSource source)
		{
			IImageSourceHandler returnValue = null;
			if (source is UriImageSource) {
				returnValue = new ImageLoaderSourceHandler ();
			} else if (source is FileImageSource) {
				returnValue = new FileImageSourceHandler ();
			} else if (source is StreamImageSource) {
				returnValue = new StreamImagesourceHandler ();
			}
			return returnValue;
		}

	}
}

