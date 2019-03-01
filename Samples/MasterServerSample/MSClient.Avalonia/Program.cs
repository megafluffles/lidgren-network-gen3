using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using MSClient.Avalonia.ViewModels;
using MSClient.Avalonia.Views;

namespace MSClient.Avalonia
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
