using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using TalentPlus.Shared.Helpers;
using TalentPlusAndroid;

[assembly: ExportRenderer (typeof(DarkIceImage), typeof(DarkIceImageRenderer))]
namespace TalentPlusAndroid
{
	public class DarkIceImageRenderer : ImageRenderer
	{
		bool _disposed;
		static long lastEventTickTime = 0;


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

			if (e.NewElement != null) {
				var darkIceImage = ActiveElement;
				Control.SetColorFilter (ActiveElement.FilterColor.ToAndroid ());

				// Get a reference to the native control
				var nativeImage = Control;
				nativeImage.Clickable = !darkIceImage.IsDefaultImage;

				if (ActiveElement.IsDefaultImage == false) {
					Control.Click += delegate {
						long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
						if (Math.Abs (curTickTime - lastEventTickTime) < 3)
							return;
						lastEventTickTime = curTickTime;

						if (darkIceImage.TappedWithInfo != null)
							darkIceImage.TappedWithInfo.Invoke (darkIceImage.TagInfo);
						else if (darkIceImage.TappedWithId != null)
							darkIceImage.TappedWithId.Invoke (darkIceImage.Tag.ToString ());
						else if (darkIceImage.Tapped != null)
							darkIceImage.Tapped.Invoke ();
					};

					nativeImage.LongClick += (object sender, LongClickEventArgs e1) => {
						long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
						if (Math.Abs (curTickTime - lastEventTickTime) < 3)
							return;
						lastEventTickTime = curTickTime;

						if (darkIceImage.LongTappedWithInfo != null)
							darkIceImage.LongTappedWithInfo.Invoke (darkIceImage.TagInfo);
					};
				}

				/*TappedRecognizer = new UITapGestureRecognizer();
            TappedRecognizer.AddTarget(() =>
            {
                Console.WriteLine("Dark Ice Image Tapped");

                // TODO: Implement parental gate

                // Invoke tapped event if it's been set
                if (imageElement.Tapped != null)
                    imageElement.Tapped.Invoke();

                // TODO: Add in event handler for the common code

            });


            nativeImage.AddGestureRecognizer(TappedRecognizer);*/


				//			nativeImage.DisposeIfNotNull ();


//            MessagingCenter.Subscribe<string>(string.Empty, "ReloadActionBar", (message) =>
//                {
//                    this.PostInvalidate();
//                });
				//Console.WriteLine("OnElementChanged");
			}
		}

		private void TouchMeImageViewOnTouch (object sender, Android.Views.View.TouchEventArgs touchEventArgs)
		{
			string message;
			switch (touchEventArgs.Event.Action) {// & MotionEventArgs.Mask)
			case Android.Views.MotionEventActions.Down:
			case Android.Views.MotionEventActions.Move:
				message = "Touch Begins";
				break;

			case Android.Views.MotionEventActions.Up:
				message = "Touch Ends";
				if (ActiveElement.Tapped != null)
					ActiveElement.Tapped.Invoke ();
				break;

			default:
				message = string.Empty;
				break;
			}

			Console.WriteLine (message);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);


			switch (e.PropertyName) {
			case "FilterColor":
				//Control.ClearColorFilter ();
				Control.SetColorFilter (ActiveElement.FilterColor.ToAndroid ());
				break;

			case "ShouldDisposeImage":
				{
					Dispose (true);
				}
				break;
			case "WidthRequestImage":
				{
					//var darkIceImage = Element as DarkIceImage;
				}
				break;
			default:
                //System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}

		}

		#region IDisposable implementation

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~DarkIceImageRenderer ()
		{
			//MessagingCenter.Unsubscribe<string> (string.Empty, "ReloadActionBar");
			Dispose (false);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (_disposed)
				return;

			if (disposing && Control != null) {
				Console.WriteLine ("Disposing Image: {0}", disposing);
				// free other managed objects that implement
				// IDisposable only


				var nativeImage = Control;

				// TODO: Call recycle here on bitmap to clear memory properly
				nativeImage.Dispose ();
				//nativeImage.RecycleBitmap();
				//nativeImage.DisposeIfNotNull();
				nativeImage = null;
			}

			// release any unmanaged objects
			// set the object references to null

			_disposed = true;
		}

		#endregion

		// TODO: Implement this for all custom image view controls

	}
}