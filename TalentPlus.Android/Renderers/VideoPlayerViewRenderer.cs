using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading;
using Android.Widget;
using TalentPlus.Shared.Helpers;
using TalentPlusAndroid;

[assembly: ExportRenderer(typeof(VideoPlayerView), typeof(ViedeoPlayerViewRenderer))]
namespace TalentPlusAndroid
{
	public class ViedeoPlayerViewRenderer : ViewRenderer//, global::Android.Media.MediaPlayer.IOnPreparedListener, ISurfaceHolderCallback
    {
        Android.App.ProgressDialog p = null;

        global::Android.Media.MediaPlayer mediaPlayer;
		VideoView videoPlay;
        MediaController media_Controller;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			base.OnElementChanged(e);

			videoPlay = new VideoView (Context);
            FrameLayout parentLayout = new FrameLayout(Context);
            parentLayout.SetBackgroundColor(Android.Graphics.Color.Black);
            FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(
            LayoutParams.MatchParent, LayoutParams.MatchParent);
            parentLayout.LayoutParameters = lp;

            FrameLayout.LayoutParams param = new FrameLayout.LayoutParams(lp);//LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            param.Gravity = GravityFlags.Center;

			videoPlay.LayoutParameters = param;
            parentLayout.AddView(videoPlay);

            SetNativeControl(parentLayout);

            VideoPlayerView control = Element as VideoPlayerView;

            if (String.IsNullOrEmpty(control.VideoURI) == false)
            {
                //ISurfaceHolder holder = videoPlay.Holder;
                //holder.SetType(SurfaceType.PushBuffers);
                //holder.AddCallback(this);
                
                media_Controller = new MediaController(Context);
                videoPlay.SetMediaController(media_Controller);
                videoPlay.SetVideoPath(control.VideoURI);
                videoPlay.Error += videoPlay_Error;
                videoPlay.Completion += videoPlay_Completion;
                videoPlay.Prepared += videoPlay_Prepared;
                /*
                mediaPlayer = new global::Android.Media.MediaPlayer();
                mediaPlayer.SetDataSource(control.VideoURI);
                mediaPlayer.PrepareAsync();
                mediaPlayer.Prepared += mediaPlayer_Prepared;
                mediaPlayer.Looping = true;                
                */
                p = new Android.App.ProgressDialog(Context);
                p.SetMessage("Loading Video...");
                p.SetCancelable(true);
                p.CancelEvent += p_CancelEvent;
                p.Show();
            }
		}

        void videoPlay_Error(object sender, Android.Media.MediaPlayer.ErrorEventArgs e)
        {
            if (p != null)
            {
                p.Dismiss();
                p = null;
            }

            VideoPlayerView control = Element as VideoPlayerView;
            if (control != null && control.LoadingCancelEvent != null)
            {
                control.IsLoadingFailed = true;
                control.LoadingCancelEvent.Invoke();
            }
        }

        void videoPlay_Completion(object sender, EventArgs e)
        {
            //Console.WriteLine(e.ToString());
        }

        void videoPlay_Prepared(object sender, EventArgs e)
        {
            if (p != null)
            {
                p.Dismiss();
                p = null;
            }

            videoPlay.Start();
        }

        void p_CancelEvent(object sender, EventArgs e)
        {            
            VideoPlayerView control = Element as VideoPlayerView;

            if (control != null && control.LoadingCancelEvent != null)
                control.LoadingCancelEvent.Invoke();
        }
        /*
        void mediaPlayer_Prepared(object sender, EventArgs e)
        {
            if (p != null)
            {
                p.Dismiss();
                p = null;

                mediaPlayer.Start();
            }
        }
        */
//	    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//		{
//			base.OnElementPropertyChanged(sender, e);
//
//			if (e.PropertyName.Equals ("StartPlay")) {
//				
//			}
//		}
        /*
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Console.WriteLine("SurfaceCreated");
            //mediaPlayer.SetDisplay(holder);
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Console.WriteLine("SurfaceDestroyed");
        }
        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int w, int h)
        {
            Console.WriteLine("SurfaceChanged");
        }
        public void OnPrepared(global::Android.Media.MediaPlayer player)
        {

        }*/
    }
}
