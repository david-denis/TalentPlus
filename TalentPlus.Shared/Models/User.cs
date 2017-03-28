using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public class User : BaseEntity
    {
		public User() { }

		public string Name { get; set; }

		public string Email { get; set; }

		public string UserImage { get; set; }

		public string Role { get; set; }

		public bool AcceptedConditions { get; set; }

		public bool NeedPurging { get; set; }

		public bool Deactivated { get; set; }
    }

	public class UserSettings : BaseEntity
	{
		public UserSettings() { }

		public bool Sound { get; set; }

		public int ColorPrimary { get; set; }

		public int ColorSecondary { get; set; }
	}

	public class UserSuggestion
	{
		public UserSuggestion() { }
		public string Email { get; set; }
		public string Name { get; set; }
		public string UserImage { get; set; }
	}
}
