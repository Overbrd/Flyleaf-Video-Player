﻿using System;
using System.Threading;
using System.Windows;

using FlyleafLib;
using FlyleafLib.MediaPlayer;

namespace WpfNetFramework
{
    /// <summary>
    /// FlyleafPlayer (WPF Control) Sample
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Flyleaf Player binded to VideoView
        /// </summary>
        public Player Player { get; set; }
        public Config Config { get; set; }

        public MainWindow()
        {
            // Ensures that we have enough worker threads to avoid the UI from freezing or not updating on time
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);

            // Initializes Engine (Specifies FFmpeg libraries path which is required)
            Engine.Start(new EngineConfig()
            {
                #if DEBUG
                LogOutput       = ":debug",
                LogLevel        = LogLevel.Debug,
                FFmpegLogLevel  = FFmpegLogLevel.Warning,
                #endif
                
                PluginsPath     = ":Plugins",
                FFmpegPath      = ":FFmpeg",
            });

            // Prepares Player's Configuration
            Config = new Config();

            // Initializes the Player
            Player = new Player(Config);

            // Allowing VideoView to access our Player
            DataContext = this;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Giving access to keyboard events on start up
            //flyleafControl.VideoView.WinFormsHost.Focus();
        }
    }
}
