using BackEnd.viewmodel;
using MvvmCross.Commands;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;

namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for HomepageView.xaml
    /// </summary>
    [MvxWindowPresentation]
    [MvxViewFor(typeof(HomepageViewModel))]
    public partial class HomepageView : MvxWindow
    {
        public HomepageView()
        {
            InitializeComponent();
        }

        
    }
}
