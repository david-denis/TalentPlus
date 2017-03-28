using System;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;

namespace TalentPlus.Shared
{
	public class UnileverLabel : DILabel
	{
		public UnileverLabel () : base ()
		{			
#if __IOS__
			FontFamily = "Unilever DIN Offc Pro";
			//FontAttributes = FontAttributes.Bold;
			//IsDefaultLabel = true;
#endif
		}

		protected override void OnPropertyChanged (string propertyName)
		{
			base.OnPropertyChanged (propertyName);

			#if __IOS__
			FontFamily = "Unilever DIN Offc Pro";
			//FontAttributes = FontAttributes.Bold;
#endif
		}
	}
}