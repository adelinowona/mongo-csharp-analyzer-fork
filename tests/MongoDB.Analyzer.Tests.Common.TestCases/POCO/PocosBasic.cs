using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Analyzer.Tests.Common.TestCases.POCO;

public sealed class PocosBasic : TestCasesBase
{

    [ExpectedJson("{ \"province\" : \"Province\", \"zip\" : \"ZipCode\" }")]
    public class BasicPoco
    {
        [BsonIgnore]
        public string City { get; set; }

        [BsonElement("province")]
        public string Province { get; set; }

        [BsonElement("zip")]
        public string ZipCode { get; set; }
    }
}