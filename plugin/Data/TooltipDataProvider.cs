using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace NestedTooltips
{
    /// <summary>
    /// Lädt und verwaltet Tooltip-Daten für verschiedene Sprachen.
    /// Die Daten werden aus .json-Dateien in sprachspezifischen Unterordnern geladen.
    /// z.B. /data/tooltips/deutsch/ oder /data/tooltips/englisch/
    /// </summary>
    public class TooltipDataProvider : ITooltipDataProvider
    {
        private readonly Dictionary<string, TooltipData> _tooltipDatabase = new();
        private const string BaseTooltipFolderPath = "res://data/tooltips/";
        private string _currentLanguage = "englisch"; // Standard-Fallback-Sprache

        /// <summary>
        /// Initialisiert den Provider und lädt die aktuelle Sprache des Spiels.
        /// </summary>
        public TooltipDataProvider()
        {
            // Finde die aktuelle Sprache von Godot (z.B. "de", "en") und lade die passenden Tooltips.
            // Wir mappen Godots Kürzel auf unsere Ordnernamen.
            var locale = TranslationServer.GetLocale(); // z.B. "de"
            
            // Standard-Mapping, das du erweitern kannst
            var localeToFolderMap = new Dictionary<string, string>
            {
                { "de", "deutsch" },
                { "en", "englisch" }
            };

            if (localeToFolderMap.TryGetValue(locale, out var languageFolder))
            {
                SwitchLanguage(languageFolder);
            }
            else
            {
                GD.Print($"Kein Tooltip-Verzeichnis für die Sprache '{locale}' gefunden. Lade Fallback-Sprache '{_currentLanguage}'.");
                SwitchLanguage(_currentLanguage); // Lade die Fallback-Sprache
            }
        }

        /// <inheritdoc />
        public TooltipData? GetTooltipData(string id)
        {
            if (_tooltipDatabase.TryGetValue(id, out TooltipData? data))
            {
                return data;
            }
            
            GD.PrintErr($"Tooltip mit der ID '{id}' wurde in der aktuellen Sprache '{_currentLanguage}' nicht gefunden.");
            return null;
        }

        /// <summary>
        /// Wechselt die aktive Sprache, löscht alte Tooltip-Daten und lädt neue.
        /// </summary>
        /// <param name="language">Der Name des Sprachordners (z.B. "deutsch").</param>
        public void SwitchLanguage(string language)
        {
            _currentLanguage = language;
            _tooltipDatabase.Clear();
            
            string languagePath = BaseTooltipFolderPath + language + "/";
            LoadTooltipsFromPath(languagePath);
        }

        /// <summary>
        /// Durchsucht ein Verzeichnis, liest alle JSON-Dateien und
        /// fügt die Daten zur internen Datenbank hinzu.
        /// </summary>
        private void LoadTooltipsFromPath(string path)
        {
            using var dir = DirAccess.Open(path);

            if (dir == null)
            {
                GD.PrintErr($"Tooltip-Verzeichnis für die Sprache '{_currentLanguage}' nicht gefunden unter: {path}");
                return;
            }

            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.EndsWith(".json"))
                {
                    LoadTooltipFile(dir.GetCurrentDir() + "/" + fileName);
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();

            GD.Print($"TooltipDataProvider: {_tooltipDatabase.Count} Tooltips für die Sprache '{_currentLanguage}' geladen.");
        }
        
        /// <summary>
        /// Lädt eine einzelne JSON-Datei und verarbeitet ihren Inhalt.
        /// </summary>
        private void LoadTooltipFile(string filePath)
        {
            using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
            string content = file.GetAsText();

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tooltips = JsonSerializer.Deserialize<List<TooltipData>>(content, options);

                if (tooltips == null) return;

                foreach (var tooltip in tooltips)
                {
                    if (!_tooltipDatabase.TryAdd(tooltip.Id, tooltip))
                    {
                        GD.PrintErr($"Doppelte Tooltip-ID '{tooltip.Id}' in '{filePath}'. Wird ignoriert.");
                    }
                }
            }
            catch (JsonException ex)
            {
                GD.PrintErr($"Fehler beim Parsen von '{filePath}': {ex.Message}");
            }
        }
    }
}