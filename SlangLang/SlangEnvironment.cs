using System;

namespace SlangLang
{
    public static class SlangEnvironment
    {
        public static Version GetEnvironmentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static Version GetInterpreterVersion()
        {
            return new Version(0, 0, 3);
        }
    }
}