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
    /// Interaction logic for UploadProductionView.xaml
    /// </summary>
    [MvxWindowPresentation(Modal = true)]
    [MvxViewFor(typeof(UploadProductionViewModel))]
    public partial class UploadProductionView : MvxWindow
    {
        public UploadProductionView()
        {
            InitializeComponent();
        }
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

        private void MvxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var set = this.CreateBindingSet<UploadProductionView, UploadProductionViewModel>();
            set.Bind(this).For(view => view.ShowMsg).To(viewmodel => viewmodel.ShowMsg);
            set.Apply();
        }
    }
}
