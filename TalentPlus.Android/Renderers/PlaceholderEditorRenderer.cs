using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using TalentPlus.Shared;
using System.ComponentModel;
using TalentPlus.Android;

[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(PlaceholderEditorRenderer))]
namespace TalentPlus.Android
{
	public class PlaceholderEditorRenderer : EditorRenderer
	{
		public PlaceholderEditorRenderer()
		{
		}

		protected override void OnElementChanged(
			ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				var element = e.NewElement as PlaceholderEditor;
				this.Control.Hint = element.Placeholder;
				this.Control.TextSize = 14;
			}
		}

		protected override void OnElementPropertyChanged(
			object sender,
			PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == PlaceholderEditor.PlaceholderProperty.PropertyName)
			{
				var element = this.Element as PlaceholderEditor;
				this.Control.Hint = element.Placeholder;
			}
		}
	}
}

