using BackEnd.viewmodel;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
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
using System.Windows.Shapes;

namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for BusyIndicatorView.xaml
    /// </summary>
    [MvxWindowPresentation(Modal =true)]
    [MvxViewFor(typeof(BusyIndicatorViewModel))]
    public partial class BusyIndicatorView : MvxWindow
    {
        private IMvxInteraction<string> _ShowMsg;
        public IMvxInteraction<string> ShowMsg
        {
            get => _ShowMsg;
            set
            {
                if (_ShowMsg != null)
                    _ShowMsg.Requested -= ShowMsgBox;
                if (value != null)
                {
                    _ShowMsg = value;
                    _ShowMsg.Requested += ShowMsgBox;
                }
            }
        }
        public void ShowMsgBox(object sender, MvxValueEventArgs<string> args)
        {
            MessageBox.Show(args.Value);
        }


        public BusyIndicatorView()
        {
            InitializeComponent();
        }

        private void MvxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var set = this.CreateBindingSet<BusyIndicatorView, BusyIndicatorViewModel>();
            set.Bind(this).For(view => view.ShowMsg).To(viewmodel => viewmodel.ShowError);
            set.Apply();
        }
    }
}
