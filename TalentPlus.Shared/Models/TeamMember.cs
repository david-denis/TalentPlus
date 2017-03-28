using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public class TeamMember : BaseEntity
	{
		public TeamMember() { }

		public string TeamUserId { get; set; }

		public DateTime? LastInteraction { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public User TeamUser { get; set; }
	}

	public enum AddUserToTeamResult
	{
		Added,
		AlreadyInTeam,
		SentEmail,
		InvalidEmail,
		EmailSendingError,
		AddingSelf
	}
}
