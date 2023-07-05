using BackEnd.CustomClass;
using BackEnd.viewmodel;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using System.Windows.Forms;
namespace FrontEnd.view
{
    /// <summary>
    /// Interaction logic for CompanyInfoView.xaml
    /// </summary>
    [MvxWindowPresentation]
    [MvxViewFor(typeof(CompanyInfoViewModel))]
    public partial class CompanyInfoView : MvxWindow
    {
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
            fileDialog.DefaultExt = "Files|*.jpg;*.jpeg;*.png"; // Required file extension 
            fileDialog.Filter = "Excel Files|*.png;*.jpg;*.jpeg"; // Optional file extensions

            if (fileDialog.ShowDialog() ==  System.Windows.Forms.DialogResult.OK)
                args.Value.UploadCallback(fileDialog.FileName, fileDialog.SafeFileName, true);
        }
        private IMvxInteraction<string> _ShowMsg;

        public IMvxInteraction<string> ShowMsg
        {
            get {
                return _ShowMsg;
            }
            set {
                if (_ShowMsg != null)
                    _ShowMsg.Requested -= ShowMsgBox;
                if(value!=null)
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

        public CompanyInfoView()
        {
            InitializeComponent();
        }

        private void MvxWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var set = this.CreateBindingSet<CompanyInfoView, CompanyInfoViewModel>();
            set.Bind(this).For(view => view.GetFile).To(viewmodel => viewmodel.GetFilePath);
            set.Bind(this).For(view => view.ShowMsg).To(viewmodel => viewmodel.ShowMsg);
            set.Apply();
        }
    }
}
