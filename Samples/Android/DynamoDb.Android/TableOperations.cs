using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using System.Threading;

namespace DynamoDb
{
	public partial class Activity1
    {
        private static string[] SampleTables = new string[] { "Actors", "Movies", "Businesses" };

        public static void CreateSampleTables()
        {
            Console.WriteLine("Getting list of tables");
            List<string> currentTables = client.ListTables().ListTablesResult.TableNames;
            Console.WriteLine("Number of tables: " + currentTables.Count);

            bool tablesAdded = false;
            if (!currentTables.Contains("Movies"))
            {
                Console.WriteLine("Movies table does not exist, creating");
                client.CreateTable(new CreateTableRequest
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
                client.CreateTable(new CreateTableRequest
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
                client.CreateTable(new CreateTableRequest
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

                    Console.WriteLine("Sleeping for 5 seconds...");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }

            Console.WriteLine("All sample tables created");
        }

        private static string GetTableStatus(string tableName)
        {
            try
            {
                var table = client.DescribeTable(new DescribeTableRequest { TableName = tableName }).DescribeTableResult.Table;
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
                client.DeleteTable(new DeleteTableRequest { TableName = table });
            }

            while(true)
            {
                Console.WriteLine("Getting list of tables");
                var currentTables = client.ListTables().ListTablesResult.TableNames;
                if (currentTables.Intersect(SampleTables).Count() == 0)
                    break;

                Console.WriteLine("Sleeping for 5 seconds...");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            };

            Console.WriteLine("Sample tables deleted");
        }
    }
}
