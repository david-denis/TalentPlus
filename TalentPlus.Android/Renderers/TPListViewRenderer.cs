using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TalentPlus.Shared;
using TalentPlusAndroid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(TPListView), typeof(TPListViewRenderer))]

namespace TalentPlusAndroid
{
	public class TPListViewRenderer : ListViewRenderer
	{
		protected bool _isDiposed = false;

		protected TPListView _listView;

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			if (!_isDiposed) {
				base.OnLayout (changed, l, t, r, b);
			}


		}

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				_listView = e.NewElement as TPListView;
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == TPListView.MakeDisposedProperty.PropertyName) {
				var tpListView = this.Element as TPListView;
				if (tpListView.MakeDisposed) {
					this.Control.Adapter = null;

					for (int i = 0; i < this.Control.ChildCount; i++) {
						var child = this.Control.GetChildAt (i);
						if (child != null) {
							child.Dispose ();
							child = null;
						}
					}
					this.Control.RemoveAllViewsInLayout ();

					_isDiposed = true;
				}
			} else {
				base.OnElementPropertyChanged (sender, e);
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);


		}

	}
}