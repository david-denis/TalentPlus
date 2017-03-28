using System;

using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class FeedbackViewContent : StackLayout
	{
		public FeedbackViewContent()
		{
		}

		protected virtual void OnValidatedFeedback(EventArgs e, int rating)
		{
			EventHandler handler = ValidatedFeedback;
			if (handler != null)
			{
                Rating = rating;
				handler(this, e);
			}
		}

		public event EventHandler ValidatedFeedback;
        public int Rating { get; set; }
	}
}
