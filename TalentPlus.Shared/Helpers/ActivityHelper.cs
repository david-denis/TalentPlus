using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;

namespace TalentPlus.Shared.Helpers
{
	public static class ActivityHelper
	{
		public static async Task<int> GetActivitiesCompletedInSixMonths()
		{
			var result = await TalentDb.client.GetSyncTable<ActivityArchive>().Take(0).IncludeTotalCount().ToCollectionAsync();
			return (int)result.TotalCount;
		}

		public static async Task<int> GetActivitiesCompletedPercent()
		{
			ICollection<string> archiveResult = await TalentDb.client.GetSyncTable<ActivityArchive>().Select(aa => aa.ActivityId).ToCollectionAsync();

			if (archiveResult == null)
			{
				return 0;
			}
			HashSet<string> unique = new HashSet<string>(archiveResult);

			var activityResult = await TalentDb.client.GetSyncTable<Activity>().Take(0).IncludeTotalCount().ToCollectionAsync();
			int totalActivities = (int)activityResult.TotalCount;
			if (totalActivities == 0)
			{
				return 0;
			}
			return (int)Math.Round((float)unique.Count / totalActivities * 100);
		}

		public static async Task<IList<ActivityArchive>> GetLatestActivityArchiveByUser(string userId)
		{
			IList<ActivityArchive> all = await TalentDb.client.GetSyncTable<ActivityArchive>().ToListAsync();
			var relevant = all.Where(aa => aa.InvolvedUserIds.Contains(userId)).OrderByDescending(aa => aa.FinishTime).ToList();
			foreach (ActivityArchive activityArchive in relevant)
			{
				activityArchive.Activity = await TalentDb.client.GetSyncTable<Activity>().LookupAsync(activityArchive.ActivityId);
				if (activityArchive.InvolvedUsers != null)
				{
					activityArchive.InvolvedUsers = await TalentDb.client.GetSyncTable<User>().Where(u => activityArchive.InvolvedUserIds.Contains(u.Id)).ToListAsync();
				}
			}

		    return relevant;
		}

		public static async Task<int> GetLastInteractionWithActivity(string activityId)
		{
			var result = await TalentDb.client.GetSyncTable<ActivityArchive>().Take(1).Where(aa => aa.ActivityId == activityId).Select(aa => aa.FinishTime).ToListAsync();
			if (result.Count == 0)
			{
				return -1;
			}
			DateTime finishDate = result.FirstOrDefault();
			return DateTime.Now.Subtract(finishDate).Days;
		}

		public static async Task<IList<ActivityArchive>> GetArchivesByTheme(string themeId)
		{
			var result = await TalentDb.client.GetSyncTable<Activity>().Where(a => a.ThemeId == themeId).Select(a => a.Id).ToListAsync();
			return await TalentDb.client.GetSyncTable<ActivityArchive>().Where(aa => result.Contains(aa.ActivityId)).ToListAsync();

		}
	}
}
