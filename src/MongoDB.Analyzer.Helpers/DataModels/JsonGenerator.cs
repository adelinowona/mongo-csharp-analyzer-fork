using MongoDB.Driver;

namespace MongoDB.Analyzer.Helpers.DataModels
{
    public static class JsonGenerator
    {
        private sealed class MqlGeneratorTemplateType
        {
            public int Field { get; set; }
        }

        public static string GetDriverVersion() => typeof(Builders<>).Assembly.GetName().Version.ToString(3);

        public static string GetJSON()
        {
            var buildersDefinition = Builders<MqlGeneratorTemplateType>.Filter.Gt(p => p.Field, 10);
            var className = "jsongenerator";
            return className;
        }
    }
}