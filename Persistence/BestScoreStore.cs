// File: Persistence/BestScoreStore.cs
using System;
using System.IO;
using System.Text.Json;

namespace NeonDrift
{
    /// <summary>
    /// Load/Save the best score to a small JSON file in the user's data directory.
    /// </summary>
    public static class BestScoreStore
    {
        private sealed class Model { public int BestScore { get; set; } }

        private static string AppDataDir()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrWhiteSpace(root)) root = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string dir = Path.Combine(root, "NeonDrift");
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static string FilePath => Path.Combine(AppDataDir(), "bestscore.json");

        public static int Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return 0;
                var json = File.ReadAllText(FilePath);
                var m = JsonSerializer.Deserialize<Model>(json);
                return m?.BestScore ?? 0;
            }
            catch { return 0; }
        }

        public static void Save(int score)
        {
            if (score < 0) return;
            try
            {
                var json = JsonSerializer.Serialize(new Model { BestScore = score }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch { /* ignore */ }
        }
    }
}
