// File: Resources/ResourceBundleLoader.cs
using System;
using SplashKitSDK;

namespace NeonDrift
{
    /// <summary>
    /// Thin helper around SplashKit resource bundles so you can:
    /// - call Load("main","main.txt") once at startup (from Program.cs)
    /// - free the bundle on shutdown
    /// Works with folders created by: `skm resources` (Resources/* + Resources/bundles). 
    /// </summary>
    internal static class ResourceBundleLoader
    {
        /// <summary>
        /// Load a resource bundle by name and manifest filename (in Resources/bundles).
        /// Safe to call multiple times; it won’t reload if already present.
        /// Example: Load("main", "main.txt")
        /// </summary>
        public static void Load(string name, string filename)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Bundle name required", nameof(name));
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Bundle filename required", nameof(filename));

            // Avoid double-loads
            if (SplashKit.HasResourceBundle(name)) return;

            // Loads all resources described in Resources/bundles/<filename>
            SplashKit.LoadResourceBundle(name, filename);
        }

        /// <summary>
        /// Free a previously loaded bundle (and all resources it created).
        /// Safe to call even if not loaded.
        /// </summary>
        public static void Free(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (!SplashKit.HasResourceBundle(name)) return;

            SplashKit.FreeResourceBundle(name);
        }
    }
}
