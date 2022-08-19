using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDB.Analyzer.Helpers.DataModels
{
    public static class JsonGenerator
    {
        private sealed class TemplateType
        {
            public int Field { get; set; }
        }

        public static string GetDriverVersion() => typeof(Builders<>).Assembly.GetName().Version.ToString(3);

        private static void populateMembers(object obj)
        {
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                if(propertyInfo.PropertyType.Name == "String")
                {
                    propertyInfo.SetValue(obj, propertyInfo.Name);
                }
            }
        }

        public static string GetJSON()
        {
            var instance = new TemplateType();
            populateMembers(instance);
            var json = instance.ToJson();
            return json;
        }
    }
}