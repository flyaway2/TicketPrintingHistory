﻿using BackEnd.viewmodel;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for SplashScreenView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(SplashScreenViewModel))]
    public partial class SplashScreenView : MvxWpfView
    {
        public SplashScreenView()
        {
            InitializeComponent();
        }
    }
}
