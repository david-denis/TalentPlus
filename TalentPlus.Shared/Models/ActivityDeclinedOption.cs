
namespace TalentPlus.Shared
{
    public class ActivityDeclinedOption : BaseEntity
    {
		public ActivityDeclinedOption() { }

		public string ActivityId { get; set; }

		public string Description { get; set; }

		public ActivityDelcinedOptionType Type { get; set; }

		public int Order { get; set; }

		[System.Runtime.Serialization.IgnoreDataMember]
		public Activity Activity { get; set; }
    }

	public enum ActivityDelcinedOptionType
	{
		Text,
		Open
	}
}
