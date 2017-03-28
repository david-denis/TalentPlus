using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using TalentPlus.iOS;
using Xamarin.Forms;
using TalentPlus;
using MediaPlayer;
using Foundation;
using System.Drawing;
using System.ComponentModel;
using TalentPlus.Shared.Helpers;

[assembly: ExportRenderer(typeof(VideoPlayerView), typeof(VideoPlayerViewRenderer))]
namespace TalentPlus.iOS
{
    public class VideoPlayerViewRenderer : ViewRenderer
    {
        //private bool _disposed;

        //private UITapGestureRecognizer TappedRecognizer;

        private MPMoviePlayerController _moviePlayer;
        //UIView rootView;

        public VideoPlayerViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var rootView = new UIView();
                rootView.Frame = new RectangleF(0, 0, (float)NativeView.Frame.Width, (float)NativeView.Frame.Height);
                SetNativeControl(rootView);
            }
            else
            {

                if (_moviePlayer != null)
                {
                    _moviePlayer.Dispose();
                    _moviePlayer = null;
                }
            }


        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {

                case "StartPlay":
                    try
                    {
                        VideoPlayerView MainView = Element as VideoPlayerView;
                        if (_moviePlayer == null)
                        {
                            _moviePlayer = new MPMoviePlayerController(NSUrl.FromString(MainView.VideoURI));
                            NativeView.Add(_moviePlayer.View);
                        }

                        if (_moviePlayer != null)
                        {
                            _moviePlayer.Stop();
                            _moviePlayer.ContentUrl = NSUrl.FromString(MainView.VideoURI);
                            _moviePlayer.SetFullscreen(true, true);
                            _moviePlayer.Play();
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("There was a problem playing back Video");
                    }
					
                    break;
                default:
					//System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
                    break;
            }

        }
    }
}

