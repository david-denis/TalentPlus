using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Widget;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using Android.Graphics;

using Color = Xamarin.Forms.Color;
using View = global::Android.Views.View;
using ViewGroup = global::Android.Views.ViewGroup;
using Context = global::Android.Content.Context;
using ListView = global::Android.Widget.ListView;
using TalentPlus.Shared;
using TalentPlusAndroid;
using System.ComponentModel;
using TalentPlus.Android;

//[assembly: ExportCell(typeof(MenuCell), typeof(MenuCellRenderer))]

namespace TalentPlusAndroid
{
	public class MenuCellRenderer : ImageCellRenderer
	{
		//protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
		//{
			//var cell = (LinearLayout)base.GetCellCore(item, convertView, parent, context);
			//cell.SetPadding(20, 45, 0, 45);
			//cell.DividerPadding = 60;

			//var div = new ShapeDrawable();
			//div.SetIntrinsicHeight(1);
			//div.Paint.Set(new Paint { Color = Color.FromHex("00FFFFFF").ToAndroid() });

			//if (parent is ListView)
			//{
			//	((ListView)parent).Divider = div;
			//	((ListView)parent).DividerHeight = 1;
			//}


			//var image = (ImageView)cell.GetChildAt(0);
			//image.SetScaleType(ImageView.ScaleType.FitCenter);

			//image.LayoutParameters.Width = 60;
			//image.LayoutParameters.Height = 60;


			//var linear = (LinearLayout)cell.GetChildAt(1);
			//linear.SetGravity(Android.Views.GravityFlags.CenterVertical);

			//Typeface font = Typeface.CreateFromAsset(Forms.Context.Assets, "UnileverIllustrativeTypeBold.ttf");

			//var label = (TextView)linear.GetChildAt(0);
			//label.Typeface = font;
			//label.SetTextColor(Color.White.ToAndroid());
			//label.TextSize = Font.SystemFontOfSize(NamedSize.Large).ToScaledPixel();
			//label.Gravity = (Android.Views.GravityFlags.CenterVertical);
			//label.SetPadding(50, 0, 0, 0);
			//label.SetTextColor(Color.FromHex("0091D1").ToAndroid());
			//var secondaryLabel = (TextView)linear.GetChildAt(1);
			//secondaryLabel.Visibility = Android.Views.ViewStates.Gone;

			//var chevronImage = new ImageView(context);
			//chevronImage.SetImageResource(Resource.Drawable.chevron);
			//chevronImage.SetScaleType(ImageView.ScaleType.FitCenter);
			//chevronImage.SetPadding(0, 0, 20, 0);
			////chevronImage.LayoutParameters.Width = 60;
			////chevronImage.LayoutParameters.Height = 60;

			//cell.AddView(chevronImage);

			//return cell;
		//}
	}
}
