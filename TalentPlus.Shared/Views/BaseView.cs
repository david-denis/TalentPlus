using Xamarin.Forms;
#if __ANDROID__
using Android.App;
#endif

namespace TalentPlus.Shared
{
	public class BaseView : ContentPage
	{
		#region PRIVATE VARIABLES
		#if __ANDROID__
		ProgressDialog p = null;
		#endif
		#endregion

		#region PROPERTIES
		public bool IsShowing { get; set; }
		public bool HideBackButtonFlag {
			get {
				return (bool)GetValue (HideBackProperty);
			}
			set {
				SetValue (HideBackProperty, value);
			}
		}
		public bool LoadingViewFlag {
			get {
				return (bool)GetValue (LoadingProperty);
			}
			set {
				SetValue (LoadingProperty, value);

				#if __ANDROID__
				if (value == true)
				{
					p = new ProgressDialog(Forms.Context);
					p.SetMessage("Loading...");
					p.SetCancelable(false);
					p.Show();
					IsShowing = true;
				}
				else
				{
					if (p != null)
					{
						p.Dismiss();
						p = null;
						IsShowing = false;
					}
				}
				#endif
			}
		}

		public static readonly BindableProperty LoadingProperty = 
			BindableProperty.Create ((BaseView w) => w.LoadingViewFlag, false);

		public static readonly BindableProperty HideBackProperty = 
			BindableProperty.Create ((BaseView w) => w.HideBackButtonFlag, false);
		
		#endregion

		public BaseView ()
		{
			SetBinding (Page.TitleProperty, new Binding(BaseViewModel.TitlePropertyName));
			SetBinding (Page.IconProperty, new Binding(BaseViewModel.IconPropertyName));
		}
	}
}

