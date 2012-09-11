using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoDB
{
	public class ActorsViewController : BaseViewController
	{
		UIBarButtonItem reloadBtn;
		public ActorsViewController() : base(UITableViewStyle.Plain,true)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			reloadBtn = new UIBarButtonItem(UIBarButtonSystemItem.Refresh,delegate{
				reload();
			});
			reload();
		}

		RootElement CreateRoot(List<Actor> actors)
		{
			var section = new Section();
			foreach(var actor in actors)
			{
				var theActor = actor;
				section.Add(new StringElement(theActor.Name,delegate{
					ActivateController(new ActorDetailViewController(theActor));
				}));
			}
			return new RootElement("Actors"){
				section
			};
		}

		void reload()
		{
			ShowLoading();
			Database.GetActorsAsync().ContinueWith(task=>{
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

