﻿using System.IO;
using gr0ssSysTools.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gr0ssSysTools.FileUtils
{
    public class RegistryKeyUtils
    {
        private const string REGISTRYKEY_FILE_NAME = "registrykey.json";

        public static void WriteRegistryKeySettings(MonitoredRegistryKey monitoredRegistryKey, bool firstLoad = false)
        {
            string environmnentJsonFile = Path.Combine(Directory.GetCurrentDirectory(), REGISTRYKEY_FILE_NAME);
            
            using (StreamWriter file = File.CreateText(environmnentJsonFile))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                var jsonRegistryKey = JsonConvert.SerializeObject(firstLoad ? GetInitialRegistryKey() : monitoredRegistryKey);
                writer.WriteRaw(jsonRegistryKey);
            }
        }

        public static MonitoredRegistryKey ReadRegistryKeySettings()
        {
            string registryKeyJsonFile = Path.Combine(Directory.GetCurrentDirectory(), REGISTRYKEY_FILE_NAME);

            var registryKey = new MonitoredRegistryKey();

            if (!File.Exists(registryKeyJsonFile))
            {
                registryKey = GetInitialRegistryKey();
                WriteRegistryKeySettings(registryKey);
            }
            else
            {
                using (StreamReader file = File.OpenText(registryKeyJsonFile))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    while (reader.Read())
                    {
                        JObject o3 = (JObject) JToken.ReadFrom(reader);
                        foreach (var child in o3.Children())
                        {
                            AddPropertyToRegistryKey(registryKey, child.Path, child.First.ToString());
                        }
                    }
                }
            }
            return registryKey;
        }

        private static MonitoredRegistryKey AddPropertyToRegistryKey(MonitoredRegistryKey monitoredRegistryKey, string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case Constants.RegistryKey.ROOT:
                    monitoredRegistryKey.Root = propertyValue;
                    break;
                case Constants.RegistryKey.SUBKEY:
                    monitoredRegistryKey.Subkey = propertyValue;
                    break;
            }
            return monitoredRegistryKey;
        }

        private static MonitoredRegistryKey GetInitialRegistryKey()
        {
            return new MonitoredRegistryKey {Root = "", Subkey = ""};
        } 
    }
}
