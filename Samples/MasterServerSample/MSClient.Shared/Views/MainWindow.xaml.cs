﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MSClient.ViewModels;

namespace MSClient.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
