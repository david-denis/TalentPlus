using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class SplashView : BaseView
	{
		public SplashView ()
		{
			Content = new Image {
				Source = ImageSource.FromFile("splash_background.png"),
				Aspect = Aspect.Fill,
			};
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			//LoadingViewFlag = true;
			bool result = await TalentPlusApp.LoginAndSync();
			//LoadingViewFlag = false;

			if (result) {
				if (!TalentPlusApp.CurrentUser.AcceptedConditions) {
					var disclaimerAccepted = new TaskCompletionSource<object> ();
					TalentPlusApp.TalentApp.SetDisclaimerPage (disclaimerAccepted);
					await disclaimerAccepted.Task;
					TalentPlusApp.TalentApp.SetIntroPage ();
					return;
				}

				TalentPlusApp.TalentApp.SetMainPage ();
			}
		}

		public void AnimateLoading()
		{
			LoadingViewFlag = true;
		}

		public void StopAnimateLoading()
		{
			LoadingViewFlag = false;
		}
	}
}

