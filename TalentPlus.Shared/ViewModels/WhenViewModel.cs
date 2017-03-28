using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    public class WhenViewModel:BaseViewModel
    {
		private Activity thisActivity;

		public WhenViewModel(Activity item)
		{
			thisActivity = item;
		}

		public Activity GetActivity()
		{
			return thisActivity;
		}

		public String Subject { get; set; }

		public String Message { get; set; }

		private List<DateTime> whenDates = new List<DateTime>();
		private List<String> whenDatesText = new List<String>();
#if DEBUG
		private List<int> whenDateDelaySeconds = new List<int>() { 30, 86400, 604800, 1209600, 2592000};
#else
		private List<int> whenDateDelaySeconds = new List<int>() { 900, 86400, 604800, 1209600, 2592000 };
#endif

		private Command loadItemsCommand;

		public Command LoadItemsCommand
		{
			get { return loadItemsCommand ?? (loadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand())); }
		}

		private async Task ExecuteLoadItemsCommand()
		{
			try
			{
				//whenDates.Clear();
				//whenDates.Add(DateTime.Today);
				//whenDates.Add(DateTime.Today.AddDays(1));
				//whenDates.Add(DateTime.Today.AddDays(7));
				//whenDates.Add(DateTime.Today.AddMonths(1));

				whenDatesText.Clear();
				whenDatesText.Add("Today");
				whenDatesText.Add("Tomorrow");
				whenDatesText.Add("Within a week");
				whenDatesText.Add("Within two weeks");
                whenDatesText.Add("Within a month");

				/*whenDatesText.Clear();
				whenDatesText.Add("Now");
				whenDatesText.Add("10 Seconds");
				whenDatesText.Add("30 Seconds");
				whenDatesText.Add("1 Minute");*/

			}
			catch(Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert("Error", "Unable to load page.", "OK", null);
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "WhenViewModel.ExecuteLoadItemsCommand()" },
					{ "What", "Unable to load page" }
				});
			}
		}

		public String GetWhenDate(double index)
		{
			int floored_index = (int)Math.Floor(index);

			if (whenDatesText.Count <= 0)
				return "Now";

			if (floored_index < whenDatesText.Count - 1)
				return whenDatesText[floored_index];

			return whenDatesText[whenDatesText.Count - 1];
		}

		public async Task ValidateSlider(double index, string subject = "", string message = "", bool isrestarted = false)
		{
			whenDates.Clear();
#if DEBUG
			whenDates.Add(DateTime.Now.AddSeconds(30)); // For Today
#else
			whenDates.Add(DateTime.Now.AddSeconds(900)); // For Today
#endif
			whenDates.Add(DateTime.Now.AddDays(1));     // For Tomorrow
			whenDates.Add(DateTime.Now.AddDays(7));     // For Next Week
			whenDates.Add(DateTime.Now.AddDays(14));    // For Two Weeks
            whenDates.Add(DateTime.Now.AddMonths(1));    // For One Month

			this.Subject = subject;
			this.Message = message;

			int floored_index = (int)Math.Floor(index);
			if (floored_index < whenDates.Count - 1) {
				if (isrestarted)
					await TalentPlusApp.RootPage.overview.StartActivityWithId(this, whenDateDelaySeconds [floored_index]);
				else
					await TalentPlusApp.RootPage.activities.GetActivityDetailsViewFromActivityId(thisActivity.Id).Start(this, whenDateDelaySeconds[floored_index]);
			} else {
				if (isrestarted)
					await TalentPlusApp.RootPage.overview.StartActivityWithId(this, whenDateDelaySeconds [whenDates.Count - 1]);
				else
					await TalentPlusApp.RootPage.activities.GetActivityDetailsViewFromActivityId(thisActivity.Id).Start(this, whenDateDelaySeconds[whenDates.Count - 1]);
			}
		}
    }
}
