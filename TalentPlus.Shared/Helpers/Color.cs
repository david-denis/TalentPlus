using System;

#if __IOS__
using UIKit;
using CoreGraphics;
#endif

namespace TalentPlus.Shared.Helpers
{
	public struct Color
	{
		public static readonly Color White = 0xFFFFFF;
		public static readonly Color Black = 0x000000;
		public static readonly Color Purple = 0xB455B6;
		public static readonly Color Blue = 0x3498DB;
		public static readonly Color DarkBlue = 0x2C3E50;
		public static readonly Color RedNotif = 0xD4000F;
		public static readonly Color Green = 0x77D065;
		public static readonly Color GreenNotif = 0x72AD38;
		public static readonly Color Gray = 0xBCBEC0;
		public static readonly Color GrayBackground = 0xEFECF4;

		public static Color Primary = TalentPlusApp.Settings.ColorPrimary;
		public static Color Secondary = TalentPlusApp.Settings.ColorSecondary;

		public static readonly Color UniLeverBlue = 0x0B85C8;
		public static readonly Color UniLeverDarkGray = 0x9a9a9a;
		public static readonly Color UniLeverMidGray = 0xdcdcdc;
		public static readonly Color UniLeverLightGray = 0xececec;

		public double R, G, B;

		public static Color FromHex (int hex)
		{
			Func<int, int> at = offset => (hex >> offset) & 0xFF;
			return new Color {
				R = at (16) / 255.0,
				G = at (8) / 255.0,
				B = at (0) / 255.0
			};
		}

		public static implicit operator Color (int hex)
		{
			return FromHex (hex);
		}

		#if __IOS__
		public UIColor ToUIColor ()
		{
			return UIColor.FromRGB ((float)R, (float)G, (float)B);
		}

		public static implicit operator UIColor (Color color)
		{
			return color.ToUIColor ();
		}

		public static implicit operator CGColor (Color color)
		{
			return color.ToUIColor ().CGColor;
		}
		#endif

		public Xamarin.Forms.Color ToFormsColor ()
		{
			return Xamarin.Forms.Color.FromRgb ((int)(255 * R), (int)(255 * G), (int)(255 * B));
		}


		public int ToIntColor ()
		{
			#if __ANDROID__
            return this.ToAndroidColor().ToArgb();
			#endif

			#if __IOS__
			return this.ToHexInt ();
			#endif
		}

		public Xamarin.Forms.Color ToFormsColorWithAlpha (int alpha)
		{			
			return Xamarin.Forms.Color.FromRgba ((int)(255 * R), (int)(255 * G), (int)(255 * B), alpha);
		}

		public int ToHexInt ()
		{
			return ((int)(255 * 1 << 24) | ((int)(255 * R) << 16) |
				((int)(255 * G) << 8) | ((int)(255 * B) << 0));
		}

		#if __ANDROID__
		public global::Android.Graphics.Color ToAndroidColor ()
		{
			return global::Android.Graphics.Color.Rgb ((int)(255 * R), (int)(255 * G), (int)(255 * B));
		}

		public static implicit operator global::Android.Graphics.Color (Color color)
		{
			return color.ToAndroidColor ();
		}
		#endif
	}
}