using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public enum InvitationStatus
	{
		Pending,
		Declined,
		Accepted
	}

	public abstract class Invitation : BaseEntity
	{
		//set by the server
		public string SenderUserId { get; set; }

		public string TargetUserId { get; set; }

		public string ActivityId { get; set; }

		public DateTime ReceiveTime { get; set; }

		public Activity Activity { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public User SenderUser { get; set; }
		[System.Runtime.Serialization.IgnoreDataMember]
		public User TargetUser { get; set; }

		public bool Visible { get; set; }

		public InvitationStatus InvitationStatus { get; set; }

		public string ReadableStatus
		{
			get
			{
				string response = "";
				switch (InvitationStatus)
				{
					case InvitationStatus.Pending:
						response = "Sent request to";
						break;
					case InvitationStatus.Declined:
						response = "Request declined by";
						break;
					case InvitationStatus.Accepted:
						response = "Request accepted by";
						break;
					default:
						break;
				}
				return response;
			}
		}

		public string ReadableDate
		{
			get { return ReceiveTime.ToString(); }
		}
	}

	public class ActivityInvitation : Invitation
    {
		public ActivityInvitation() { }

		public string Subject { get; set; }
		public string Message { get; set; }

		public DateTime FinishTime { get; set; }
    }

	public class FeedbackInvitation : Invitation
	{
		public FeedbackInvitation() { }
	}
}
