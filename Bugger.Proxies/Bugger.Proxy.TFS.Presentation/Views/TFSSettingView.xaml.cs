﻿using BigEgg.Framework.Applications.Views;
using Bugger.Proxy.TFS.ViewModels;
using Bugger.Proxy.TFS.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bugger.Proxy.TFS.Presentation.Views
{
    [Export(typeof(ITFSSettingView))]
    public partial class TFSSettingView : UserControl, ITFSSettingView
    {
        private readonly Lazy<TFSSettingViewModel> viewModel;
        
        
        public TFSSettingView()
        {
            InitializeComponent();

            viewModel = new Lazy<TFSSettingViewModel>(() => ViewHelper.GetViewModel<TFSSettingViewModel>(this));
            Loaded += LoadedHandler;
            Unloaded += UnloadedHandler;
        }


        public string Title { get { return Properties.Resources.SettingViewTitle; } }

        private TFSSettingViewModel ViewModel { get { return viewModel.Value; } }


        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            password.Password = ViewModel.Password;
            ViewModel.PropertyChanged += SettingsPropertyChanged;
            tfsName.Focus();
        }

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= SettingsPropertyChanged;
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Password")
            {
                password.Password = ViewModel.Password;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            ViewModel.Password = passwordBox.Password;
        }
    }
}
