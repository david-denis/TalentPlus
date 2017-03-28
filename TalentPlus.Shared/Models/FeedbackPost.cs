using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public enum VideoStatus
	{
		NoVideo,
		Processing,
		Ready
	}

	public class FeedbackPost : BaseEntity
    {
		public FeedbackPost() { }

		public string ActivityId { get; set; }

		public string UserName { get; set; }

		public string UserImage { get; set; }

		public string Description { get; set; }

		public float EaseRating { get; set; }

		public float EffectivenessRating { get; set; }

		public string ImageUrl { get; set; }

		public string VideoUrl { get; set; }

		public bool VIPContent { get; set; }

		public VideoStatus VideoStatus { get; set; }

		public long VideoId { get; set; }

		public DateTime Date { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }
    }
}
