using System;
using System.Collections.Generic;
using Xamarin;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class TextPage : BaseView
	{
        public FeedbackPost Post { get; set; }
        public Activity activity { get; set; }

		private ScrollView contentView;
		private double yScrollPosition = 0;

		#region PRIVATE MEMBERS
		Editor UserTextEditor;
		Label HintLabel;

		const int LIMIT_TEXT = 300;
		#endregion

		public TextPage ()
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
			if (LoadingViewFlag) { return; }
            LoadingViewFlag = true;

			if (String.IsNullOrEmpty (UserTextEditor.Text)) {
				await DisplayAlert ("Warning", "Text can't be empty", "OK");
				LoadingViewFlag = false;
				return;
			}

			HideBackButtonFlag = true;
            try{
				ActivitiesView.IsNeedReload = true;
                Post.Description = UserTextEditor.Text;
                await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
				//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();
				//activity.FeedbackPosts.Add(Post);
            }
            catch (Exception ex)
            {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "Textpage.SaveAndNextPage()" }
				});
            }

			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);
            
			LoadingViewFlag = false;

			OnProperlyQuit(EventArgs.Empty);

			await Navigation.PopToRootAsync ();
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
		}

		protected virtual void OnProperlyQuit(EventArgs e)
		{
			EventHandler handler = ProperlyQuit;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler ProperlyQuit;
	}
}

