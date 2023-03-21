using BackEnd.viewmodel;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Printing;
using System;
using BackEnd.CustomClass;
using System.Windows.Input;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;

namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for ImpressionView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(ImpressionViewModel))]
    public partial class ImpressionView : MvxWpfView
    {
        public ImpressionView()
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
            System.Windows.MessageBox.Show(args.Value);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var comb = (ComboBox)sender;
            _SelectedPrinter = comb.SelectedItem.ToString();
        }
        private string _SelectedPrinter;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_SelectedPrinter == null)
                return;
            UIServices.SetBusyState();
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintTicket.PageMediaSize = new PageMediaSize(Ticket.Width, Ticket.Height);
            printDlg.UserPageRangeEnabled = true;
            printDlg.PrintQueue = new PrintQueue(new PrintServer(), _SelectedPrinter);
            printDlg.PrintTicket.CopyCount = Convert.ToInt32(NumCopies.Value);
            printDlg.PrintVisual(Ticket, "etiquette");
        }

        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(Keyboard.Modifiers==ModifierKeys.Control && Key.P==e.Key)
            {
                if(PageProp.Visibility==Visibility.Collapsed)
                {
                    PageProp.Visibility = Visibility.Visible;
                }
                else
                {
                    PageProp.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void MvxWpfView_Loaded(object sender, RoutedEventArgs e)
        {
            var set = this.CreateBindingSet<ImpressionView, ImpressionViewModel>();
            set.Bind(this).For(view => view.ShowMsg).To(viewmodel => viewmodel.ShowMsg);
            set.Apply();
        }
    }
}
