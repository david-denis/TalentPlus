using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using System.Reflection;
using SVG.Forms.Plugin.Abstractions;

namespace TalentPlus.Shared
{
	public class SettingsPage : BaseView
	{
		#if __IOS__
		public const string IMAGE_RESOURCE = "TalentPlus.iOS.Resources.Images";

#else
		public const string IMAGE_RESOURCE = "TalentPlus.Android.Images";
		#endif

		private StackLayout _innerLayout, _mainLayout;
		private Grid _userGrid;
		private Grid _configGrid;

		protected SvgImage layerImage;

		private Dictionary<string, string> NameToCountry = new Dictionary<string, string> {
			{ "United Kingdom", "UK" },
			{ "Spain", "ES" }
		};

		private TPCircleImage UserProfileImage;

		private bool IsPhotoChanged { get; set; }

		private Entry nameEntry { get; set; }

		TPButton BtnChangePhoto { get;set; }

		public SettingsPage ()
		{
			BuildLayout ();
			return;
		}

		private void BuildLayout ()
		{
			_mainLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = 0
			};

			_innerLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0 - 0,
				Padding = 0
			};

			_userGrid = new Grid {
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 90,
				Padding = new Thickness (10),
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (3, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (7, GridUnitType.Star) }
				}
			};

			var userImageSource = "user_icon.png";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty (TalentPlusApp.CurrentUser.UserImage))
				userImageSource = TalentPlusApp.CurrentUser.UserImage;

			UserProfileImage = new TPCircleImage {
				Source = userImageSource,
				HorizontalOptions =	LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HeightRequest = 90,
				WidthRequest = 90,
				Aspect = Aspect.AspectFill
			};

			UserProfileImage.GestureRecognizers.Add(new TapGestureRecognizer(async (View obj) => {
				var result = await DisplayActionSheet ("Choose source", "Cancel", null, new string[2] {
					"Camera",
					"Photo Library"
				});

				if(result == null || result.Equals("Cancel"))
				{
					return;
				}
				else if (result.Equals ("Camera")) {
					await TakePicture ();
					if (ImageSource != null)
					{
						UserProfileImage.Source = ImageSource;
						SavePhoto();
					}
				} else if (result.Equals("Photo Library")){
					await SelectPicture ();
					if (ImageSource != null)
					{
						UserProfileImage.Source = ImageSource;
						SavePhoto();
					}
				}
			}));

			_userGrid.Children.Add (UserProfileImage, 0, 0);

			BtnChangePhoto = new TPButton {
				BackgroundColor = Helpers.Color.Primary.ToFormsColor (),
				Text = "Change Photo",
				TextColor = Helpers.Color.White.ToFormsColor (),
				FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(TPButton)),
				BorderWidth = 0,
				HeightRequest = 30,
				BorderColor = Helpers.Color.Primary.ToFormsColor (),
				BorderRadius = 5,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.End
			};
			BtnChangePhoto.Clicked += ChangePhotoButtonClicked;

			var userName = "Me";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty (TalentPlusApp.CurrentUser.Name))
				userName = TalentPlusApp.CurrentUser.Name;

			_userGrid.Children.Add (new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (5, 5, 0, 5),
				Children = {
					new Image {
						Source = "camera_icon.png",
						Aspect = Aspect.AspectFit,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Start,
					},
					BtnChangePhoto
					//new Label{Text = "Global Brand Lead", Font = Font.SystemFontOfSize(NamedSize.Small), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, TextColor = Helpers.Color.White.ToFormsColor() },
				}
			}, 1, 0);

			_innerLayout.Children.Add (_userGrid);
		
			//for config grid
			_configGrid = new Grid {
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				Padding = new Thickness (0),
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (6, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (3, GridUnitType.Star) }
				},

				RowDefinitions = {
					new RowDefinition { Height = new GridLength (0.3, GridUnitType.Absolute) },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength (0.3, GridUnitType.Absolute) },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength (0.3, GridUnitType.Absolute) },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength (0.3, GridUnitType.Absolute) },
				}
			};

			//ROW 1
			var topBoxView = new BoxView () {
				VerticalOptions = LayoutOptions.Start,
				Color = Helpers.Color.Gray.ToFormsColor ()
			};

			_configGrid.Children.Add (topBoxView, 0, 0);
			Grid.SetColumnSpan (topBoxView, 3);

			_configGrid.Children.Add (
				new Image {
					Source = "person.png",
					Aspect = Aspect.AspectFit,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Scale = 0.6
				}, 0, 1);

			_configGrid.Children.Add (new Label {
				Text = userName,
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				HorizontalOptions = LayoutOptions.Start, 
				VerticalOptions = LayoutOptions.Center, 
				TextColor = Helpers.Color.Black.ToFormsColor (),
			}, 1, 1);


			var bottomName = new BoxView () {
				VerticalOptions = LayoutOptions.Start,
				Color = Helpers.Color.Gray.ToFormsColor ()
			};

			_configGrid.Children.Add (bottomName, 1, 2);
			Grid.SetColumnSpan (bottomName, 2);

			//ROW 2
			_configGrid.Children.Add (
				new Image {
					Source = "global_icon.png",
					Aspect = Aspect.AspectFit,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Scale = 0.6
				}, 0, 3);

			_configGrid.Children.Add (new Label {
				Text = "English / UK",
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)), 
				HorizontalOptions = LayoutOptions.Fill, 
				VerticalOptions = LayoutOptions.Center, 
				TextColor = Helpers.Color.Black.ToFormsColor (),
			}, 1, 3);

			var bottomLanguage = new BoxView () {
				VerticalOptions = LayoutOptions.Start,
				Color = Helpers.Color.Gray.ToFormsColor ()
			};

			_configGrid.Children.Add (bottomLanguage, 1, 4);
			Grid.SetColumnSpan (bottomLanguage, 2);

			//ROW 3
			_configGrid.Children.Add (
				new Image {
					Source = "volumn_icon.png",
					Aspect = Aspect.AspectFit,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Scale = 0.6
				}, 0, 5);

			_configGrid.Children.Add (new Label {
				Text = "Sound on/off",
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)), 
				HorizontalOptions = LayoutOptions.Fill, 
				VerticalOptions = LayoutOptions.Center, 
				TextColor = Helpers.Color.Black.ToFormsColor ()
			}, 1, 5);

			_configGrid.Children.Add (
				new TPSwitch {
					IsToggled = true,
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.Center,
				}, 2, 5);

			var bottomView = new BoxView () {
				VerticalOptions = LayoutOptions.Start,
				Color = Helpers.Color.Gray.ToFormsColor ()
			};

			_configGrid.Children.Add (bottomView, 0, 6);
			Grid.SetColumnSpan (bottomView, 3);

			_innerLayout.Children.Add (_configGrid);

			//add the color scheme
			var colorSchemeLayout = new StackLayout {
				Spacing = 10,
				Padding = new Thickness (0),
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor (),
			};

			colorSchemeLayout.Children.Add (
				new ContentView {
					Padding = new Thickness (20, 0, 0, 0),
					VerticalOptions = LayoutOptions.Start,
					Content = new Label () { 
						Text = "YOUR COLOUR SCHEME",
						TextColor = Color.Black,
						FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)), 
						HorizontalOptions = LayoutOptions.Fill,
						VerticalOptions = LayoutOptions.Start,
						YAlign = TextAlignment.End,
						HeightRequest = 40,
						BackgroundColor = Color.Transparent,
					}
				}
			);

			#if __IOS__

			const float containerWidth = 300;
			const float imageWidth = 275;

			#else

			const float containerWidth = 280;
			const float imageWidth = 280;

			#endif

			var wheelLayout = new AbsoluteLayout { 
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = containerWidth,
				HeightRequest = containerWidth,
				BackgroundColor = Color.Transparent,
			};

			var colourWheelImage = new SvgImage {
				SvgPath = string.Format ("{0}.{1}", IMAGE_RESOURCE, "colour_wheel.svg"),
				SvgAssembly = typeof(TalentPlusApp).GetTypeInfo ().Assembly, 
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Transparent,
				HeightRequest = imageWidth,
				WidthRequest = imageWidth,
			};
			//int layerColor = Helpers.Color.Primary.ToIntColor ();

			layerImage = new SvgImage {
				SvgPath = string.Format ("{0}.{1}", IMAGE_RESOURCE, "hoverName.svg"),
				SvgAssembly = typeof(TalentPlusApp).GetTypeInfo ().Assembly, 
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Transparent,
				HeightRequest = imageWidth,
				WidthRequest = imageWidth,
				IsVisible = true,
				IsHoverImages = true,
				ColorCode = Helpers.Color.Primary.ToIntColor ()
			};

			colourWheelImage.TouchIn = async(int color) => {
				var intcolor = Helpers.Color.Primary.ToIntColor();
				if (intcolor != color)
				{
					layerImage.IsVisible = false;
					layerImage.ColorCode = color;

					Helpers.Color.Primary = TalentPlus.Shared.Helpers.Color.FromHex(color);
					Helpers.Color.Secondary = TalentPlus.Shared.Helpers.Color.FromHex(SvgImage.Colors_Matrix[color]);

					TalentPlusApp.Settings.ColorPrimary = color;
					TalentPlusApp.Settings.ColorSecondary = SvgImage.Colors_Matrix[color];
				 	await TalentDb.SaveOrUpdateItem<UserSettings>(TalentPlusApp.Settings);

					TalentPlus.Shared.Helpers.Utility.RefreshActionBar();
					TalentPlus.Shared.Helpers.Utility.UpdateColorWheel();

					layerImage.IsVisible = true;
				}
			};

//			colourWheelImage.TouchCancel = () => {
//				
//				//layerImage.IsVisible = false;
//			};


			wheelLayout.Children.Add (colourWheelImage);
			wheelLayout.Children.Add (layerImage);

			colorSchemeLayout.Children.Add (new StackLayout () { 
				Spacing = 10,
				BackgroundColor = Color.White,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness (20, 10, 20, 10),
				Children = {
					new Label () {
						Text = "Use the color wheel below to choose your colour scheme for the app",
						TextColor = Color.Gray,
						VerticalOptions = LayoutOptions.Start,
						FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)), 
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						XAlign = TextAlignment.Center
					},
					wheelLayout
				}
			});
			

			_innerLayout.Children.Add (colorSchemeLayout);
			_mainLayout.Children.Add (new ScrollView {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = _innerLayout
			});

			Content = _mainLayout;
		}

		async void SavePhoto ()
		{
			User myself = TalentPlusApp.CurrentUser;
			if (!IsPhotoChanged) {
				return;
			}

			LoadingViewFlag = true;

			if (IsPhotoChanged) {
				var ImageUrl = await Helpers.Utility.UploadPhotoWithSize (ImageBytes, 300, 300);

				myself.UserImage = ImageUrl;
			}
			await TalentDb.SaveOrUpdateItem<User> (myself);

			LoadingViewFlag = false;
			IsPhotoChanged = false;
			MessagingCenter.Send<SettingsPage> (this, "UserImageChanged");
		}

		#region PHOTO FUNCTIONS

		private readonly TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext ();
		private IMediaPicker _mediaPicker;
		private ImageSource _imageSource;
		private string _status;

		public string ImagePath { get; set; }

		public byte[] ImageBytes { get; set; }

		/// <summary>
		/// Gets or sets the image source.
		/// </summary>
		/// <value>The image source.</value>
		public ImageSource ImageSource {
			get {
				return _imageSource;
			}
			set {
				_imageSource = value;
			}
		}

		public string Status {
			get { return _status; }
			set { _status = value; }
		}

		private void Setup ()
		{
			if (_mediaPicker != null) {
				return;
			}

			var device = Resolver.Resolve<IDevice> ();

			////RM: hack for working on windows phone? 
			//_mediaPicker = DependencyService.Get<IMediaPicker>() ?? device.MediaPicker;
#if __ANDROID__
			_mediaPicker = Resolver.Resolve<IMediaPicker> () ?? device.MediaPicker;
#else
			_mediaPicker = DependencyService.Get<IMediaPicker> () ?? device.MediaPicker;
#endif
		}

		private async Task<MediaFile> TakePicture ()
		{
			Setup ();

			ImageSource = null;

			return await _mediaPicker.TakePhotoAsync (new CameraMediaStorageOptions {
				DefaultCamera = CameraDevice.Front,
				MaxPixelDimension = 400
			}).ContinueWith (t => {
				if (t.IsFaulted) {
					Status = t.Exception.InnerException.ToString ();
				} else if (t.IsCanceled) {
					Status = "Canceled";
				} else {
					var mediaFile = t.Result;

					ImagePath = mediaFile.Path;

					byte[] bytes = new byte[mediaFile.Source.Length];
					mediaFile.Source.Read (bytes, 0, System.Convert.ToInt32 (mediaFile.Source.Length));
					ImageBytes = bytes;

					ImageSource = ImageSource.FromStream (() => new System.IO.MemoryStream (Helpers.ImageResizer.ResizeImage (bytes, 300, 300)));
					IsPhotoChanged = true;
					//ImageSource = ImageSource.FromStream(() => mediaFile.Source);


					return mediaFile;
				}

				return null;
			}, _scheduler);
		}

		private async Task SelectPicture ()
		{
			Setup ();

			ImageSource = null;
			try {
				var mediaFile = await _mediaPicker.SelectPhotoAsync (new CameraMediaStorageOptions {
					DefaultCamera = CameraDevice.Front,
					MaxPixelDimension = 400
				});
				if(mediaFile == null)
				{
					return;
				}
				ImagePath = mediaFile.Path;

				byte[] bytes = new byte[mediaFile.Source.Length];
				mediaFile.Source.Read (bytes, 0, System.Convert.ToInt32 (mediaFile.Source.Length));
				ImageBytes = bytes;

				ImageSource = ImageSource.FromStream (() => new System.IO.MemoryStream (Helpers.ImageResizer.ResizeImage (bytes, 300, 300)));
				IsPhotoChanged = true;
				//ImageSource = ImageSource.FromStream(() => mediaFile.Source);
			}
			catch (TaskCanceledException) {
				
			}
			catch (System.Exception ex) {
				Status = ex.Message;
				Insights.Report (ex, new Dictionary<string, string> {
					{ "Where", "SettingsPage.SelectPicture()" }
				});
			}
		}

		async void ChangePhotoButtonClicked (object s, EventArgs e) {
				BtnChangePhoto.Clicked -= ChangePhotoButtonClicked;
				var result = await DisplayActionSheet ("Choose source", "Cancel", null, new string[2] {
					"Camera",
					"Photo Library"
				});
				BtnChangePhoto.Clicked += ChangePhotoButtonClicked;
				if(result == null || result.Equals("Cancel"))
				{
					return;
				}
				else if (result.Equals ("Camera")) {
					await TakePicture ();
					if (ImageSource != null)
					{
						UserProfileImage.Source = ImageSource;
						SavePhoto();
					}
				} else if (result.Equals("Photo Library")){
					await SelectPicture ();
					if (ImageSource != null)
					{
						UserProfileImage.Source = ImageSource;
						SavePhoto();
					}
				}
			}

		#endregion
	}
}

