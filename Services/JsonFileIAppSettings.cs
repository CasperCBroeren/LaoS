using System;
using LaoS.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LaoS.Services
{
    public class JsonFileIAppSettings : IAppSettings
    {
        private Dictionary<string, string> settings;

        public JsonFileIAppSettings()
        {
            this.settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("appsettings.json"));

        }
        public string Get(string name)
        {
            return this.settings[name];
        }
    }
}
