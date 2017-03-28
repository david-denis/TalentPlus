using CaveBirdLabs.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    public class ActivityYesView : CarouselPage//CxPagedCarouselPage//CarouselPage
    {
		//private ActivityYesViewModel ViewModel
		//{
		//	get { return BindingContext as ActivityYesViewModel; }
		//}

		//private List<ActivityDetailsView> ActivitiesDetailsView = new List<ActivityDetailsView>();

		/// <summary>
		/// Page when no activities are available
		/// </summary>
//		private ContentPage NoActivitiesPage = new ContentPage();

		public ActivityYesView(Activity activity)
		{
			this.Title = activity.ShortDescription;

			BindingContext = activity;
		}
        
        public void MoveToNextPage(int curPage)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.CurrentPage = this.Children[curPage];

                if (curPage == 2)
                {
                    curPage = 0;
                }
                else
                {
                    curPage++;
                }
            });            
        }
    }
}
