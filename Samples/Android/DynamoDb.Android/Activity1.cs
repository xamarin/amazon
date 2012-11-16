using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Amazon.DynamoDB;
using Amazon;
using Android;

namespace DynamoDb
{
	[Activity (Label = "DynamoDb.Android", MainLauncher = true)]
	public partial class Activity1 : Activity
	{
		int count = 1;
		public static AmazonDynamoDB client;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Android.Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Android.Resource.Id.myButton);
			
			button.Click += delegate {
				Console.WriteLine();
				Console.WriteLine("Setting up DynamoDB client");
				client = new AmazonDynamoDBClient(RegionEndpoint.USWest2);

				Console.WriteLine();
				Console.WriteLine("Creating sample tables");
				CreateSampleTables();

				Console.WriteLine();
				Console.WriteLine("Running DataModel sample");
				RunDataModelSample();

				Console.WriteLine();
				Console.WriteLine("Running DataModel sample");
				RunDocumentModelSample();

				Console.WriteLine();
				Console.WriteLine("Removing sample tables");
				DeleteSampleTables();

				Console.WriteLine();
			};
		}
	}
}


