using System;
using System.Threading.Tasks;
using TalentPlus.Shared.Helpers;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class Easiness : FeedbackViewContent
	{
		#region PRIVATE MEMBERS
		RatingBarControl RatingControl { get; set; }
		#endregion

		public Easiness(Activity activity)
		{
			VerticalOptions = LayoutOptions.FillAndExpand;

			UnileverLabel question = new UnileverLabel
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.2,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				XAlign = TextAlignment.Center,
			};

			var fs = new FormattedString();
			fs.Spans.Add(new Span
			{
				Text = "Did you find the \"",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = activity.ShortDescription,
				ForegroundColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = "\" activity easy to do?",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});

			question.FormattedText = fs;

			RatingControl = new RatingBarControl(50, true, 0, 10, true)
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			RatingControl.StarRated += async (s, e) =>
			{
				await Task.Delay(1000);
				validateButton_Clicked(null, null);
				RatingControl.IsChangeable = true;
			};

			var stackLayout = new StackLayout
			{
				//BackgroundColor = Xamarin.Forms.Color.Fuschia,
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(10, 0),
			};

			var hintImage = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				Source = "icon_info_gray.png",
				Scale = 0.7
			};

			var hintText = new UnileverLabel
			{
				Text = "Tap a star to rate this activity",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center
			};

			stackLayout.Children.Add(new StackLayout { Padding = new Thickness(0, 10), Children = { question } });
			stackLayout.Children.Add(RatingControl);
			stackLayout.Children.Add(new StackLayout { Padding = new Thickness(20, 30, 20, 10), Spacing = 0, VerticalOptions = LayoutOptions.StartAndExpand, Children = { hintImage, hintText } });

			Children.Add(stackLayout);
		}

		void validateButton_Clicked(object sender, EventArgs e)
		{
			this.OnValidatedFeedback(EventArgs.Empty, RatingControl.Rating);
		}
	}
}
