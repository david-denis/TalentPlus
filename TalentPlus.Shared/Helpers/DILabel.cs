using System;
using Xamarin.Forms;

namespace TalentPlus.Shared.Helpers
{
	public class DILabel : Label
	{
		public DILabel ()
		{
		}

		public static readonly BindableProperty ShouldDisposeProperty =
			BindableProperty.Create<DILabel, bool> (
				p => p.ShouldDispose, false);
		public bool ShouldDispose {
			get {
				return (bool)GetValue (ShouldDisposeProperty);
			}

			set {
				this.SetValue (ShouldDisposeProperty, value);
			}
		}

		public Action Tapped;

		public bool IsDefaultLabel;

		public bool IsUnderline;

		public int Lines;
	}

	public static class DILabelExtensions
	{
		public static void DisposeLabelIfNotNull (this DILabel item)
		{
			if (item != null)
				item.ShouldDispose = true;
		}
	}
}

