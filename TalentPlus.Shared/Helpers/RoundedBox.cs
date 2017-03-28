using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    public class RoundedBox : BoxView
	{
		public enum RoundedSideType
		{
			Top,
			Bottom,
			All
		}

		/// <summary>
		/// The corner radius property.
		/// </summary>
		public static readonly BindableProperty CornerRadiusProperty =
			BindableProperty.Create("CornerRadius", typeof(double), typeof(RoundedBox), 0.0);
 
		/// <summary>
		/// Gets or sets the corner radius.
		/// </summary>
		public double CornerRadius
		{
			get { return (double)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		/// <summary>
		/// The rounded side property.
		/// </summary>
		public static readonly BindableProperty RoundedSideProperty =
			BindableProperty.Create("CornerRadius", typeof(RoundedSideType), typeof(RoundedSideType), RoundedSideType.All);

		/// <summary>
		/// Gets or sets the rounded side.
		/// </summary>
		public RoundedSideType RoundedSide
		{
			get { return (RoundedSideType)GetValue(RoundedSideProperty); }
			set { SetValue(RoundedSideProperty, value); }
		}

		protected override void OnPropertyChanged (string propertyName)
		{
			base.OnPropertyChanged (propertyName);

		}
	}
}
