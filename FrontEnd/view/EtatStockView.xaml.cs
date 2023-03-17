using BackEnd.viewmodel;
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
    /// Interaction logic for EtatStockView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(EtatStockViewModel))]
    public partial class EtatStockView : MvxWpfView
    {
        public EtatStockView()
        {
            InitializeComponent();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Key.P == e.Key)
            {
                if (DBCred.Visibility == Visibility.Collapsed)
                {
                    DBCred.Visibility = Visibility.Visible;
                }
                else
                {
                    DBCred.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
