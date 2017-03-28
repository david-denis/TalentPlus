using System;
using System.Collections.Generic;

namespace TalentPlus.Shared
{
	public class ActivityArchive : BaseEntity
	{
		public ActivityArchive()
		{ 
			InvolvedUserIds = new List<string>();
		}

		public string ActivityId { get; set; }

		public List<string> InvolvedUserIds { get; set; }

		public float Feedback { get; set; }

		public DateTime FinishTime { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public List<User> InvolvedUsers { get; set; }
	}
}
