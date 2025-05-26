using System;
using System.Globalization;
using System.Windows;

namespace SyncCore.Helpers
{
    public static class LanguageManager
    {
        public static void SetLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            
            // Force reload of resources
            var resources = Application.Current.Resources;
            resources.MergedDictionaries.Clear();
            
            // Load English first
            resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/Lang/en-US.xaml", UriKind.Relative) });
            
            // Then load the selected language if it's not English
            if (languageCode != "en-US")
            {
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"Resources/Lang/{languageCode}.xaml", UriKind.Relative) });
            }
        }
    }
} 