using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.DynamoDB.DataModel;
using Amazon.DynamoDB.DocumentModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace DynamoDb
{
    [DynamoDBTable("Movies")]
    public class Movie
    {
        [DynamoDBHashKey]
        public string Title { get; set; }
        [DynamoDBRangeKey(AttributeName="Released")]
        public DateTime ReleaseDate { get; set; }

        public List<string> Genres { get; set; }
        [DynamoDBProperty("Actors")]
        public List<string> ActorNames { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} - {1}
Actors: {2}", Title, ReleaseDate, string.Join(", ", ActorNames.ToArray()));
        }
    }

    [DynamoDBTable("Actors")]
    public class Actor
    {
        [DynamoDBHashKey]
        public string Name { get; set; }

        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }

        [DynamoDBProperty(AttributeName = "Height")]
        public float HeightInMeters { get; set; }

        [DynamoDBProperty(Converter=typeof(AddressConverter))]
        public Address Address { get; set; }

        [DynamoDBIgnore]
        public string Comment { get; set; }

        public TimeSpan Age
        {
            get
            {
                return DateTime.UtcNow - BirthDate.ToUniversalTime();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, BirthDate);
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class AddressConverter : IPropertyConverter
    {
        private XmlSerializer _serializer = new XmlSerializer(typeof(Address));

        #region IPropertyConverter Members

        public object FromEntry(DynamoDBEntry entry)
        {
			Primitive primitive = entry as Primitive;
			if (primitive == null) return null;

			if (primitive.Type == DynamoDBEntryType.Numeric) throw new InvalidCastException();
			string xml = primitive.Value.ToString();
			using (StringReader reader = new StringReader(xml))
			{
				return _serializer.Deserialize(reader);
			}
        }

        public DynamoDBEntry ToEntry(object value)
        {
            Address address = value as Address;
            if (address == null) return null;

            string xml;
            using (StringWriter stringWriter = new StringWriter())
            {
                _serializer.Serialize(stringWriter, address);
                xml = stringWriter.ToString();
            }
            return new Primitive(xml);
        }

        #endregion
    }
}
