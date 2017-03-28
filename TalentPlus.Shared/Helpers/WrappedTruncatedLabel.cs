using System;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;

namespace TalentPlus.Shared
{
    public class WrappedTruncatedLabel : Label
    {

        public static readonly BindableProperty LinesProperty =
            BindableProperty.Create("Lines", typeof(int), typeof(WrappedTruncatedLabel), 
                0);

        public int Lines
        {
            get { return (int)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }
    }
}