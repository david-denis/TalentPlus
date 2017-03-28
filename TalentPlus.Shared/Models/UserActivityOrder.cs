using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public class UserActivityOrder : BaseEntity
	{
		public UserActivityOrder() { }

		public string ActivityId { get; set; }
		public int Order { get; set; }
		public bool Hidden { get; set; }
	}
}
