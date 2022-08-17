namespace MongoDB.Analyzer.Tests.Common.TestCases.POCO;

public sealed class PocosBasic : TestCasesBase
{
    [ExpectedJson("jsongenerator")]
    public class BasicPoco
    {
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
    }

    [ExpectedJson("jsongenerator")]
    public class BasicPoco2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
    }
}