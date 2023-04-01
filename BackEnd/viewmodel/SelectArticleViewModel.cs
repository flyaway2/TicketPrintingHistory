using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.viewmodel
{
    public class SelectArticleViewModel:MvxViewModel<Article,Article>
    {
        public readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public MvxInteraction<string> ShowMsg { get; } = new MvxInteraction<string>();

        public SelectArticleViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            UpdateArticleList();
            UpdateCategorieList();
        }

        private IMvxCommand _AnnulerCmd;

        public IMvxCommand AnnulerCmd
        {
            get {
                _AnnulerCmd = new MvxCommand(ExitWindow);
                return _AnnulerCmd; }
        }
        public void ExitWindow()
        {
            _navigationService.Close(this);
        }
        private IMvxAsyncCommand _ValiderCmd;

        public IMvxAsyncCommand ValiderCmd
        {
            get
            {
                _ValiderCmd = new MvxAsyncCommand(ValiderSelection);
                return _ValiderCmd;
            }
        }
        public async Task ValiderSelection()
        {
            if(SelectedArticle!=null)
            {
                await _navigationService.Close(this,SelectedArticle);
            }
            else
            {
                ShowMsg.Raise("Aucun article Séléctionnée");
            }
            
        }

        public void UpdateArticleList()
        {
            FullArticles = new MvxObservableCollection<Article>(_db.GetArticlesDetails());
            Articles = new MvxObservableCollection<Article>();
            Articles.ReplaceWith(FullArticles);
        }
        public void UpdateCategorieList()
        {
            categories = new MvxObservableCollection<Categorie>(_db.GetCategorie());
        }

        public override void Prepare(Article parameter)
        {
           
        }

        private MvxObservableCollection<Article> _Articles;

        public MvxObservableCollection<Article> Articles
        {
            get { return _Articles; }
            set
            {
                _Articles = value;
                RaisePropertyChanged();
            }
        }
        private Article _SelectedArticle;

        public Article SelectedArticle
        {
            get { return _SelectedArticle; }
            set
            {
                _SelectedArticle = value;
                RaisePropertyChanged();
               

            }
        }
        private MvxObservableCollection<Article> _FullArticles;

        public MvxObservableCollection<Article> FullArticles
        {
            get { return _FullArticles; }
            set
            {
                _FullArticles = value;
                RaisePropertyChanged();
            }
        }
        private Categorie _SelectedCategorie;

        public Categorie SelectedCategorie
        {
            get { return _SelectedCategorie; }
            set
            {
                _SelectedCategorie = value;
                RaisePropertyChanged();
                if (value != null)
                    FilterArticles();
            }
        }
        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                RaisePropertyChanged();
                if (value != null)
                {
                    SearchArticles();

                }

            }
        }
        public void SearchArticles()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                if (SelectedCategorie != null)
                {
                    FilterArticles();

                    return;
                }
                Articles.ReplaceWith(FullArticles);
                return;
            }
            if (SelectedCategorie != null)
            {
                FilterArticles();
                Articles.ReplaceWith(Articles.Where(art => art.refarticle.ToLower().Contains(SearchText.ToLower())
                       || art.largeur.ToString().Contains(SearchText.ToLower())
                       || art.nom.ToLower().ToString().Contains(SearchText.ToLower())
                       || art.client.ToLower().ToString().Contains(SearchText.ToLower())
                       ).ToList());
            }
            else
            {
                Articles.ReplaceWith(FullArticles.Where(
                        art => art.refarticle.ToLower().Contains(SearchText.ToLower())
                        || art.largeur.ToString().Contains(SearchText.ToLower())
                        ).ToList());
            }

        }

        public void FilterArticles()
        {
            Articles.ReplaceWith(FullArticles.Where(art => art.categorie == SelectedCategorie.id).ToList());

        }

        private MvxObservableCollection<Categorie> _categories;

        public MvxObservableCollection<Categorie> categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged();
            }
        }
    }
}
