using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class TPButton : Button
	{
		public TPButton () : base()
		{
			MessagingCenter.Subscribe<string>(this,"ReloadActionBar", (message) => 
				{
					this.BackgroundColor = Helpers.Color.Primary.ToFormsColor();
				});
		}

		~TPButton()
		{
			MessagingCenter.Unsubscribe<string> (this, "ReloadActionBar");
		}
	}
}

