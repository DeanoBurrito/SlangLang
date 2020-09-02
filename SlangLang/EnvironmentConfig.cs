using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SlangLang.Tests")]
namespace SlangLang
{
    public sealed class EnvironmentConfig
    {
        public static readonly EnvironmentConfig Default = new EnvironmentConfig(DefaultLoadOrder);
        
        public static readonly EnvironmentConfigSource[] DefaultLoadOrder = new EnvironmentConfigSource[] 
        {
            EnvironmentConfigSource.GlobalFile,
            EnvironmentConfigSource.EnvironmentVariables,
            EnvironmentConfigSource.LocalFile,
            EnvironmentConfigSource.CommandLineArguments,
        };
        
        private readonly Dictionary<string, EnvironmentConfigEntry> entries;
        internal static string localDirectory = ".";
        internal static string globalDirectory = ".";
        
        public EnvironmentConfig(EnvironmentConfigSource[] loadOrder)
        {
            entries = new Dictionary<string, EnvironmentConfigEntry>();
            foreach (EnvironmentConfigSource src in loadOrder)
            {
                switch (src)
                {
                    case EnvironmentConfigSource.CommandLineArguments:
                        PopulateFromCommandLine();
                        break;
                    case EnvironmentConfigSource.EnvironmentVariables:
                        PopulateFromEnvironmentVars();
                        break;
                    case EnvironmentConfigSource.GlobalFile:
                        PopulateFromGlobalFile();
                        break;
                    case EnvironmentConfigSource.LocalFile:
                        PopulateFromLocalFile();
                        break;
                }
            }
        }

        public bool IsDefined(string key)
        {
            return entries.ContainsKey(key);
        }

        public bool IsReadOnly(string key)
        {
            if (entries.ContainsKey(key))
                return entries[key].isReadOnly;
            return true;
        }

        public bool Write(string key, string value)
        {
            return false;
        }

        public string Get(string key)
        {
            if (entries.ContainsKey(key))
                return entries[key].strValue;
            return string.Empty;
        }

        public T Get<T>(string key)
        {
            if (entries.ContainsKey(key) && entries[key].value.GetType() == typeof(T))
                return (T)entries[key].value;
            return default(T);
        }

        public int Count()
        {
            return entries.Count;
        }

        private void PopulateFromCommandLine()
        {
            string[] clParts = Environment.CommandLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            List<string> keys = new List<string>(clParts.Length);
            List<string> values = new List<string>(clParts.Length);
            List<bool> readOnlyFlags = new List<bool>(clParts.Length);

            foreach (string s in clParts)
            {
                string[] sParts = s.Split('=');
                if (s.Contains('=') && sParts.Length == 2)
                {
                    if (sParts[0].EndsWith("_ro"))
                    {
                        keys.Add(sParts[0].Remove(sParts[0].Length - 3));
                        values.Add(sParts[1]);
                        readOnlyFlags.Add(true);
                    }
                    keys.Add(sParts[0]); 
                    values.Add(sParts[1]);
                    readOnlyFlags.Add(false);
                }
            }
            PopulateFrom(keys.ToArray(), values.ToArray(), readOnlyFlags.ToArray());
        }

        private void PopulateFromEnvironmentVars()
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            List<bool> readOnlyFlags = new List<bool>();
            
            foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                string key = (string)entry.Key;
                string value = (string)entry.Value;
                bool readOnly = false;
                if (key.EndsWith("_ro"))
                {
                    readOnly = true;
                    key = key.Remove(key.Length - 3);
                }
                keys.Add(key);
                values.Add(value);
                readOnlyFlags.Add(readOnly);
            }
            PopulateFrom(keys.ToArray(), values.ToArray(), readOnlyFlags.ToArray());
        }

        private void PopulateFromGlobalFile()
        {
            
        }

        private void PopulateFromLocalFile()
        {
            if (File.Exists("./SlangEnvironment.json"))
                PopulateFromFile("./SlangEnvironment.json");
        }

        private void PopulateFromFile(string path)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            List<bool> readOnlyFlags = new List<bool>();
            JsonDocument jdoc = JsonDocument.Parse(File.ReadAllText(path));
            if (!jdoc.RootElement.TryGetProperty("slangEnvironmentConfig", out JsonElement configRoot))
                return;
            foreach (JsonElement configElement in configRoot.EnumerateArray())
            {
                if (!configElement.TryGetProperty("key", out JsonElement keyElement))
                    continue;
                if (!configElement.TryGetProperty("value", out JsonElement valueElement))
                    continue;
                bool readOnly = false;
                if (configElement.TryGetProperty("readonly", out JsonElement readonlyElement))
                    readOnly = readonlyElement.GetBoolean();
                string key = keyElement.GetString();
                if (key.EndsWith("_ro"))
                {
                    readOnly = true;
                    key = key.Remove(key.Length - 3);
                }
                keys.Add(key);
                values.Add(valueElement.GetString());
                readOnlyFlags.Add(readOnly);
            }
            PopulateFrom(keys.ToArray(), values.ToArray(), readOnlyFlags.ToArray());
        }

        private void PopulateFrom(string[] keys, string[] values, bool[] readOnlyFlags)
        {
            if (keys.Length != values.Length || keys.Length != readOnlyFlags.Length)
            {
                throw new Exception("Key/Value/ReadOnlyFlag array length mismatch in EnvironmentConfig population.");
            }

            for (int i = 0; i < keys.Length; i++)
            {
                if (entries.ContainsKey(keys[i]))
                {
                    if (entries[keys[i]].isReadOnly)
                        continue;
                }
                else
                {
                    entries.Add(keys[i], null);
                }
                entries[keys[i]] = new EnvironmentConfigEntry(keys[i], values[i], values[i], readOnlyFlags[i]);
            }
        }
    }
}