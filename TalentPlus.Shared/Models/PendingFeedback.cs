using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public class PendingFeedback : BaseEntity
	{

		public PendingFeedback() { }

		public PendingFeedback(Activity activity, DateTime receiveTime)
		{
			ActivityId = activity.Id;
			ReceiveTime = receiveTime;
			Activity = activity;

			InvolvedUserIds = new List<string>();
			InvolvedUsers = new List<User>();
		}

		public string ActivityId { get; set; }

		public DateTime ReceiveTime { get; set; }

		public List<string> InvolvedUserIds { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public List<User> InvolvedUsers { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }

		public string ReadableDate
		{
			get { return ReceiveTime.ToString(); }
		}
	}
}
