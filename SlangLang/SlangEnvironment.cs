using System;
using System.Linq;
using System.Reflection;

namespace SlangLang
{
    public static class SlangEnvironment
    {
        public static Version GetEnvironmentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static Version GetInterpreterVersion()
        {
            Assembly[] allAsms = AppDomain.CurrentDomain.GetAssemblies();
            Assembly interpreterAssembly = allAsms.FirstOrDefault(asm => asm.GetName().Name == "SlangLang.Interactive");
            if (interpreterAssembly == null)
                return new Version(0, 0, 0, 0);
            return interpreterAssembly.GetName().Version;
        }
    }
}