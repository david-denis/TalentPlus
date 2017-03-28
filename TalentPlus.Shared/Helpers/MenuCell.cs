using System;
using System.Collections.Generic;
using Xamarin;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
	public class MenuCell : ViewCell//ImageCell
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create<MenuCell, string>(p => p.Text, "");
		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create<MenuCell, string>(p => p.ImageSource, "");
		public string ImageSource
		{
			get
			{
				return (string)GetValue(ImageSourceProperty);
			}
			set
			{
				SetValue(ImageSourceProperty, value);
			}
		}

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create<MenuCell, Xamarin.Forms.Color>(p => p.TextColor, Helpers.Color.Primary.ToFormsColor());
        public Xamarin.Forms.Color TextColor 
        {
            get
            {
                return (Xamarin.Forms.Color)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }

		private StackLayout DefaultLayout = new StackLayout();
		private UnileverLabel TextLabel = new UnileverLabel();
		private Image ImageIcon = new Image();

		private string UserName = "Me";
		private string UserImageSource = "user_icon.png";

		private UnileverLabel UserNameLabel = new UnileverLabel { Text = "Me", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)), TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, IsDefaultLabel = true, };
		private TPCircleImage UserImage = new TPCircleImage { Source = "user_icon.png", WidthRequest = 70, HeightRequest = 70, Aspect = Aspect.AspectFill };

		public MenuCell()
		{
			ImageIcon.Aspect = Aspect.AspectFit;
			//ImageIcon.WidthRequest = 60;
			//ImageIcon.HeightRequest = 60;
			ImageIcon.HorizontalOptions = LayoutOptions.Start;
			ImageIcon.VerticalOptions = LayoutOptions.CenterAndExpand;

			StackLayout imageContent = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(0, 0, 10, 0),
				Children = { ImageIcon }
			};

			TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
			TextLabel.VerticalOptions = LayoutOptions.Center;
			TextLabel.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel));
			TextLabel.TextColor = Helpers.Color.Primary.ToFormsColor();
			TextLabel.IsDefaultLabel = true;

			Image rightChevron = new Image
			{
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
				Source = "chevron.png",
				//WidthRequest = 60,
				//HeightRequest = 60
			};

			DefaultLayout.HeightRequest = 80;
			DefaultLayout.Orientation = StackOrientation.Horizontal;
			DefaultLayout.Padding = new Thickness(15, 0);
			//DefaultLayout.Children.Add(new StackLayout { VerticalOptions = LayoutOptions.Center, HeightRequest = 60, WidthRequest = 60, Children = { ImageIcon } });
			DefaultLayout.Children.Add(imageContent);
			DefaultLayout.Children.Add(TextLabel);
			DefaultLayout.Children.Add(rightChevron);

			View = DefaultLayout;

            MessagingCenter.Subscribe<SettingsPage>(this, "UserImageChanged", (page) =>
            {
                try
                {
                    var myself = TalentPlusApp.CurrentUser;// await TalentDb.GetItem<User>(TalentPlusApp.CurrentUserId);
                    if (myself != null)
                    {
                        TalentPlusApp.CurrentUser = myself;
                        if (!string.IsNullOrEmpty(myself.UserImage))
                        {
                            UserImageSource = myself.UserImage;
                            UserImage.Source = UserImageSource;
                        }
                        if (!string.IsNullOrEmpty(myself.Name))
                        {
                            UserName = myself.Name;
                            UserNameLabel.Text = UserName;
                        }
                    }
                }
                catch (Exception ex)
                {
					Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "MenuCell()" }
					});
                }
            });
		}

        ~MenuCell()
        {
            MessagingCenter.Unsubscribe<SettingsPage>(this, "UserImageChanged");
        }

		protected override void OnPropertyChanged(string propertyName)
		{
			base.OnPropertyChanged(propertyName);

			switch (propertyName)
			{
				case "Text":
					ChangeCellLayout();
					break;
				case "ImageSource":
					UpdateImageIcon();
					break;
                case "TextColor":
                    TextLabel.TextColor = TextColor;
                    break;
			}
		}

		private void ChangeCellLayout()
		{
			if (Text == "Me")
			{
				UserImage.Source = UserImageSource;
				UserNameLabel.Text = UserName;

				var absoluteLayout = new AbsoluteLayout { HeightRequest = 100 };
				var image = new Image { Source = "pattern_header.png", Aspect = Aspect.AspectFill };

				var stackLayoutProfile = new StackLayout
				{
					VerticalOptions = LayoutOptions.StartAndExpand,
					Orientation = StackOrientation.Horizontal,
					Padding = new Thickness(10, 0),
					Children =
					{
						new StackLayout 
						{ 
							VerticalOptions = LayoutOptions.Center,
							Padding = new Thickness(0, 0, 10, 0),
							HeightRequest = 70,
							WidthRequest = 70,
							Children =
							{
								UserImage
							} 
						},
						UserNameLabel
					}
				};

				absoluteLayout.Children.Add(image, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
				absoluteLayout.Children.Add(stackLayoutProfile, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

				View = absoluteLayout;

				var myself = TalentPlusApp.CurrentUser;// await TalentDb.GetItem<User>(TalentPlusApp.CurrentUserId);
				if (myself != null)
				{
					TalentPlusApp.CurrentUser = myself;
					if (!string.IsNullOrEmpty(myself.UserImage))
					{
						UserImageSource = myself.UserImage;
						UserImage.Source = UserImageSource;
					}
					if (!string.IsNullOrEmpty(myself.Name))
					{
						UserName = myself.Name;
						UserNameLabel.Text = UserName;
					}
				}
			}
			else
			{
				TextLabel.Text = Text;
			}
		}

		private void UpdateUserName()
		{
			if (UserName == "Me")
				return;
			UserNameLabel.Text = UserName;
		}

		private void UpdateUserImageSource()
		{
			if (UserImageSource == "user_icon.png")
				return;
			UserImage.Source = UserImageSource;
		}

		private void UpdateImageIcon()
		{
			ImageIcon.Source = ImageSource;
		}
	}
}