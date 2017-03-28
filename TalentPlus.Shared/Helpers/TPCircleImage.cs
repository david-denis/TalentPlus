using System;
using Xamarin.Forms;


namespace TalentPlus.Shared
{
	public class TPCircleImage : Image
	{
		/// <summary>
		/// Thickness property of border
		/// </summary>
		public static readonly BindableProperty BorderThicknessProperty =
			BindableProperty.Create<TPCircleImage, int>(
				p => p.BorderThickness, 0);

		/// <summary>
		/// Border thickness of circle image
		/// </summary>
		public int BorderThickness
		{
			get { return (int)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}

		/// <summary>
		/// Color property of border
		/// </summary>
		public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create<TPCircleImage, Color>(
				p => p.BorderColor, Color.White);

		/// <summary>
		/// Border Color of circle image
		/// </summary>
		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}
	}
}

