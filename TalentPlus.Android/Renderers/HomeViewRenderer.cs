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
using Android.Support.V4.Widget;
using Android.App;

[assembly: ExportRenderer(typeof(HomeView), typeof(HomeViewRenderer))]
namespace TalentPlusAndroid
{
	public class HomeViewRenderer : MasterDetailRenderer
	{
		bool firstDone;

		public override void AddView(View child)
		{
			if (firstDone)
			{
				HomeView page = (HomeView)this.Element;
				LayoutParams p = (LayoutParams)child.LayoutParameters;

				//p.Width = page.DrawerWidth;
				base.AddView(child, p);
			}
			else
			{
				firstDone = true;
				base.AddView(child);
			}
		}
	}
}
