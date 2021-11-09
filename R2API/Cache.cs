﻿using BepInEx;
using BepInEx.Configuration;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace R2API {
    internal static class Cache {
        internal const string CachedDataIsReused =
            "THIS IS AUTOGENERATED. Will be reused if the plugins folder hash is still the same as the last game launch.";

        private static ConfigFile _config;
        private static ConfigEntry<string> _cacheHash;

        internal static bool UseCache;

        public static BigInteger CurrentFolderHash { get; private set; }

        internal static void Init(ConfigFile config) {
            _config = config;

            _cacheHash = _config.Bind<string>(
                nameof(Cache), "Hash", "0",
                "The plugin folder hash generated at game launch.");

            var oldFolderHash = BigInteger.Parse(_cacheHash.Value, NumberStyles.HexNumber);

            CurrentFolderHash = GetFolderHash(Paths.PluginPath);
            if (CurrentFolderHash == oldFolderHash) {
                UseCache = true;
                R2API.Logger.LogInfo("Using Cache.");
            }
            else {
                _cacheHash.Value = CurrentFolderHash.ToString("X");
                R2API.Logger.LogInfo($"Not using Cache.");
            }
        }

        private static BigInteger GetFolderHash(string folderPath) {
            BigInteger res = 0;

            var dllPaths = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);
            foreach (var dllPath in dllPaths) {
                var hash = File.GetLastWriteTimeUtc(dllPath).ToBinary() + File.GetCreationTimeUtc(dllPath).ToBinary();
                var checksumStr = hash.ToString("X");
                var checksum = BigInteger.Parse(checksumStr, NumberStyles.HexNumber);
                res += checksum;
            }

            return res;
        }
    }
}
