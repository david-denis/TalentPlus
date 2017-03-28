using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
    public class NegativeFeedbackPost : BaseEntity
    {
		public NegativeFeedbackPost() {
			Value = "";
			Time = DateTime.Now;
		}

		public string ActivityId { get; set; }

		public string AnswerId { get; set; }

		public string Value { get; set; }

		public DateTime Time { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }
    }
}
