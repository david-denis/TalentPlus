
namespace TalentPlus.Shared
{
	public class ActivityStatistics : BaseEntity
    {

		public ActivityStatistics() { }

		public string ActivityId { get; set; }

		public int FeedbackCount { get; set; }

		public float EaseAverage { get; set; }

		public float EffectivenessAverage { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }
		
		// don't forget to reset total count on sync
		//private static int _totalFeedbackCount = -1;
		//[System.Xml.Serialization.XmlIgnore]
		//public static int TotalFeedbackCount {
		//	get {
		//		if (_totalFeedbackCount < 0)
		//		{
					
		//			TotalFeedbackCount = Task.Run(() => TalentDb.GetTotalFeedbackCount()).Result;
		//		} 
		//		return _totalFeedbackCount;
		//	}
		//	set { _totalFeedbackCount = value; }
		//}
    }
}
