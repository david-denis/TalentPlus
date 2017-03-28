using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace TalentPlus.Shared
{
	public static class ViewModelLocator
    {
		private static ActivitiesViewModel activitiesViewModel = new ActivitiesViewModel();
		public static ActivitiesViewModel ActivitiesViewModel
		{
			get
			{
				return activitiesViewModel;
			}
		}

		private static UsersViewModel usersViewModel = new UsersViewModel();
		public static UsersViewModel UsersViewModel
		{
			get
			{
				return usersViewModel;
			}
		}
	}
}
