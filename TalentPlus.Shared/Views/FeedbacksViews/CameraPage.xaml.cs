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
	public partial class CameraPage : ContentPage
	{
        public FeedbackPost Post { get; set; }
        public Activity activity { get; set; }
		public CameraPage()
		{
			InitializeComponent ();

			ToolbarItems.Add(new ToolbarItem("Submit", "", () =>
				{
					//Utility.ForceHideBackButton ();
                    SubmitPictureFeedback();
				}));

			ImageButton cameraButton = new ImageButton {
				Source = ImageSource.FromFile("icon_camera.png"),
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				Orientation = ImageOrientation.ImageOnCenter,
				HeightRequest = 64,
				WidthRequest = 64
			};

			cameraButton.Clicked += async (object sender, EventArgs e) => {
				var result = await DisplayActionSheet("Choose source", "Cancel", null, new string[2]{"Camera", "Photo Library"});
				if(result == null || result.Equals("Cancel")){
					return;
				}
				else if (result.Equals("Camera"))
				{
					var viewModel = BindingContext as CameraViewModel;
					viewModel.TakePictureCommand.Execute(null);
				}
				else{
					var viewModel = BindingContext as CameraViewModel;
					viewModel.SelectPictureCommand.Execute(null);
				}
			};

			AbsoluteLayout.SetLayoutFlags (cameraButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (cameraButton, new Rectangle (0.5, 0.9, 0.2, 0.1));
			MainLayout.Children.Add (cameraButton);

		}

        private async void SubmitPictureFeedback()
        {
            bool IsSuccess = false;
            ShowLoading();

            try
            {		 
				ActivitiesView.IsNeedReload = true;
                //Post.ImageUrl = (BindingContext as CameraViewModel).ImagePath;
				Post.ImageUrl = await Helpers.Utility.UploadPhoto((BindingContext as CameraViewModel).ImageBytes);
                
				await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "CameraPage.SubmitPictureFeedback()" }
				});
            }

            HideLoading();

			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);

			if (IsSuccess) {
				await Navigation.PopToRootAsync ();
				//TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
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
