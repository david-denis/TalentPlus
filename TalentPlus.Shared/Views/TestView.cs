using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;

namespace TalentPlus.Shared
{
	public class TestView : BaseView
	{
		public TestView ()
		{
			Content =  new DarkIceImage
			{
				Source = ImageSource.FromFile("activities_white.png"),
				WidthRequest = 50,
				HeightRequest = 50,
				FilterColor = Xamarin.Forms.Color.Red,
			};
		}
	}
}

