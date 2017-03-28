using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using TalentPlus.Shared;
using TalentPlus.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CustomScrollViewRenderer))]
namespace TalentPlus.Android
{
	public class CustomScrollViewRenderer : ScrollViewRenderer
	{
		public CustomScrollViewRenderer ()
		{
		}

		public override void Draw(Canvas canvas)
		{
			canvas.ClipRect(canvas.ClipBounds);

			base.Draw(canvas);
		}
	}
}