using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace DynamoDB
{
	public class MovieDetailController : BaseViewController
	{
		Movie Movie;
		public MovieDetailController (Movie movie) : base (UITableViewStyle.Grouped,true)
		{
			this.Title = movie.Title;
			Movie = movie;
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Root = CreateRoot(Movie);
		}

		RootElement CreateRoot (Movie movie)
		{
			var topSection = new Section("Details"){
				new StringElement("Title",movie.Title),
				new StringElement("Release Date", movie.ReleaseDate.ToShortDateString()),
			};

			var actorSection = new Section("Actors");
			foreach(var actor in movie.ActorNames)
			{
				var theActor = actor;
				actorSection.Add(new StringElement(actor,delegate{
					ActivateController(new ActorDetailViewController(theActor));
				}));
			}
			return new RootElement(movie.Title){
				topSection,
				actorSection,
			};
		}
	}
}

