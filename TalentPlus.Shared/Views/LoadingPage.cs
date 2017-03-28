using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class LoadingPage : BaseView
	{
		public LoadingPage()
		{
			Content = new Image {
				Source = ImageSource.FromFile("splash_background.png"),
				Aspect = Aspect.Fill,
			};
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

