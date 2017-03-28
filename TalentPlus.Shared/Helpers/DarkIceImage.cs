using System;
using Xamarin.Forms;

namespace TalentPlus.Shared.Helpers
{
	public class DarkIceImage : Image
	{
		public DarkIceImage ()
        {
            
        }

       

		public string ImageName { get; set; }

		public int Tag { get; set; }

		public Action Tapped { get; set; }
		public Action<String> TappedWithInfo { get; set; }
		public Action<String> TappedWithId { get; set; }

		public Action<String> LongTappedWithInfo { get; set; }

		public bool IsDefaultImage { get; set; }

		public float LeftInset { get; set;}
		public float RightInset { get; set; }
		public float TopInset { get; set; }
		public float BottomInset { get; set; }

		public static readonly BindableProperty DataIdProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.DataId, "");

		public String DataId {
			get {
				return (String)GetValue (DataIdProperty);
			}

			set {
				this.SetValue (DataIdProperty, value);
			}
		}

		public Xamarin.Forms.Color FilterColor
		{
			get { return (Xamarin.Forms.Color)GetValue(FilterColorProperty); }
			set { SetValue(FilterColorProperty, value); }
		}

		public static readonly BindableProperty FilterColorProperty =
			BindableProperty.Create("FilterColor", typeof(Xamarin.Forms.Color), typeof(DarkIceImage), default(Xamarin.Forms.Color));


		public static readonly BindableProperty TagInfoProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.TagInfo, "");

		public String TagInfo {
			get {
				return (String)GetValue (TagInfoProperty);
			}

			set {
				this.SetValue (TagInfoProperty, value);
			}
		}

		public static readonly BindableProperty ShouldUseInsetProperty =
			BindableProperty.Create<DarkIceImage, bool> (
				p => p.ShouldUseInset, false);

		public bool ShouldUseInset {
			get {
				return (bool)GetValue (ShouldUseInsetProperty);
			}

			set {
				this.SetValue (ShouldUseInsetProperty, value);
			}
		}

		public static readonly BindableProperty ShouldDisposeImageProperty =
			BindableProperty.Create<DarkIceImage, bool> (
				p => p.ShouldDisposeImage, false);


		public bool ShouldDisposeImage {
			get {
				return (bool)GetValue (ShouldDisposeImageProperty);
			}

			set {
				this.SetValue (ShouldDisposeImageProperty, value);

//				var eventHandler = ShouldDisposeImageProperty.;
//				if (eventHandler != null)
//				{
//					eventHandler.Invoke(this, value);
//				}
			}
		}
	}

	public static class DarkIceImageExtensions
	{
		public static void DisposeImageIfNotNull (this DarkIceImage item)
		{
			if (item != null)
				item.ShouldDisposeImage = true;
		}
	}
}

