using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;

namespace TalentPlus.Shared.Helpers
{
	public class RatingBarControl : StackLayout
	{
		#if __IOS__
        const float containerWidth = 300;

#else
		const float containerWidth = 280;
		#endif

		#region PRIVATE MEMBERS

		const int RATING_IMAGE_COUNT = 5;
		DarkIceImage[] Rating_Image_List = new DarkIceImage[RATING_IMAGE_COUNT];
		//SvgColorIcon[] Rating_Image_List = new SvgColorIcon[RATING_IMAGE_COUNT];

		public static readonly BindableProperty RatingBarProperty =
			BindableProperty.Create<RatingBarControl, int> (
				p => p.Rating, 0);


		public int Rating {
			get {
				return (int)GetValue (RatingBarProperty);
			}

			set {
				this.SetValue (RatingBarProperty, value);
			}
		}

		public bool IsChangeable { get; set; }

		public bool NeverChangeable { get; set; }

		public int ImageSize { get; set; }

		private int LastRatingValue;

		private bool IsFullWidth { get; set; }

		private Grid FullWidthGrid;

		#endregion

		public RatingBarControl (int DefaultSize = 40, bool IsChangeable = true, int Rating = 0, int Spacing = 10, bool isFullWidth = false)
		{
			if (IsChangeable) {
				this.HorizontalOptions = LayoutOptions.CenterAndExpand;
			} else {
				this.HorizontalOptions = LayoutOptions.StartAndExpand;
			}
			this.HeightRequest = DefaultSize;
			this.Spacing = Spacing;
			this.Padding = 0;
			this.IsChangeable = IsChangeable;
			this.NeverChangeable = !IsChangeable;
			this.Rating = Rating;
			this.IsFullWidth = isFullWidth;

			LastRatingValue = Rating;
			ImageSize = DefaultSize;

			if (IsFullWidth) {
				FullWidthGrid = new Grid {
					HorizontalOptions = LayoutOptions.FillAndExpand,
					ColumnDefinitions = {
						new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) }
					}
				};

				this.Children.Add (FullWidthGrid);
			}

			Initialize ();

            MessagingCenter.Subscribe<string> (string.Empty, "ReloadActionBar", (title) => 
                {   
                    this.UpdateRatingColor();
                });
		}

        ~RatingBarControl()
        {
            MessagingCenter.Unsubscribe<string> (string.Empty, "ReloadActionBar");
        }
		public void Initialize ()
		{
			var assembly = typeof(TalentPlusApp).GetTypeInfo ().Assembly;
			for (int i = 0; i < Rating; i++) {
				Rating_Image_List[i] = new DarkIceImage
				{
				    Source = ImageSource.FromFile("star_icon.png"),
				    WidthRequest = ImageSize,
				    HeightRequest = ImageSize,
					FilterColor = Helpers.Color.Secondary.ToFormsColor(),
				    Tag = i,
				};

                Rating_Image_List[i].TappedWithId += (String id) =>
                {
                    int idx = int.Parse(id);
                    RatingImage_Clicked(idx);
                };

//				Rating_Image_List [i] = new SvgColorIcon {
//					SvgPath = "star_icon.svg",
//					SvgAssembly = assembly,
//					HorizontalOptions = LayoutOptions.Center,
//					VerticalOptions = LayoutOptions.Center,
//					BackgroundColor = Xamarin.Forms.Color.Transparent,
//					HeightRequest = ImageSize,
//					WidthRequest = ImageSize,
//					IconColor = Helpers.Color.Secondary.ToIntColor (),
//					Tag = i
//				};
//
//				Rating_Image_List [i].TappedWithId += (String id) => {
//					int idx = int.Parse (id);
//					RatingImage_Clicked (idx);
//				};

				if (IsFullWidth) {
					Rating_Image_List [i].HorizontalOptions = LayoutOptions.CenterAndExpand;
					FullWidthGrid.Children.Add (Rating_Image_List [i], i, 0);
				} else {
					this.Children.Add (Rating_Image_List [i]);
				}
			}
			if (IsChangeable) {
				for (int i = Rating; i < RATING_IMAGE_COUNT; i++) {
//					Rating_Image_List [i] = new SvgColorIcon {
//						SvgPath = "star_icon.svg",
//						SvgAssembly = assembly,
//						HorizontalOptions = LayoutOptions.Center,
//						VerticalOptions = LayoutOptions.Center,
//						BackgroundColor = Xamarin.Forms.Color.Transparent,
//						HeightRequest = ImageSize,
//						WidthRequest = ImageSize,
//						IconColor = Helpers.Color.Gray.ToIntColor (),
//						SecondIconColor = Helpers.Color.Secondary.ToIntColor (),
//						Tag = i
//					};

					Rating_Image_List[i] = new DarkIceImage
					{
					    //Source = ImageSource.FromFile("star_nofilled.png"),
						Source = ImageSource.FromFile("star_icon.png"),
						FilterColor = Helpers.Color.Gray.ToFormsColor(),
					    WidthRequest = ImageSize,
					    HeightRequest = ImageSize,
					    Tag = i,
					};

					Rating_Image_List [i].TappedWithId += (String id) => {
						int idx = int.Parse (id);
						RatingImage_Clicked (idx);
					};

					if (IsFullWidth) {
						Rating_Image_List [i].HorizontalOptions = LayoutOptions.CenterAndExpand;
						FullWidthGrid.Children.Add (Rating_Image_List [i], i, 0);
					} else {
						this.Children.Add (Rating_Image_List [i]);
					}
				}
			}
		}

		public void UpdateRating(int newRating)
		{
			UpdateImagesFromIdx(newRating - 1);
			this.UpdateRatingColor();
		}

        public void UpdateRatingColor()
        {
            if (Rating_Image_List != null && Rating_Image_List.Length > 0)
            {
                for (int i = 0; i < Rating; i++)
                {
                    Rating_Image_List[i].FilterColor = Helpers.Color.Secondary.ToFormsColor();
                }
            }
        }

		#region IMAGE_EVENTS

		private void RatingImage_Clicked (int idx)
		{
			if (IsChangeable == false)
				return;
			IsChangeable = false; //prevent from clicking multiple times

			UpdateImagesFromIdx(idx);

			OnStarRated (EventArgs.Empty);
		}

		private void UpdateImagesFromIdx(int idx)
		{
			Rating = idx + 1;

			int lastIdx = LastRatingValue - 1;
			//if (LastRatingValue == 0)
			//	lastIdx = 1;
			//if (lastIdx < 0)
			//	lastIdx = 0;
			if (lastIdx >= RATING_IMAGE_COUNT)
				lastIdx = RATING_IMAGE_COUNT - 1;

			if (idx > lastIdx)
			{
				for (int i = lastIdx; i <= idx; i++)
				{
					if (i >= 0)
					{
						//Rating_Image_List [i].Source = ImageSource.FromFile ("star_filled.png");
						if (Rating_Image_List[i] == null)
						{
							Rating_Image_List[i] = new DarkIceImage
							{
								Source = ImageSource.FromFile("star_icon.png"),
								WidthRequest = ImageSize,
								HeightRequest = ImageSize,
								FilterColor = Helpers.Color.Secondary.ToFormsColor(),
								Tag = i,
							};
							if (IsFullWidth)
							{
								Rating_Image_List[i].HorizontalOptions = LayoutOptions.CenterAndExpand;
								FullWidthGrid.Children.Add(Rating_Image_List[i], i, 0);
							}
							else
							{
								this.Children.Add(Rating_Image_List[i]);
							}
						}
						Rating_Image_List[i].FilterColor = Color.Secondary.ToFormsColor();
					}
				}
			}
			else if (idx < lastIdx)
			{
				for (int i = idx + 1; i <= lastIdx; i++)
				{
					if (i >= 0)
					{
						if (NeverChangeable)
						{
							Rating_Image_List[i].FilterColor = Color.White.ToFormsColor();
						}
						else
						{
							Rating_Image_List[i].FilterColor = Color.Gray.ToFormsColor();
						}
						//Rating_Image_List [i].Source = ImageSource.FromFile ("star_nofilled.png");
					}
				}
			}

			LastRatingValue = Rating;
		}

		#endregion

		protected virtual void OnStarRated (EventArgs e)
		{
			EventHandler handler = StarRated;
			if (handler != null) {
				handler (this, e);
			}
		}

		public event EventHandler StarRated;
	}
}
