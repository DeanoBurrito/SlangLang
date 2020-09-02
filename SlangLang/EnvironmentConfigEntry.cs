using System;

namespace SlangLang
{
    public sealed class EnvironmentConfigEntry
    {
        public readonly string key;
        public object value;
        public string strValue;
        public bool isReadOnly;

        public EnvironmentConfigEntry(string key, string stringValue, object castValue, bool readOnly)
        {
            this.key = key;
            value =  castValue;
            strValue = stringValue;
            isReadOnly = readOnly;
        }
    }
}