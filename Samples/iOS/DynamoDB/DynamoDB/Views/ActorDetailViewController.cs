using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading.Tasks;

namespace DynamoDB
{
	public class ActorDetailViewController : BaseViewController
	{
		public ActorDetailViewController(Actor actor) : base(UITableViewStyle.Grouped,true)
		{
			Root = CreateRoot(actor);
			Title = actor.Name;
		}
		
		public ActorDetailViewController(string actorName) : base(UITableViewStyle.Grouped,true)
		{
			Title = actorName;
			ShowLoading();
			Database.GetActorAsync(actorName).ContinueWith(task=>{
				Root = CreateRoot(task.Result);
				HideLoading();
			},TaskScheduler.FromCurrentSynchronizationContext());
		}
		
		RootElement CreateRoot(Actor actor)
		{
			if(actor == null)
				return new RootElement(""){
					new Section(){
						new StringElement("No Data Available"),
					}
				};
			var section = new Section("Stats"){
				new StringElement("Name",actor.Name),
				new StringElement("Age",((int)((double)actor.Age.Days/365.2425)).ToString()),
				new StringElement("Birthday",actor.BirthDate.ToShortDateString()),
				new StringElement("Height",actor.HeightInMeters.ToString()),
			};

			var bio = new Section("Bio"){
				new MultilineElement(actor.Bio),
			};
			
			var addressSection = new Section("Address"){
				new StringElement("Street", actor.Address.Street),
				new StringElement("City",actor.Address.City),
				new StringElement("Country",actor.Address.Country),
			};

			var commentSection = new Section("Comments"){
				new MultilineElement(actor.Comment ?? ""),
			};

			return new RootElement(actor.Name){
				section,
				bio,
				addressSection,
				commentSection,

			};
		}
	}
}

