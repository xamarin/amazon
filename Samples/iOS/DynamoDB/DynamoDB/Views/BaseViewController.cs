using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;

namespace DynamoDB
{
	public class BaseViewController : DialogViewController
	{
		public BaseViewController (UITableViewStyle style, bool pushing) : base(style , null ,pushing)
		{

		}
		public override void ViewDidLoad ()
		{
			UIActivityIndicatorView spinner = new UIActivityIndicatorView (new RectangleF (0, 0, 22, 22));
			spinner.StartAnimating ();
			loadingBtn = new UIBarButtonItem (spinner);
		}

		
		UIBarButtonItem loadingBtn;
		public virtual void ShowLoading()
		{
			NavigationItem.RightBarButtonItem = loadingBtn;		
		}
		public virtual void HideLoading()
		{
			NavigationItem.RightBarButtonItem = null;
		}
	}
}

