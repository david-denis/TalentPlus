using System.Reflection;
using Xamarin.Forms;
using System;
using SVG.Forms.Plugin.Abstractions;

namespace TalentPlus.Controls
{
    public class SvgColorIcon : SvgImage
    {
		#if __IOS__
		public const string IMAGE_RESOURCE = "TalentPlus.iOS.Resources.Images";
		#else
		public const string IMAGE_RESOURCE = "TalentPlus.Android.Images";
		#endif

		public Action Tapped { get; set; }
		public Action<String> TappedWithInfo { get; set; }
		public Action<String> TappedWithId { get; set; }

		public Action<String> LongTappedWithInfo { get; set; }

		public int Tag { get; set; }

		public static readonly BindableProperty TagInfoProperty =
			BindableProperty.Create<SvgColorIcon, String> (
				p => p.TagInfo, "");

		public String TagInfo {
			get {
				return (String)GetValue (TagInfoProperty);
			}

			set {
				this.SetValue (TagInfoProperty, value);
			}
		}

        public int IconColor
        {
            get { return (int)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        public static readonly BindableProperty IconColorProperty =
			BindableProperty.Create("IconColor", typeof(int), typeof(SvgColorIcon), default(int));

		public int SecondIconColor
		{
			get { return (int)GetValue(SecondIconColorProperty); }
			set { SetValue(SecondIconColorProperty, value); }
		}

		public static readonly BindableProperty SecondIconColorProperty =
			BindableProperty.Create("SecondIconColor", typeof(int), typeof(SvgColorIcon), default(int));


        public static readonly BindableProperty SvgPathForSelectedStateProperty =
            BindableProperty.Create("SvgPathForSelectedState", typeof(string), typeof(SvgColorIcon), default(string));

        public string SvgPathForSelectedState
        {
            get { return (string)GetValue(SvgPathForSelectedStateProperty); }
            set
            {
                SetValue(SvgPathForSelectedStateProperty, GetIconPath(value));
            }
        }

        public static readonly BindableProperty SelectedProperty =
           BindableProperty.Create("SvgPathForSelectedState", typeof(bool), typeof(SvgColorIcon), default(bool));

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }


        public SvgColorIcon()
            : base()
        { 
            
        }

		protected override string GetIconPath (string iconName)
		{
			return string.Format ("{0}.{1}", IMAGE_RESOURCE, iconName);
		}
    }
}