using System.Windows;
using System.Globalization;
using System.Windows.Media;

namespace SyncCore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set default culture to English
            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            // Set default background color
            Resources["WindowBackground"] = Brushes.White;
            Resources["GridBackground"] = Brushes.White;
        }
    }
} 