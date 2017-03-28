using System;
using Xamarin.Forms;
using FormsColor = Xamarin.Forms.Color;

namespace TalentPlus.Shared
{
	public class FixedLabel : View
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create<FixedLabel, string>(p => p.Text, "");

		public static readonly BindableProperty TextColorProperty = BindableProperty.Create<FixedLabel, FormsColor>(p => p.TextColor, FormsColor.Black);

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create<FixedLabel, float>(p => p.FontSize, 8.0f);


		public readonly double FixedWidth;

		public readonly double FixedHeight;

		public Font Font;

		public LineBreakMode LineBreakMode = LineBreakMode.WordWrap;

		public TextAlignment XAlign;

		public TextAlignment YAlign;

		public FixedLabel(string text, double width, double height)
		{
			SetValue(TextProperty, text);
			FixedWidth = width;
			FixedHeight = height;
		}

		public float FontSize
		{
			get
			{
				return (float)GetValue(FontSizeProperty);
			}
			set
			{
				if (FontSize == value)
					return;
				SetValue(FontSizeProperty, value);
				OnPropertyChanged("FontSize");
			}
		}

		public FormsColor TextColor
		{
			get
			{
				return (FormsColor)GetValue(TextColorProperty);
			}
			set
			{
				if (TextColor == value)
					return;
				SetValue(TextColorProperty, value);
				OnPropertyChanged("TextColor");
			}
		}

		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				if (Text == value)
					return;
				SetValue(TextProperty, value);
				OnPropertyChanged("Text");
			}
		}

		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(FixedWidth, FixedHeight));
		}
	}
}