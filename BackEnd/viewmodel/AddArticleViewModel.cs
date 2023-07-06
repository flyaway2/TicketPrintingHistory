using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace BackEnd.viewmodel
{
    public class AddArticleViewModel:MvxViewModel<ArticleViewModel>
    {
        private IMvxNavigationService _navigationService;
        public AddArticleViewModel(IMvxNavigationService navSer)
        {
            _navigationService = navSer;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            GetCompositions();
            GetCouleurs();
            GetCategories();
          
        }

        #region Properties
        private Article _NewProd=new Article();

        public Article NewProd
        {
            get { return _NewProd; }
            set { _NewProd = value; }
        }

        private MvxObservableCollection<composition> _ListComposition;

        public MvxObservableCollection<composition> ListComposition
        {
            get { return _ListComposition; }
            set { _ListComposition = value; RaisePropertyChanged(); }
        }
        private MvxObservableCollection<Categorie> _ListCategorie;

        public MvxObservableCollection<Categorie>ListCategorie
        {
            get { return _ListCategorie; }
            set { _ListCategorie = value; RaisePropertyChanged(); }
        }

        private MvxObservableCollection<Couleur> _ListCouleur;

        public MvxObservableCollection<Couleur> ListCouleur
        {
            get { return _ListCouleur; }
            set { _ListCouleur = value; RaisePropertyChanged(); }
        }
        private SqliteData _db;
        private ArticleViewModel ArticleVM;
        #endregion

        #region methods

        public void SetArticle()
        {
            if (!ArticleVM.IsEditArticle)
                return;
            NewProd = ArticleVM.SelectedArticle;
            NewProd.categorieObj = ListCategorie.First(cat=>cat.id== ArticleVM.SelectedArticle.categorieObj.id);
            NewProd.compositionObj = ListComposition.First(comp => comp.id == ArticleVM.SelectedArticle.compositionObj.id);
            NewProd.couleurObj = ListCouleur.First(col => col.id == ArticleVM.SelectedArticle.couleurObj.id);
        }
        public void GetCouleurs()
        {
            ListCouleur = new MvxObservableCollection<Couleur>(_db.GetCouleurs());
        }
        public void GetCompositions()
        {
            ListComposition = new MvxObservableCollection<composition>(_db.GetCompositions());
        }
        public void GetCategories()
        {
            ListCategorie = new MvxObservableCollection<Categorie>(_db.GetCategorie());
        }
        public void Cancel()
        {
            _navigationService.Close(this);
        }
        public bool IsFieldsEmpty()
        {
            if (NewProd != null)
            {
                if (NewProd.idarticle == 0)
                    return true;
                if (NewProd.largeur == 0)
                    return true;
                if (NewProd.refarticle == null || string.IsNullOrWhiteSpace(NewProd.refarticle))
                    return true;
                if (NewProd.designation == null || string.IsNullOrWhiteSpace(NewProd.designation))
                    return true;
                if (NewProd.unite == null || string.IsNullOrWhiteSpace(NewProd.unite))
                    return true;
                if (NewProd.couleurObj == null)
                    return true;
                if (NewProd.compositionObj == null)
                    return true;
                if (NewProd.categorieObj == null)
                    return true;
            }

                
            return false;
        }
        public void AjouterProduit()
        {
            if (IsFieldsEmpty())
            {
                ShowMsg.Raise("S.V.P Remplit les champs obligatoires");
                return;
            }
            if(ArticleVM.IsEditArticle)
            {
                if (_db.IsArticleExist(NewProd.id, NewProd.idarticle, NewProd.refarticle, NewProd.designation).Count > 0)
                {
                    ShowMsg.Raise("Article existe déja");
                    return;
                }
                _db.UpdateArticle(NewProd);
            }
            else
            {
                if (_db.IsArticleExist(NewProd.idarticle, NewProd.refarticle, NewProd.designation).Count > 0)
                {
                    ShowMsg.Raise("Article existe déja");
                    return;
                }
                _db.AddNewArticle(NewProd);
               
            }

            ArticleVM.UpdateArticleList();
            _navigationService.Close(this);
        }

        public override void Prepare(ArticleViewModel parameter)
        {
            ArticleVM = parameter;
            SetArticle();
        }
        #endregion

        #region Events
        public MvxInteraction<string> ShowMsg { get; } = new MvxInteraction<string>();
        #endregion
    }
}
