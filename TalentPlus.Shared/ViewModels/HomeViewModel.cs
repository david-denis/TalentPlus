using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class HomeViewModel : BaseViewModel
	{
		public ObservableCollection<HomeMenuItem> MenuItems { get; set; }
		public HomeViewModel ()
		{
			CanLoadMore = true;
			Title = "TalentPlus";
			MenuItems = new ObservableCollection<HomeMenuItem> ();
			MenuItems.Add(new HomeMenuItem
			{
				Id = 0,
				Title = "Me",
				MenuType = MenuType.Me,
				Icon = "user_icon.png",
                TextColor = Helpers.Color.White.ToFormsColor()
			});
			MenuItems.Add(new HomeMenuItem
			{
				Id = 1,
				Title = "Overview",
				MenuType = MenuType.Overview,
				Icon = "overview.png",
				TextColor = Helpers.Color.White.ToFormsColor()
			});
			MenuItems.Add(new HomeMenuItem
			{
				Id = 1,
				Title = "Activities",
				MenuType = MenuType.Activities,
				Icon = "activities.png",
				TextColor = Helpers.Color.White.ToFormsColor()
			});
			MenuItems.Add(new HomeMenuItem
			{
				Id = 2,
				Title = "My Team",
				MenuType = MenuType.Team,
				Icon = "team.png",
				TextColor = Helpers.Color.White.ToFormsColor()
			});
			MenuItems.Add(new HomeMenuItem
			{
				Id = 3,
				Title = "Settings",
				MenuType = MenuType.Settings,
				Icon = "small_icons_menu_settings.png",
				TextColor = Helpers.Color.White.ToFormsColor()
			});
		}

        public void ChangeTextColor(MenuType type)
        {
            for (int i = 0; i < MenuItems.Count; i++ )
            {
				if (MenuItems [i].MenuType == type) {
					MenuItems [i].TextColor = Xamarin.Forms.Color.White;

					#if __ANDROID__

					if(!MenuItems[i].Icon.Contains("_white")){
						MenuItems[i].Icon = MenuItems[i].Icon.Replace(".png","") + "_white.png";
					}
					#endif
				} else {
					MenuItems [i].TextColor = Helpers.Color.Primary.ToFormsColor ();

					#if __ANDROID__

					if(MenuItems[i].Icon.Contains("white")){
						MenuItems[i].Icon = MenuItems[i].Icon.Replace("_white","");
					}

					#endif
				}
            }
        }
	}
}

