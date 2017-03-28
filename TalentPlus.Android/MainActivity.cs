
using Android.App;
using Android.OS;
using Android.Content.PM;
using TalentPlus.Shared;
using XLabs.Ioc;
using XLabs.Forms;
using XLabs.Platform.Device;
using XLabs.Platform.Mvvm;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Services;
using XLabs.Serialization;
using CaveBirdLabs.Forms.Platform.Android;
using Xamarin;

[assembly: Permission(Name = Android.Manifest.Permission.Internet)]
[assembly: Permission(Name = Android.Manifest.Permission.WriteExternalStorage)]

namespace TalentPlusAndroid
{
	[Activity(Label = "Everyday Actions", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan, LaunchMode = LaunchMode.SingleTask, Theme = "@style/ThemeHoloWithActionBar")]
	public class MainActivity : XFormsApplicationDroid
	{
		// Create a new instance field for this activity.
		static MainActivity instance;

		// Return the current activity instance.
		public static MainActivity CurrentActivity
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Called when [create].
		/// </summary>
		/// <param name="bundle">The bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Insights.Initialize("1cd01f5ecb5aacbfc2156d30fd2006170a0de18b", this);

			if (!Resolver.IsSet)
			{
				this.SetIoc();
			}
			else
			{
				var app = Resolver.Resolve<IXFormsApp>() as IXFormsApp<XFormsApplicationDroid>;
				app.AppContext = this;
			}

			Xamarin.Forms.Forms.Init(this, bundle);
			//Forms.SetTitleBarVisibility(AndroidTitleBarVisibility.Never);
            Renderers.Init();

			TalentPlusApp.UIContext = this;

			this.ActionBar.SetIcon (new Android.Graphics.Drawables.ColorDrawable (Android.Graphics.Color.Transparent));

			Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.View.StyleId))
				{
					e.NativeView.ContentDescription = e.View.StyleId;
				}
			};

			LoadApplication(new TalentPlusApp());
			//this.SetPage(TalentPlusApp.GetMainPage());
			// Set the current instance of TodoActivity.
			instance = this;
        }

		public override void OnBackPressed()
		{
			// Block Back button when we don't want
			if (TalentPlusApp.BackBlocked) return;

			base.OnBackPressed();
		}
			
		protected override void OnResume ()
		{
			base.OnResume ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			TalentPlusApp.JustResume = true;
		}
	

		/// Sets the IoC.
		/// </summary>
		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();

			var app = new XFormsAppDroid();

			app.Init(this);

			resolverContainer.Register<IDevice>(t => AndroidDevice.CurrentDevice)
				.Register<IDisplay>(t => t.Resolve<IDevice>().Display)
				.Register<IMediaPicker>(t => new TalentPlusAndroid.Services.Media.MediaPicker())
				.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>()
				.Register<IDependencyContainer>(resolverContainer)
				.Register<IXFormsApp>(app)
				.Register<ISecureStorage>(t => new KeyVaultStorage(t.Resolve<IDevice>().Id.ToCharArray()));

			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}


