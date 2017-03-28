using System;
using System.ComponentModel;

namespace TalentPlus.Shared
{
	public enum MenuType
	{
		Me,
		Overview,
		Activities,
		Team,
		Settings
	}

	public class HomeMenuItem : BaseModel, INotifyPropertyChanged 
	{
        public event PropertyChangedEventHandler PropertyChanged;

		public HomeMenuItem ()
		{
			MenuType = MenuType.Overview;
		}

		private string _icon;
		public string Icon 
		{get
			{
				return _icon;
			}
			set
			{
				_icon = value;
				OnPropertyChanged("Icon");
			}
		}
		public MenuType MenuType { get; set; }

        private Xamarin.Forms.Color textColor;
        public Xamarin.Forms.Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                OnPropertyChanged("TextColor");
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}

