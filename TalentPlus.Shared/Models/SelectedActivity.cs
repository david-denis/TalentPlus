using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public class SelectedActivity : BaseEntity
    {
		public SelectedActivity() { }

		public SelectedActivity(Activity activity, DateTime finishTime)
		{
			ActivityId = activity.Id;
			Activity = activity;
			FinishTime = finishTime;

			InvolvedUserIds = new List<string>();
		}

		public string ActivityId { get; set; }

		public DateTime FinishTime { get; set; }

		public List<string> InvolvedUserIds { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public List<User> InvolvedUsers { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }
    }
}
