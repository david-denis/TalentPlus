using System;
using TalentPlus.Shared;
using TalentPlus.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using UIKit;

[assembly: ExportRenderer(typeof(TPCircleImage), typeof(TPCircleImageRenderer))]
namespace TalentPlus.iOS
{
	public class TPCircleImageRenderer : CircleImageRenderer
	{
        public TPCircleImageRenderer()
            : base()
		{

		}

	}
}
