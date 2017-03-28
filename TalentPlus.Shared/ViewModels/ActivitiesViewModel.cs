using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Xamarin;

namespace TalentPlus.Shared
{
	public class ActivitiesViewModel : BaseViewModel
	{
		public ActivitiesViewModel ()
		{
		}

		private ObservableCollection<Activity> activities = new ObservableCollection<Activity> ();

		/// <summary>
		/// gets or sets the feed items
		/// </summary>
		public ObservableCollection<Activity> Activities {
			get { return activities; }
			set {
				activities = value;
				OnPropertyChanged ("Activities");
			}
		}

		private ObservableCollection<ActivityArchive> activityArchives = new ObservableCollection<ActivityArchive> ();

		public ObservableCollection<ActivityArchive> ActivityArchives {
			get { return activityArchives; }
			set {
				activityArchives = value;
				OnPropertyChanged ("ActivityArchives");
			}
		}

		private IList<ActivityInvitation> dbActivityInvitations = new ObservableCollection<ActivityInvitation> ();

		public IList<ActivityInvitation> DbActivityInvitations {
			get { return dbActivityInvitations; }
			set {
				dbActivityInvitations = value;
				OnPropertyChanged ("DbActivityInvitations");
			}
		}

		private ObservableCollection<SelectedActivity> selectedActivities = new ObservableCollection<SelectedActivity> ();

		public ObservableCollection<SelectedActivity> SelectedActivities {
			get { return selectedActivities; }
			set {
				selectedActivities = value;
				OnPropertyChanged ("SelectedActivities");
			}
		}

		public async Task<bool> AddSelectedActivity (string activityId, DateTime alarmTime, String subject, String message, bool isAccepted = false)
		{
			Activity activity = this.GetActivity (activityId);

			if (activity == null) {
				activity = await GetActivityFromDb (activityId);
			}
			
			if (activity != null) {
				var sact = new SelectedActivity (activity, alarmTime);
				sact.InvolvedUsers = new List<User> ();
				sact.InvolvedUserIds = new List<string> ();

				if (activity.SelectedUsers != null) {
					foreach (User user in activity.SelectedUsers) {
						sact.InvolvedUsers.Add (user);
						sact.InvolvedUserIds.Add (user.Id);
					}
				}

				SelectedActivities.Add (sact);

				await TalentDb.SaveOrUpdateItem<SelectedActivity> (sact);

				if (!isAccepted) {
					await SendInvitationsFromSelectedActivity (sact, subject, message);
				}
				return true;
			}
			return false;
		}

		public int ActivitiesCompletedInSixMonths { get; set; }

		public int ActivitiesCompletedPercent { get; set; }

		private ObservableCollection<PendingFeedback> pendingFeedbacks = new ObservableCollection<PendingFeedback> ();

		public ObservableCollection<PendingFeedback> PendingFeedbacks {
			get { return pendingFeedbacks; }
			set {
				pendingFeedbacks = value;
				OnPropertyChanged ("PendingFeedbacks");
			}
		}

		public async Task<bool> AddPendingFeedback (string activityId, DateTime receiveTime)
		{
			Activity activity = this.GetActivity (activityId);
			if (activity == null) {
				activity = await this.GetActivityFromDb (activityId);
			}
			SelectedActivity selectedActivity = this.GetSelectedActivity (activityId);
			if (selectedActivity == null) {
				await this.GetSelectedActivitiesFromDb ();
				selectedActivity = this.GetSelectedActivity (activityId);
			}

			if (activity != null && selectedActivity != null) {
				var pfeed = new PendingFeedback (activity, receiveTime);

				foreach (string involvedUserId in selectedActivity.InvolvedUserIds) {
					pfeed.InvolvedUserIds.Add (involvedUserId);
				}
				foreach (User involvedUser in selectedActivity.InvolvedUsers) {
					pfeed.InvolvedUsers.Add (involvedUser);
				}

				await TalentDb.DeleteItem<SelectedActivity> (selectedActivity);
				SelectedActivities.Remove (selectedActivity);

				await TalentDb.SaveOrUpdateItem<PendingFeedback> (pfeed);
				pendingFeedbacks.Add (pfeed);

				await TalentPlusApp.RootPage.OverviewRefresh();
				return true;
			}
			return false;
		}

		public async Task RemovePendingFeedBackAndAddArchive (string activityId)
		{
			PendingFeedback pendingFeedback = PendingFeedbacks.FirstOrDefault (i => i.ActivityId == activityId);

			if (pendingFeedback == null) {
				return;
			}

			var feedbackValue = 0.0f;

			if (pendingFeedback.Activity != null && pendingFeedback.Activity.ActivityStatistics != null &&
			    pendingFeedback.Activity.ActivityStatistics.EaseAverage >= 0.0f && pendingFeedback.Activity.ActivityStatistics.EffectivenessAverage >= 0.0f) {
				feedbackValue = (pendingFeedback.Activity.ActivityStatistics.EaseAverage + pendingFeedback.Activity.ActivityStatistics.EffectivenessAverage) / 2;
			}

			ActivityArchive activityArchive = new ActivityArchive {
				Activity = pendingFeedback.Activity,
				ActivityId = pendingFeedback.ActivityId,
				Feedback = feedbackValue,
				FinishTime = pendingFeedback.ReceiveTime,
				InvolvedUserIds = pendingFeedback.InvolvedUserIds,
				InvolvedUsers = pendingFeedback.InvolvedUsers
			};

			foreach (ActivityInvitation ai in DbActivityInvitations) {
				if (ai.ActivityId == pendingFeedback.ActivityId) {
					ai.Visible = false;
					await TalentDb.SaveOrUpdateItem<ActivityInvitation> (ai);
				}
			}

			try {
				await TalentDb.SaveOrUpdateItem<ActivityArchive> (activityArchive);
			} catch (Exception ex) {
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "ActivitiesViewModel.RemovePendingFeedBackAndAddArchive()" }
				});
			}

			PendingFeedbacks.Remove (pendingFeedback);
			try {
				await TalentDb.DeleteItem<PendingFeedback> (pendingFeedback);
			} catch (Exception ex) {
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "ActivitiesView.RemovePendingFeedBackAndAddArchive()" }
				});
			}
		}

		public async Task RemovePendingFeedBack (string activityId)
		{
			PendingFeedback pendingFeedback = PendingFeedbacks.FirstOrDefault (i => i.ActivityId == activityId);

			if (pendingFeedback == null) {
				return;
			}

			foreach (ActivityInvitation ai in DbActivityInvitations) {
				if (ai.ActivityId == pendingFeedback.ActivityId) {
					ai.Visible = false;
					await TalentDb.SaveOrUpdateItem<ActivityInvitation> (ai);
				}
			}

			PendingFeedbacks.Remove (pendingFeedback);
			try {
				await TalentDb.DeleteItem<PendingFeedback> (pendingFeedback);
			} catch (Exception ex) {
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "ActivitiesView.RemovePendingFeedBackAndAddArchive()" }
				});
			}
		}

		private Activity selectedActivity;

		/// <summary>
		/// Gets or sets the selected feed item
		/// </summary>
		public Activity SelectedActivity {
			get{ return selectedActivity; }
			set {
				selectedActivity = value;
				OnPropertyChanged ("SelectedActivity");
			}
		}

		private Command loadItemsCommand;

		/// <summary>
		/// Command to load/refresh items
		/// </summary>
		public Command LoadItemsCommand {
			get { return loadItemsCommand ?? (loadItemsCommand = new Command (async () => await ExecuteLoadItemsCommand ())); }
		}

		private async Task ExecuteLoadItemsCommand ()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try {
				Activities.Clear ();
				var activities = await TalentDb.GetActivities ();
				Activities = new ObservableCollection<Activity> (activities.Where(x => x.Order.Hidden == false));

				PendingFeedbacks.Clear ();
				var pendingFeedbacks = await TalentDb.GetPendingFeedback ();
				PendingFeedbacks = new ObservableCollection<PendingFeedback> (pendingFeedbacks);

				await this.GetSelectedActivitiesFromDb ();

				DbActivityInvitations.Clear ();
				var dbActivityInvitations = await TalentDb.GetActivityInvitations ();
				DbActivityInvitations = new ObservableCollection<ActivityInvitation> (dbActivityInvitations);

				//Remove the current activities
				foreach (PendingFeedback pf in PendingFeedbacks) {
					Activities.Remove (GetActivity (pf.ActivityId));
				}

				foreach (SelectedActivity sa in SelectedActivities) {
					Activities.Remove (GetActivity (sa.ActivityId));
				}

				foreach (ActivityInvitation ai in DbActivityInvitations) {
					if ((ai.SenderUserId != TalentPlusApp.CurrentUserId) && (ai.InvitationStatus == InvitationStatus.Pending)) {
						Activities.Remove (GetActivity (ai.ActivityId));
					}
				}

				ActivityArchives.Clear ();
				var activityArchives = await TalentDb.GetActivityArchives ();
				ActivityArchives = new ObservableCollection<ActivityArchive> (activityArchives);

				ActivitiesCompletedPercent = await Helpers.ActivityHelper.GetActivitiesCompletedPercent ();
				ActivitiesCompletedInSixMonths = await Helpers.ActivityHelper.GetActivitiesCompletedInSixMonths ();

				CanLoadMore = false;
			} catch (Exception ex) {
				var page = new ContentPage ();
				var result = page.DisplayAlert ("Error", "Unable to load activities.", "OK");
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "ActivitiesViewModel.ExecuteLoadItemsCommand()" },
					{ "What", "Unable to load activities" }
				});
			}

			IsBusy = false;
			OnBusyStateChanged (EventArgs.Empty);
		}

		public async Task RefreshCountItems ()
		{
			ActivitiesCompletedPercent = await Helpers.ActivityHelper.GetActivitiesCompletedPercent ();
			ActivitiesCompletedInSixMonths = await Helpers.ActivityHelper.GetActivitiesCompletedInSixMonths ();
		}

		private Command refreshActivitiesFeedbacksCommand;

		/// <summary>
		/// Command to load/refresh items
		/// </summary>
		public Command RefreshActivitiesFeedbacksCommand {
			get { return refreshActivitiesFeedbacksCommand ?? (refreshActivitiesFeedbacksCommand = new Command (async () => await RefreshFeedback ())); }
		}

		public async Task RefreshFeedback()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try {
				await TalentDb.SyncFeedback ();

				foreach (Activity activity in Activities)
				{
					var detailsView = TalentPlusApp.RootPage.activities.GetActivityDetailsViewFromActivityId(activity.Id);
					if (detailsView != null)
					{
						await detailsView.RefreshFeedback();
					}
				}

			} catch (Exception ex) {
				var page = new ContentPage ();
				var result = page.DisplayAlert ("Error", "Unable to load activities.", "OK");
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "ActivitiesViewModel.RefreshActivitiesFeedbacksCommand()" },
					{ "What", "Unable to load activities" }
				});
			}

			IsBusy = false;
//			//OnBusyStateChanged(EventArgs.Empty);
		}

		private Command loadInvitationsCommand;

		/// <summary>
		/// Command to load/refresh items
		/// </summary>
		public Command LoadInvitationsCommand {
			get { return loadInvitationsCommand ?? (loadInvitationsCommand = new Command (async () => await LoadInvitations ())); }
		}

		public async Task LoadInvitations ()
		{
			await TalentDb.SyncInvitations ();
			DbActivityInvitations = await TalentDb.GetActivityInvitations ();

			ActivitiesCompletedPercent = await Helpers.ActivityHelper.GetActivitiesCompletedPercent ();
			ActivitiesCompletedInSixMonths = await Helpers.ActivityHelper.GetActivitiesCompletedInSixMonths ();

			foreach (ActivityInvitation ai in DbActivityInvitations) {
				if (ai.SenderUserId != TalentPlusApp.CurrentUserId) {
					Activities.Remove (GetActivity (ai.ActivityId));
				}
			}

			await TalentPlusApp.RootPage.OverviewRefresh();
			await TalentPlusApp.RootPage.RefreshActivities();
		}

		public async Task StartActivity (Activity activity, int delay, String subject, String message, bool isAccepted = false)
		{
			DateTime time = DateTime.Now.AddSeconds (delay);
			await AddSelectedActivity (activity.Id, time, subject, message, isAccepted);

			//Start Reminder Service
			var remiderService = DependencyService.Get<IReminderService> ();
#if __ANDROID__
			if (remiderService != null)
				remiderService.Remind (time, "Activity Reminder", activity.ShortDescription, activity.Id, activity.AlarmId);
#else
			if (remiderService != null)
				remiderService.Remind(delay, "Activity Reminder", activity.ShortDescription, activity.Id, activity.AlarmId);
#endif
		}

		/// <summary>
		/// Gets a specific feed item for an Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Activity GetActivity (string id)
		{
			return Activities.FirstOrDefault (i => i.Id == id);
		}

		public async Task<Activity> GetActivityFromDb (string id)
		{
			return await TalentDb.GetActivity (id);
		}

		public ActivityInvitation GetActivityInvitation (string id)
		{
			var activityInv = DbActivityInvitations.FirstOrDefault (i => i.Id == id);
			return activityInv;
		}

		public SelectedActivity GetSelectedActivity (string activityId)
		{
			return SelectedActivities.FirstOrDefault (i => i.ActivityId == activityId);
		}

		public async Task GetSelectedActivitiesFromDb ()
		{
			SelectedActivities.Clear ();
			var selectedActivities = await TalentDb.GetSelectedActivities ();
			SelectedActivities = new ObservableCollection<SelectedActivity> (selectedActivities);
		}


		public List<Activity> GetInnactiveActivities ()
		{
			return Activities.ToList ();
		}

		public bool EmptiedActivities { get; set; }

		private async Task SendInvitationsFromSelectedActivity (SelectedActivity selectedAct, string subject = "", string message = "")
		{
			foreach (User invitedUser in selectedAct.InvolvedUsers) {
				ActivityInvitation activityInvitation = new ActivityInvitation {
					Activity = selectedAct.Activity,
					ActivityId = selectedAct.ActivityId,
					FinishTime = selectedAct.FinishTime,
					InvitationStatus = InvitationStatus.Pending,
					Visible = true,
					ReceiveTime = DateTime.Now,
					TargetUser = invitedUser,
					TargetUserId = invitedUser.Id,
					SenderUserId = TalentPlusApp.CurrentUserId,
					Subject = subject,
					Message = message,
				};
				await TalentDb.SaveOrUpdateItem<ActivityInvitation> (activityInvitation);
				DbActivityInvitations.Add (activityInvitation);
			}
		}

		protected virtual void OnBusyStateChanged (EventArgs e)
		{
			EventHandler handler = BusyStateChanged;
			if (handler != null) {
				handler (this, e);
			}
		}

		public event EventHandler BusyStateChanged;
	}
}

