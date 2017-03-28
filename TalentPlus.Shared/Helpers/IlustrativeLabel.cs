using System;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;

namespace TalentPlus.Shared
{
	public class IlustrativeLabel : DILabel
	{
		public IlustrativeLabel () : base()
		{			
#if __IOS__
			FontFamily = "Unilever Illustrative Type";
			FontAttributes = FontAttributes.Bold;
			//IsDefaultLabel = true;
#endif
		}

		protected override void OnPropertyChanged (string propertyName)
		{
			base.OnPropertyChanged (propertyName);

			#if __IOS__
			FontFamily = "Unilever Illustrative Type";
			FontAttributes = FontAttributes.Bold;
#endif
		}
	}
}