using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class PlaceholderEditor : Editor
	{
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create<PlaceholderEditor, string>(view => view.Placeholder, String.Empty);

		public PlaceholderEditor()
		{
		}

		public string Placeholder
		{
			get
			{
				return (string)GetValue(PlaceholderProperty);
			}

			set
			{
				SetValue(PlaceholderProperty, value);
			}
		}
	}
}

