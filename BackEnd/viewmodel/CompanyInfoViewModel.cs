using BackEnd.CustomClass;
using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BackEnd.viewmodel
{
    public class CompanyInfoViewModel:MvxViewModel<ImpressionViewModel>
    {
        private IMvxNavigationService _navigationService;
        private SqliteData _db;
        public CompanyInfoViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            GetCompanyInfo();
        }


        #region Properties
        private string _CompanyName;

        public string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = value;
                RaisePropertyChanged();
            }
        }
        private string _Facebook;

        public string Facebook
        {
            get { return _Facebook; }
            set { _Facebook = value;
                RaisePropertyChanged();
            }
        }

        private string _HomePhone;

        public string HomePhone
        {
            get { return _HomePhone; }
            set { _HomePhone = value;
                RaisePropertyChanged();
            }
        }

        private string _Whatsapp;

        public string Whatsapp
        {
            get { return _Whatsapp; }
            set { _Whatsapp = value;
                RaisePropertyChanged();
            }
        }

        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { _Email = value;
                RaisePropertyChanged();
            }
        }

        private string _Logo;

        public string Logo
        {
            get { return _Logo; }
            set { _Logo = value;
                RaisePropertyChanged();
            }
        }


        private companyinfo SavedCompInfo;
        #endregion

        #region Methods
        public void GetCompanyInfo()
        {
           if(_db.GetCompanyInfo().Count==0)
                return;
            SavedCompInfo = _db.GetCompanyInfo()[0];
            CompanyName = SavedCompInfo.nom;
            Facebook = SavedCompInfo.facebook;
            HomePhone = SavedCompInfo.homephone;
            Whatsapp = SavedCompInfo.whatsapp;
            Email = SavedCompInfo.email;
            Logo = SavedCompInfo.logo;
        }

        public bool IsEmptyFields()
        {
            return CompanyName == null || string.IsNullOrWhiteSpace(CompanyName) ||
                    Facebook == null || string.IsNullOrWhiteSpace(Facebook) ||
                    HomePhone == null || string.IsNullOrWhiteSpace(HomePhone) ||
                    Whatsapp == null || string.IsNullOrWhiteSpace(HomePhone) ||
                    Email == null || string.IsNullOrWhiteSpace(Email) ||
                    Logo == null || string.IsNullOrWhiteSpace(Logo);
        }
        public void UploadLogo()
        {
            var upload = new UploadFile
            {
                UploadCallback = (FullPath,FileName,ok) =>
                {
                    if(ok)
                    {
                        Logo = FileName;
                        File.Copy(FullPath, Directory.GetCurrentDirectory()+"/Asset/" + FileName);
                    }
                }
            };
            GetFilePath.Raise(upload);
        }

        public void Close()
        {
            _navigationService.Close(this);
        }

        public async void Validation()
        {
            if(IsEmptyFields())
            {
                ShowMsg.Raise("S.V.P Remplit tous les champs");
                return;
            }
            companyinfo compInfo = new companyinfo();
            compInfo.nom = CompanyName;
            compInfo.facebook = Facebook;
            compInfo.whatsapp = Whatsapp;
            compInfo.homephone = HomePhone;
            compInfo.email = Email;
            compInfo.logo = Logo;

            if (SavedCompInfo != null)
            {
                compInfo.id = SavedCompInfo.id;
                _db.UpdateCompanyInfo(compInfo);
            }
               
            else
                _db.AddCompanyInfo(compInfo);

            ImpressionViewM.GetCompanyInfo();
            _navigationService.Close(this);
        }
        private ImpressionViewModel ImpressionViewM;
        public override void Prepare(ImpressionViewModel parameter)
        {
            ImpressionViewM = parameter;
        }
        #endregion

        #region Events
        public MvxInteraction<string> ShowMsg { get; } = new MvxInteraction<string>();
        public MvxInteraction<UploadFile> GetFilePath { get; } = new MvxInteraction<UploadFile>();
        #endregion
    }
}
