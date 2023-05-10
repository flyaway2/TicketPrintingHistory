using BackEnd.CustomClass;
using BackEnd.viewmodel;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for ArticleView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(ArticleViewModel))]
    public partial class ArticleView : MvxWpfView
    {
        public ArticleView()
        {
            InitializeComponent();
        }

        private IMvxInteraction<YesNoQuestion> _ConfirmAction;

        public IMvxInteraction<YesNoQuestion> ConfirmAction
        {
            get { return _ConfirmAction; }
            set {
                if (_ConfirmAction != null)
                    _ConfirmAction.Requested -= ShowMsgBox;
                if (value != null)
                {
                    _ConfirmAction = value;
                    _ConfirmAction.Requested += ShowMsgBox;
                }
            }
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
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void DataObject_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
        public void ShowMsgBox(object sender, MvxValueEventArgs<YesNoQuestion> args)
        {
           var result= System.Windows.MessageBox.Show(args.Value.Question,"Confirm Suppression",MessageBoxButton.YesNo);
            if(result==MessageBoxResult.Yes)
            {
                args.Value.YesNoCallback(true);
            }
        }
        public void ShowMsgBox(object sender, MvxValueEventArgs<string> args)
        {
            System.Windows.MessageBox.Show(args.Value);
        }

        private IMvxInteraction<UploadFile> _UploadFile;
        public IMvxInteraction<UploadFile> GetFile
        {
            get => _UploadFile;
            set
            {
                if (_UploadFile != null)
                    _UploadFile.Requested -= GetFilePath;
                if (value != null)
                {
                    _UploadFile = value;
                    _UploadFile.Requested += GetFilePath;
                }
            }
        }

        public void GetFilePath(object sender, MvxValueEventArgs<UploadFile> args)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "Files|*.xls;*.xlsx;*.xlsm"; // Required file extension 
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"; // Optional file extensions

            if (fileDialog.ShowDialog() == DialogResult.OK)
                args.Value.UploadCallback(fileDialog.FileName, fileDialog.SafeFileName, true);
        }

        private void MvxWpfView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var set = this.CreateBindingSet<ArticleView, ArticleViewModel>();
            set.Bind(this).For(view => view.GetFile).To(viewmodel => viewmodel.GetFilePath);
            set.Bind(this).For(view => view.ShowMsg).To(viewmodel => viewmodel.ShowError);
            set.Bind(this).For(view => view.ConfirmAction).To(viewmodel => viewmodel.ConfirmAction);
            set.Apply();
        }
    }
}
