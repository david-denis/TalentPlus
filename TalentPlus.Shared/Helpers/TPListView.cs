using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class TPListView : ListView
	{
		//Bindable property for the progress color
		public static readonly BindableProperty MakeDisposedProperty =
			BindableProperty.Create<TPListView,bool> (p => p.MakeDisposed , false);
		//Gets or sets the color of the progress bar

		public bool MakeDisposed {
			get { return (bool)GetValue (MakeDisposedProperty); }
			set { SetValue (MakeDisposedProperty, value); }
		}

		public TPListView ():base()
		{
		}

		public void ClearAllChildren()
		{
			MakeDisposed = true;
		}
	}
}

