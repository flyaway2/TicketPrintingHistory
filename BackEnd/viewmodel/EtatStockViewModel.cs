using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.viewmodel
{
    public class EtatStockViewModel:MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public EtatStockViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            GetProduction();
        }

        private MvxObservableCollection<Article> _Articles;

        public MvxObservableCollection<Article> Articles
        {
            get { return _Articles; }
            set { _Articles = value;
                RaisePropertyChanged();
            }
        }
        private Article _SelectedArticle;

        public Article SelectedArticle
        {
            get { return _SelectedArticle; }
            set { _SelectedArticle = value;
                RaisePropertyChanged();
            }
        }
        private string _Source;

        public string Source
        {
            get { return _Source; }
            set { _Source = value; RaisePropertyChanged(); }
        }
        private string _Catalog;

        public string Catalog
        {
            get { return _Catalog; }
            set { _Catalog = value; RaisePropertyChanged(); }
        }
        private IMvxCommand _SaveCmd;

        public IMvxCommand SaveCmd
        {
            get {
                _SaveCmd = new MvxCommand(SaveDBCred);
                return _SaveCmd; }
        }
        public void SaveDBCred()
        {
            if (Catalog == null || string.IsNullOrWhiteSpace(Catalog)
                || Source == null || string.IsNullOrWhiteSpace(Source))
                return;
            var dbCred = _db.GetDBCred();
            if (dbCred.Count>0)
            {

                var NewDBCred = new DBCred();
                NewDBCred.source = Source;
                NewDBCred.catalog = Catalog;
                NewDBCred.id = dbCred[0].id;
                _db.UpdateDefaultDBCred(NewDBCred);
            }
            else
            {
                var NewDBCred = new DBCred();
                NewDBCred.source = Source;
                NewDBCred.catalog = Catalog;
                _db.AddDefaultDBCred(NewDBCred);
            }
        }
        public void GetProduction()
        {
            Articles = new MvxObservableCollection<Article>(_db.GetEtatProduction());
        }

        


    }
}
