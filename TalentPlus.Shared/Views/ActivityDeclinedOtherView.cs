using System;
using System.Collections.Generic;
using System.Text;
using Xamarin;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    class ActivityDeclinedOtherView : BaseView
    {
		public ActivityDeclinedOption Answer { get; set; }

		 public FeedbackPost Post { get; set; }
        public Activity activity { get; set; }
		public ActivityNegativeFeedbackView NegativeView { get; set; }

		private ScrollView contentView;
		private double yScrollPosition = 0;

		#region PRIVATE MEMBERS
		protected Editor UserTextEditor;
		Label HintLabel;

		const int LIMIT_TEXT = 300;
		#endregion

		public ActivityDeclinedOtherView()
		{
			Title = "Type Text";

			BuildUI ();
		}

		private void BuildUI()
		{
			ToolbarItems.Add(new ToolbarItem("Submit", "", SaveAndNextPage));

			HintLabel = new Label {
				Text = LIMIT_TEXT.ToString() + " left",
				TextColor = Color.Black,
			};

			UserTextEditor = new Editor{
				BackgroundColor = Color.FromRgb(240, 240, 240),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			UserTextEditor.TextChanged += (object sender, TextChangedEventArgs e) => {
				string text = UserTextEditor.Text;
				if (String.IsNullOrEmpty(text) == false)
				{
					int left = LIMIT_TEXT - text.Length;
					if (left < 0)
					{
						left = 0;
						text = text.Substring(0, LIMIT_TEXT);
						UserTextEditor.Text = text;
					}

					HintLabel.Text = left.ToString() + " left";
				}
			};

			UserTextEditor.Focused += (object sender, FocusEventArgs e) => {
				contentView.ScrollToAsync(0, yScrollPosition, false);
			};

			var MainLayout = new StackLayout {
				Padding = 10,
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				Children = {
					HintLabel,
					UserTextEditor,
				}
			};

			contentView = new ScrollView{ Content = MainLayout };
			contentView.Scrolled += (object sender, ScrolledEventArgs e) => {
				yScrollPosition = e.ScrollY;
			};

			Content = contentView;
		}

		private async void SaveAndNextPage()
		{
			if (LoadingViewFlag)
			{
				return;
			}
			LoadingViewFlag = true;
			if (String.IsNullOrEmpty(UserTextEditor.Text))
			{
				await DisplayAlert("Warning", "Text can't be empty", "OK");
				LoadingViewFlag = false;
				return;
			}

			HideBackButtonFlag = true;
			try
			{
				await TalentDb.SaveOrUpdateItem<NegativeFeedbackPost>(new NegativeFeedbackPost() { 
					Activity = this.Answer.Activity,
					ActivityId = this.Answer.ActivityId,
					AnswerId = this.Answer.Id,
					Value = UserTextEditor.Text,
					Time = DateTime.Now
				});

				//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();
			}
			catch (Exception ex)
			{
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "ActivityDeclinedOtherView.SaveAndNextPage()" }
				});
			}
			//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();

			await NegativeView.RemoveActivityAfterClick(Answer);
			await Navigation.PopToRootAsync();
			LoadingViewFlag = false;
		}
    }
}
