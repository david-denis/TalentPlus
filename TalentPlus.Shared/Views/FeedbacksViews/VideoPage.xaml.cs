using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using TalentPlus.Shared.Helpers;
using XLabs.Forms.Mvvm;
using Xamarin;


#if __ANDROID__
using Android.App;
#endif

namespace TalentPlus.Shared
{
	public partial class VideoPage : ContentPage
	{
        public FeedbackPost Post { get; set; }
        public Activity activity { get; set; }

		public VideoPage()
		{
			InitializeComponent ();

			ToolbarItems.Add(new ToolbarItem("Submit", "", () =>
				{
					//Utility.ForceHideBackButton ();
                    SubmitVideoFeedback();
				}));

			var VideoInfoLabel = new DILabel {
				TextColor = Xamarin.Forms.Color.Black,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};
			VideoInfoLabel.SetBinding (DILabel.TextProperty, "VideoInfo");
			AbsoluteLayout.SetLayoutFlags (VideoInfoLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (VideoInfoLabel, new Rectangle (0.5, 0.2, 0.8, 0.1));
			MainLayout.Children.Add (VideoInfoLabel);

			ImageButton videoButton = new ImageButton {
				Source = ImageSource.FromFile("icon_video.png"),
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				Orientation = ImageOrientation.ImageOnCenter,
				HeightRequest = 64,
				WidthRequest = 64
			};

			videoButton.Clicked += async (object sender, EventArgs e) => {
				var result = await DisplayActionSheet("Choose source", "Cancel", null, new string[2]{"Camera", "Video Library"});
				if(result == null || result.Equals("Cancel")) {
					return;
				}
				else if (result.Equals("Camera"))
				{
					var viewModel = BindingContext as VideoViewModel;
					viewModel.TakeVideoCommand.Execute(null);
				}
				else{
					var viewModel = BindingContext as VideoViewModel;
					viewModel.SelectVideoCommand.Execute(null);
				}
			};

			AbsoluteLayout.SetLayoutFlags (videoButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (videoButton, new Rectangle (0.5, 0.9, 0.2, 0.1));
			MainLayout.Children.Add (videoButton);

		}

        private async void SubmitVideoFeedback()
        {
            bool IsSuccess = false;
            ShowLoading();

            try
            {
				ActivitiesView.IsNeedReload = true;
				var thing = BindingContext as VideoViewModel;
				Post.VideoUrl = "";
				Post.VideoId = await Helpers.Utility.UploadVideo(thing.ImageBytes);
				Post.VideoStatus = VideoStatus.Processing;

                //Post.VideoUrl = (BindingContext as VideoViewModel).VideoInfo;
                await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "VideoPage.SubmitVideoFeedback()" },
				});
            }

            HideLoading();

			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);

			if (IsSuccess) {
				await Navigation.PopToRootAsync ();

			}
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
        }

#if __ANDROID__
        ProgressDialog p = null;
#endif
        private void ShowLoading()
        {
#if __ANDROID__
			p = new ProgressDialog(Forms.Context);
			p.SetMessage("Loading...");
			p.SetCancelable(false);
			p.Show();
#endif
        }

        private void HideLoading()
        {
#if __ANDROID__
			if (p != null)
			{
			p.Dismiss();
			p = null;
			}
#endif
        }
	}
}
