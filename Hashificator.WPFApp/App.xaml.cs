using Hashificator.Common;
using System;
using System.IO;
using System.Windows;

namespace Hashificator.WPFApp
{
    public partial class App : Application
    {
        public static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hashificator");

        public static readonly string ConfigFile = "config.json";

        public static Config Configuration { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Configuration = await Config.LoadConfig(AppDataFolder, ConfigFile);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}