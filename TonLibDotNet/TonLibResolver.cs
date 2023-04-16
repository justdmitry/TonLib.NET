using System.Reflection;
using System.Runtime.InteropServices;

namespace TonLibDotNet
{
    /// <summary>
    /// Helps to load different native libraries on different platforms.
    /// Based on https://github.com/dotnet/samples/tree/main/core/extensions/DllMapDemo.
    /// </summary>
    public static class TonLibResolver
    {
        public const string DllNamePlaceholder = "ton-lib-name-here";

        public static void Register(Assembly assembly)
        {
            NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
        }

        private static IntPtr MapAndLoad(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
        {
            if (StringComparer.Ordinal.Equals(libraryName, DllNamePlaceholder))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    libraryName = "tonlibjson.dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libraryName = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "tonlibjson-linux-arm64.so" : "tonlibjson-linux-x86_64.so";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryName = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "tonlibjson-mac-arm64.dylib" : "tonlibjson-mac-x86-64.dylib";
                }
            }

            return NativeLibrary.Load(libraryName, assembly, dllImportSearchPath);
        }
    }
}
