﻿using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using MSClient.ViewModels;
using MSClient.Views;

namespace MSClient
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
