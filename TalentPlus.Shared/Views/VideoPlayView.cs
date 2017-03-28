using System;
using System.Collections.Generic;
using System.Text;
using TalentPlus.Shared.Helpers;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    public class VideoPlayView : BaseView
    {
        public VideoPlayView(String videoUri)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            BackgroundColor = Xamarin.Forms.Color.White;

            var videoPlayer = new VideoPlayerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VideoURI = videoUri,
            };

            videoPlayer.StartPlay = true;

            videoPlayer.LoadingCancelEvent += () =>
            {
                if (videoPlayer.IsLoadingFailed == true)
                {
                    DisplayAlert("Error", "There was an error while loading video", "OK");
                }

                Navigation.PopAsync();
            };

            Content = videoPlayer;
        }
    }
}
