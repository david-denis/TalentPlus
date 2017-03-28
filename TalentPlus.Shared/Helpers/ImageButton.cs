using System;
using System.Globalization;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace TalentPlus.Shared.Helpers
{
	public enum ImageOrientation
	{
		/// <summary>
		/// The image to left
		/// </summary>
		ImageToLeft = 0,
		/// <summary>
		/// The image on top
		/// </summary>
		ImageOnTop = 1,
		/// <summary>
		/// The image to right
		/// </summary>
		ImageToRight = 2,
		/// <summary>
		/// The image on bottom
		/// </summary>
		ImageOnBottom = 3,
		/// <summary>
		/// The image on center
		/// </summary>
		ImageOnCenter = 4,
	}

	public class ImageSourceConverter : TypeConverter
	{
		/// <summary>
		/// Checks to see if the type attempted to be converted from is a string.
		/// </summary>
		/// <param name="sourceType">The type that is attempting to be converted.</param>
		/// <returns>Returns true if the sourceType is a <see cref="string"/>.</returns>
		public override bool CanConvertFrom(Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Converts the string value into a <see cref="ImageSource"/> either from a file or URI.
		/// </summary>
		/// <param name="culture">The current culture being used.</param>
		/// <param name="value">The string value to convert.</param>
		/// <returns>Returns a <see cref="ImageSource"/> loaded from the value.</returns>
		public override object ConvertFrom(CultureInfo culture, object value)
		{
			if (value == null)
			{
				return null;
			}

			var str = value as string;
			if (str != null)
			{
				Uri result;
				if (!Uri.TryCreate(str, UriKind.Absolute, out result) || !(result.Scheme != "file"))
				{
					return ImageSource.FromFile(str);
				}
				return ImageSource.FromUri(result);
			}
			throw new InvalidOperationException(
				string.Format("Cannot convert \"{0}\" into {1}",
					new[] { value, typeof(ImageSource) }));
		}
	}

	public class ImageButton : Button
	{
		/// <summary>
		/// Backing field for the Image property.
		/// </summary>
		public static readonly BindableProperty SourceProperty = BindableProperty.Create<ImageButton, ImageSource>((Expression<Func<ImageButton, ImageSource>>)(w => w.Source), (ImageSource)null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate<ImageSource>)null, (BindableProperty.BindingPropertyChangedDelegate<ImageSource>)((bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString()), (BindableProperty.BindingPropertyChangingDelegate<ImageSource>)null, (BindableProperty.CoerceValueDelegate<ImageSource>)null);

		/// <summary>
		/// Gets or sets the ImageSource to use with the control.
		/// </summary>
		/// <value>
		/// The Source property gets/sets the value of the backing field, SourceProperty.
		/// </value>
		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		/// <summary>
		/// Backing field for the orientation property.
		/// </summary>
		public static readonly BindableProperty OrientationProperty =
			BindableProperty.Create<ImageButton, ImageOrientation>(
				p => p.Orientation, ImageOrientation.ImageToLeft);

		/// <summary>
		/// Gets or sets The orientation of the image relative to the text.
		/// </summary> 
		/// <value>
		/// The Orientation property gets/sets the value of the backing field, OrientationProperty.
		/// </value> 
		public ImageOrientation Orientation
		{
			get { return (ImageOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		/// <summary>
		/// Backing field for the image height property.
		/// </summary>
		public static readonly BindableProperty ImageHeightRequestProperty =
			BindableProperty.Create<ImageButton, int>(
				p => p.ImageHeightRequest, default(int));

		/// <summary>
		/// Gets or sets the requested height of the image. If less than or equal to zero than a 
		/// height of 50 will be used.
		/// </summary>
		/// <value>
		/// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
		/// </value> 
		public int ImageHeightRequest
		{
			get { return (int)GetValue(ImageHeightRequestProperty); }
			set { SetValue(ImageHeightRequestProperty, value); }
		}

		/// <summary>
		/// Backing field for the image width property.
		/// </summary>
		public static readonly BindableProperty ImageWidthRequestProperty =
			BindableProperty.Create<ImageButton, int>(
				p => p.ImageWidthRequest, default(int));

		/// <summary>
		/// Gets or sets the requested width of the image.  If less than or equal to zero than a 
		/// width of 50 will be used.
		/// </summary>
		/// <value>
		/// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
		/// </value> 
		public int ImageWidthRequest
		{
			get { return (int)GetValue(ImageWidthRequestProperty); }
			set { SetValue(ImageWidthRequestProperty, value); }
		}
	}
}

