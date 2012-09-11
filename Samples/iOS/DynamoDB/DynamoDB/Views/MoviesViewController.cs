using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

namespace DynamoDB
{
	public class MoviesViewController : BaseViewController
	{
		public MoviesViewController () : base (UITableViewStyle.Plain,true)
		{

		}
		UIBarButtonItem loadingBtn;
		UIBarButtonItem reloadBtn;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			reloadBtn = new UIBarButtonItem(UIBarButtonSystemItem.Refresh,delegate{
				reload();
			});

			UIActivityIndicatorView spinner = new UIActivityIndicatorView (new RectangleF (0, 0, 22, 22));
			spinner.StartAnimating ();
			loadingBtn = new UIBarButtonItem (spinner);
			reload();

		}
		RootElement CreateRoot(List<Movie> movies)
		{
			Section section = new Section();
			foreach(var movie in movies)
			{
				var theMovie = movie;
				section.Add(new StringElement(movie.ToString(),delegate{
					ActivateController(new MovieDetailController(theMovie));
				}));
			}
			return new RootElement("Movies")
			{
				section
			};
		}
		void reload()
		{
			ShowLoading();
			Database.GetMoviesAsync().ContinueWith(task=>{
				Root = CreateRoot(task.Result);
				HideLoading();
			},TaskScheduler.FromCurrentSynchronizationContext());
		}
		public override void HideLoading ()
		{
			NavigationItem.RightBarButtonItem = reloadBtn;
		}

	}
}

