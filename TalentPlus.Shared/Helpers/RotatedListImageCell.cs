using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class RotatedListImageCell : ViewCell
	{
		public RotatedListImageCell()
		{
			var image = new Image ();
            image.SetBinding (Image.SourceProperty, "UserImage");
			image.Rotation = 90;
			image.HorizontalOptions = LayoutOptions.Center;
			image.VerticalOptions = LayoutOptions.Center;

			var layoutWithImage = new StackLayout();
            layoutWithImage.Children.Add(image);

            this.View = layoutWithImage;
		}
	}
}