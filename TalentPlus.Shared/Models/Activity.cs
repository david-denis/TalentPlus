using System;
using System.Collections.Generic;

namespace TalentPlus.Shared
{

	public class Activity : BaseEntity
	{
		public int AlarmId { get; set; }

		public string ThemeId { get; set; }

		public string ShortDescription { get; set; }

		public string FullDescription { get; set; }

		public int RequiredTime { get; set; }

		public int RequiredPeople { get; set; }

		public string FeedbackQuestion { get; set; }

		public string TopTip { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual ActivityTheme Theme { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual UserActivityOrder Order { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual IList<User> SelectedUsers { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual DateTime SelectedTime { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public ActivityStatistics ActivityStatistics { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual IList<ActivityDeclinedOption> ActivityDeclinedOptions { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public virtual IList<FeedbackPost> FeedbackPosts { get; set; }
	}
}
