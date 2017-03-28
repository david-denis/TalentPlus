using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Xamarin;

namespace TalentPlus.Shared
{
	public class UsersViewModel : BaseViewModel
	{
		public UsersViewModel()
		{
		}

		public bool IsSearchPage = false;

		private ObservableCollection<TeamMember> teamMembers = new ObservableCollection<TeamMember>();
		public ObservableCollection<TeamMember> TeamMembers
		{
			get { return teamMembers; }
			set { teamMembers = value; OnPropertyChanged("TeamMembers"); }
		}
		public async Task AddTeamMember(string activityId, DateTime alarmTime)
		{
			//Activity activity = this.GetActivity(activityId);

			//if (activity == null)
			//{
			//	activity = await GetActivityFromDb(activityId);
			//}
			
			//if (activity != null)
			//{
			//	var sact = new SelectedActivity(activity, alarmTime);
			//	SelectedActivities.Add(sact);
			//	await TalentDb.SaveOrUpdateItem<SelectedActivity>(sact);
			//	return true;
			//}
			//return false;
		}

		private Command loadItemsCommand;
		/// <summary>
		/// Command to load/refresh items
		/// </summary>
		public Command LoadItemsCommand
		{
			get { return loadItemsCommand ?? (loadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand())); }
		}

		private async Task ExecuteLoadItemsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			//if (IsSearchPage == false)
			//	OnBusyStateChanged(EventArgs.Empty);

			try
			{
				var teamMembers = await TalentDb.GetTeamMembers();

				if (teamMembers.Count != TeamMembers.Count)
				{
					TeamMembers.Clear();

					// Populate team members
					foreach (TeamMember member in teamMembers)
					{
						TeamMembers.Add(member);
					}
				}


				IsBusy = false;

				if (IsSearchPage == false)
					OnBusyStateChanged(EventArgs.Empty);
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				var result = page.DisplayAlert ("Error", "Unable to load team members.", "OK", null);
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "UserViewModel.ExecuteLoadItemsCommand()" },
					{ "What", "Unable to load team members" }
				});
			}
		}

		/// <summary>
		/// Gets a specific feed item for an Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TeamMember GetTeamMember(string id)
		{
			return TeamMembers.FirstOrDefault(i => i.Id == id);
		}

		public async Task<TeamMember> GetTeamMemberFromDb(string id)
		{
			return await TalentDb.GetItem<TeamMember>(id);
		}

		public TeamMember GetTeamMemberFromEmail(string email)
		{
			return TeamMembers.FirstOrDefault(i => i.TeamUser.Email == email);
		}

		protected virtual void OnBusyStateChanged(EventArgs e)
		{
			EventHandler handler = BusyStateChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public ObservableCollection<ActivityArchive> GetCurrentActivitiesWithUser(string userId)
		{
			ObservableCollection<SelectedActivity> selectedActivities = null;
			if (ViewModelLocator.ActivitiesViewModel != null && ViewModelLocator.ActivitiesViewModel.SelectedActivities != null)
				selectedActivities = ViewModelLocator.ActivitiesViewModel.SelectedActivities;

			if (selectedActivities == null)
				return null;

			ObservableCollection<ActivityArchive> currentArchives = new ObservableCollection<ActivityArchive>();

			foreach (SelectedActivity sa in selectedActivities)
			{
				if (sa.InvolvedUserIds != null)
				{
					foreach (string involvedUserId in sa.InvolvedUserIds)
					{
						if (involvedUserId == userId)
						{
							currentArchives.Add(new ActivityArchive { Activity = sa.Activity, ActivityId = sa.ActivityId, Feedback = 0, FinishTime = sa.FinishTime });
						}
					}
				}
			}

			return currentArchives;
		}

		public event EventHandler BusyStateChanged;
	}
}

