using System;
using Xamarin.Forms;

namespace TalentPlus.Shared.Helpers
{
	public class VideoPlayerView : View
	{
		public VideoPlayerView ()
		{
		}

		public String VideoURI = "";

		public static readonly BindableProperty StartPlayProperty =
			BindableProperty.Create<VideoPlayerView, bool> (
				p => p.StartPlay, false);

        public Action LoadingCancelEvent { get; set; }

        public bool IsLoadingFailed { get; set; }

		public bool StartPlay {
			get {
				return (bool)GetValue (StartPlayProperty);
			}

			set {
				this.SetValue (StartPlayProperty, value);
			}
		}
	}
}

