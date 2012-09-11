using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Drawing;

namespace DynamoDB
{
	public class MainViewController : BaseViewController
	{
		UIBarButtonItem loadingBtn;
		public MainViewController () : base ( UITableViewStyle.Grouped,false)
		{
			Root = CreateRoot();
			Title = "Dynamo DB";
		}

		RootElement CreateRoot()
		{
			Section tableSetup = new Section("Database Setup"){
				new StringElement("Create Tables",delegate{
					ShowLoading();
					Task.Factory.StartNew(delegate{
						Database.CreateSampleTables();						
					}).ContinueWith(task=>{
						HideLoading();
						new UIAlertView("","Tables Created",null,"Ok").Show();
					},TaskScheduler.FromCurrentSynchronizationContext ());
				}),
				new StringElement("Populate Movies Table",delegate{
					ShowLoading();
					Task.Factory.StartNew(delegate{
						Database.PopulateMovies();
					}).ContinueWith(task=>{
						HideLoading();
						new UIAlertView("","Movies Populated",null,"Ok").Show();
					},TaskScheduler.FromCurrentSynchronizationContext());
				}),
				new StringElement("Delete Tables",delegate{
					ShowLoading();
					Task.Factory.StartNew(delegate{
						Database.DeleteSampleTables();
					}).ContinueWith(task=>{
						HideLoading();
						new UIAlertView("","Tables Dropped",null,"Ok").Show();
					},TaskScheduler.FromCurrentSynchronizationContext());
				})
			};

			Section viewData = new Section(){
				new StringElement("Movies",delegate{
					this.ActivateController(new MoviesViewController());
				}),
				new StringElement("Actors",delegate{
					this.ActivateController(new ActorsViewController());
				}),
			};
			return new RootElement("Dynamo DB"){
				tableSetup,
				viewData,
			};
		}
	}
}

