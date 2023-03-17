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
    }
}
