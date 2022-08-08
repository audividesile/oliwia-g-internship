using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VigilantMeerkat.Micro.Base.Config
{
    public class ConfigLoader
    {
        public ConfigDefinition Load()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            
            return JsonConvert.DeserializeObject<ConfigDefinition>(File.ReadAllText("config.json"), settings);
        }
    }
}