using System;

namespace SlangLang
{
    public enum EnvironmentConfigSource
    {
        CommandLineArguments,
        EnvironmentVariables,
        GlobalFile,
        LocalFile,
    }
}