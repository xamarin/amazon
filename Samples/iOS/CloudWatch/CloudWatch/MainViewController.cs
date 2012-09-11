using System;
using MonoTouch.UIKit;
using System.Drawing;
using Amazon.CloudWatch;
using Amazon;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.CloudWatch.Model;
using MonoTouch.Foundation;

namespace CloudWatch
{
	public class MainViewController : UITableViewController
	{
		public MainViewController ()
		{
		}

		UIBarButtonItem addBtn;
		UIBarButtonItem	refreshBtn;
		UIBarButtonItem loadingBtn;
		AmazonCloudWatch client;
		TableViewSource Source;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			client = Amazon.AWSClientFactory.CreateAmazonCloudWatchClient (RegionEndpoint.USWest2);				

			this.Title = "Cloud Watch";
			Source = new TableViewSource();
			TableView.Source = Source;
			addBtn = new UIBarButtonItem (UIBarButtonSystemItem.Add, delegate {
				AddItem();
			});
			refreshBtn = new UIBarButtonItem (UIBarButtonSystemItem.Refresh, delegate {
				Refresh();
			});

			UIActivityIndicatorView spinner = new UIActivityIndicatorView (new RectangleF (0, 0, 22, 22));
			spinner.StartAnimating ();
			loadingBtn = new UIBarButtonItem (spinner);
			ReloadUI();
			Refresh();
		}

		private void AddItem ()
		{
			addBtn.Enabled = false;
			refreshBtn.Enabled = false;
			this.NavigationItem.RightBarButtonItem = loadingBtn;
			Task.Factory.StartNew (delegate {
				Random rand = new Random ();
				IList<MetricDatum> data = new List<MetricDatum> ();
				
				data.Add (new MetricDatum ()
				         .WithMetricName ("PagingFilePctUsage")
				         .WithTimestamp (DateTime.Now)
				         .WithUnit ("Percent")
				         .WithValue (rand.NextDouble ()));
				
				data.Add (new MetricDatum ()
				         .WithMetricName ("PagingFilePctUsagePeak")
				         .WithTimestamp (DateTime.Now)
				         .WithUnit ("Percent")
				         .WithValue (rand.NextDouble ()));
				
				
				var response = client.PutMetricData (new PutMetricDataRequest ()
				                     .WithMetricData (data)
				                     .WithNamespace ("Xamarin/MonoTouch"));				
				loadData();
			}).ContinueWith (task => {
				ReloadUI ();
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		private void Refresh ()
		{
			addBtn.Enabled = false;
			refreshBtn.Enabled = false;
			this.NavigationItem.LeftBarButtonItem = loadingBtn;	
			Task.Factory.StartNew (delegate {			
				loadData();
			}).ContinueWith(task=>{
				ReloadUI();
			}, TaskScheduler.FromCurrentSynchronizationContext ());

		}

		private void ReloadUI ()
		{
			addBtn.Enabled = true;
			refreshBtn.Enabled = true;
			this.NavigationItem.RightBarButtonItem = addBtn;
			this.NavigationItem.LeftBarButtonItem = refreshBtn;	
			TableView.ReloadData();

		}
		private void loadData()
		{
			Source.Metrics = client.ListMetrics ().ListMetricsResult.Metrics;
			Console.WriteLine(Source.Metrics.Count);
		}

		private class TableViewSource: UITableViewSource
		{
			public List<Metric> Metrics = new List<Metric>();
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			public override int RowsInSection (UITableView tableview, int section)
			{
				return Metrics.Count;
			}
			NSString key = new NSString("metricCell");
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(key);
				if(cell == null)
				{
					cell = new UITableViewCell( UITableViewCellStyle.Subtitle,key);
				}
				var metric = Metrics[indexPath.Row];
				cell.TextLabel.Text = metric.MetricName;
				cell.DetailTextLabel.Text = metric.Namespace;
				return cell;
			}
		}
	}
}

