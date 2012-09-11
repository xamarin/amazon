using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using System.Threading;
using Amazon;
using Amazon.DynamoDB.DataModel;
using Amazon.DynamoDB.DocumentModel;
using System.Threading.Tasks;

namespace DynamoDB
{
	public class Database
	{
		private static string[] SampleTables = new string[] { "Actors", "Movies", "Businesses" };
		static AmazonDynamoDB client;
		public static AmazonDynamoDB Client{
			get{
				try{
				if(client == null)
					client = new AmazonDynamoDBClient(RegionEndpoint.USWest2);	
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				return client;
			}
		}

		static DynamoDBContext context;
		public static DynamoDBContext Context{
			get{
				try{
					if(context == null)
						context = new DynamoDBContext(Database.Client);		
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
				}
				return context;
			}
		}


		public static void CreateSampleTables()
		{
			Console.WriteLine("Getting list of tables");
			List<string> currentTables = Client.ListTables().ListTablesResult.TableNames;
			Console.WriteLine("Number of tables: " + currentTables.Count);
			
			bool tablesAdded = false;
			if (!currentTables.Contains("Movies"))
			{
				Console.WriteLine("Movies table does not exist, creating");
				Client.CreateTable(new CreateTableRequest
				                   {
					TableName = "Movies",
					ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
					KeySchema = new KeySchema
					{
						HashKeyElement = new KeySchemaElement { AttributeName = "Title", AttributeType = "S" },
						RangeKeyElement = new KeySchemaElement { AttributeName = "Released", AttributeType = "S" }
					}
				});
				tablesAdded = true;
			}
			if (!currentTables.Contains("Actors"))
			{
				Console.WriteLine("Actors table does not exist, creating");
				Client.CreateTable(new CreateTableRequest
				                   {
					TableName = "Actors",
					ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
					KeySchema = new KeySchema
					{
						HashKeyElement = new KeySchemaElement { AttributeName = "Name", AttributeType = "S" }
					}
				});
				tablesAdded = true;
			}
			if (!currentTables.Contains("Businesses"))
			{
				Console.WriteLine("Businesses table does not exist, creating");
				Client.CreateTable(new CreateTableRequest
				                   {
					TableName = "Businesses",
					ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 10, WriteCapacityUnits = 10 },
					KeySchema = new KeySchema
					{
						HashKeyElement = new KeySchemaElement { AttributeName = "Name", AttributeType = "S" },
						RangeKeyElement = new KeySchemaElement { AttributeName = "Id", AttributeType = "N" }
					}
				});
				tablesAdded = true;
			}
			
			if (tablesAdded)
			{
				while(true)
				{
					bool allActive = true;
					foreach (var table in SampleTables)
					{
						string tableStatus = GetTableStatus(table);
						bool isTableActive = string.Equals(tableStatus, "ACTIVE", StringComparison.OrdinalIgnoreCase);
						if (!isTableActive)
							allActive = false;
					}
					if (allActive)
					{
						Console.WriteLine("All tables are ACTIVE");
						break;
					}
					
					//Console.WriteLine("Sleeping for 5 seconds...");
					//Thread.Sleep(TimeSpan.FromSeconds(5));
				}
			}
			
			Console.WriteLine("All sample tables created");
		}
		
		private static string GetTableStatus(string tableName)
		{
			try
			{
				var table = Client.DescribeTable(new DescribeTableRequest { TableName = tableName }).DescribeTableResult.Table;
				return (table == null) ? string.Empty : table.TableStatus;
			}
			catch (AmazonDynamoDBException db)
			{
				if (db.ErrorCode == "ResourceNotFoundException")
					return string.Empty;
				throw;
			}
		}
		
		public static void DeleteSampleTables()
		{
			foreach (var table in SampleTables)
			{
				Console.WriteLine("Deleting table " + table);
				Client.DeleteTable(new DeleteTableRequest { TableName = table });
			}
			
			while(true)
			{
				Console.WriteLine("Getting list of tables");
				var currentTables = Client.ListTables().ListTablesResult.TableNames;
				if (currentTables.Intersect(SampleTables).Count() == 0)
					break;
				
				Console.WriteLine("Sleeping for 5 seconds...");
				Thread.Sleep(TimeSpan.FromSeconds(5));
			};
			
			Console.WriteLine("Sample tables deleted");
		}		


		public static void PopulateMovies()
		{
			Console.WriteLine("Creating the context object");
			DynamoDBContext context = new DynamoDBContext(Database.Client);
			
			Console.WriteLine("Creating actors");
			Actor christianBale = new Actor
			{
				Name = "Christian Bale",
				Bio = "Christian Charles Philip Bale is an excellent horseman and an avid reader.",
				BirthDate = new DateTime(1974, 1, 30),
				Address = new Address
				{
					City = "Los Angeles",
					Country = "USA"
				},
				HeightInMeters = 1.83f
			};
			Actor michaelCaine = new Actor
			{
				Name = "Michael Caine",
				Bio = "Maurice Joseph Micklewhite is an English actor, better known as Michael Caine",
				BirthDate = new DateTime(1933, 3, 14),
				Address = new Address
				{
					City = "London",
					Country = "England"
				},
				HeightInMeters = 1.88f
			};
			
			Console.WriteLine("Creating movie");
			Movie darkKnight = new Movie
			{
				Title = "The Dark Knight",
				ReleaseDate = new DateTime(2008, 7, 18),
				Genres = new List<string> { "Action", "Crime", "Drama" },
				ActorNames = new List<string>
				{
					christianBale.Name,
					michaelCaine.Name
				}
			};
			
			Console.WriteLine("Saving actors and movie");
			context.Save<Actor>(michaelCaine);
			context.Save<Actor>(christianBale);
			context.Save<Movie>(darkKnight);
			
			Console.WriteLine("Creating and saving new actor");
			Actor maggieGyllenhaal = new Actor
			{
				Name = "Maggie Gyllenhaal",
				BirthDate = new DateTime(1977, 11, 16),
				Bio = "Maggie Gyllenhaal studied briefly at the Royal Academy of Dramatic Arts in London.",
				Address = new Address
				{
					City = "New York City",
					Country = "USA"
				},
				HeightInMeters = 1.75f
			};
			context.Save<Actor>(maggieGyllenhaal);
			
			Console.WriteLine();
			Console.WriteLine("Loading existing movie");
			Movie existingMovie = context.Load<Movie>("The Dark Knight", new DateTime(2008, 7, 18));
			Console.WriteLine(existingMovie.ToString());
			
			Console.WriteLine();
			Console.WriteLine("Loading nonexistent movie");
			Movie nonexistentMovie = context.Load<Movie>("The Dark Knight", new DateTime(2008, 7, 19));
			Console.WriteLine("Movie is null : " + (nonexistentMovie == null));
			
			Console.WriteLine("Updating movie and saving");
			existingMovie.ActorNames.Add(maggieGyllenhaal.Name);
			existingMovie.Genres.Add("Thriller");
			context.Save<Movie>(existingMovie);
			
			Console.WriteLine("Adding movie with same hash key but different range key");
			Movie darkKnight89 = new Movie
			{
				Title = "The Dark Knight",
				Genres = new List<string> { "Drama" },
				ReleaseDate = new DateTime(1989, 2, 23),
				ActorNames = new List<string>
				{
					"Juan Diego",
					"Fernando Guillén",
					"Manuel de Blas"
				}
			};
			context.Save<Movie>(darkKnight89);

		}	

		public static Task<List<Movie>> GetMoviesAsync ()
		{
			return Task.Factory.StartNew (delegate {
				return GetMovies ();
			});
		} 
		public static List<Movie> GetMovies()
		{		
			return Context.Scan<Movie>().ToList();
		}
		public static Task<List<Actor>> GetActorsAsync ()
		{
			return Task.Factory.StartNew (delegate {
				return GetActors ();
			});
		} 
		public static List<Actor> GetActors()
		{		
			return Context.Scan<Actor>().ToList();
		}
		public static Task<Actor> GetActorAsync (string actorName)
		{
			return Task.Factory.StartNew (delegate {
				var actor =  GetActor (actorName);
				return actor;
			});
		} 
		public static Actor GetActor(string name)
		{
			return Context.Load<Actor>(name);
			
		}
			
	}
}

