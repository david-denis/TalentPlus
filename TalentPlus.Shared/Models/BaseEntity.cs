
namespace TalentPlus.Shared
{
	public interface IBaseEntity
	{
		string Id { get; set; }
	}
	public abstract class BaseEntity : IBaseEntity
	{
		public BaseEntity()
		{
		}

		/// <summary>
		/// Gets or sets the Database ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public string Id { get; set; }


	}
}
