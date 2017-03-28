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

namespace TalentPlusAndroid
{
	internal static class DateTimeExtensions
	{
		public static long ToAndroidTimestamp(this DateTime self)
		{
			return (long)self.ToUniversalTime().Subtract(Epoch).TotalMilliseconds;
		}

		private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
	}
}