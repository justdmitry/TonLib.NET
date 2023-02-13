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

        private const string DllNameWindows = "tonlibjson.dll";
        private const string DllNameLinux = "libtonlibjson.so.0.5";
        private const string DllNameOSX = "libtonlibjson.0.5.dylib";

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
                    libraryName = DllNameWindows;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libraryName = DllNameLinux;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryName = DllNameOSX;
                }
            }

            return NativeLibrary.Load(libraryName, assembly, dllImportSearchPath);
        }
    }
}
