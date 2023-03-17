using BackEnd.CustomClass;
using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing ;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;
using System.Globalization;

namespace BackEnd.viewmodel
{
    public class ImpressionViewModel:MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public ImpressionViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            UpdateArticleList();
            UpdateCategorieList();
            UpdatePrintCategories();
            SelectedCategorie = categories.FirstOrDefault();
            GetInstalledPrinters();

            UpdateContratList();
        }
        private MvxObservableCollection<contrat> _ListContrat;

        public MvxObservableCollection<contrat> ListContrat
        {
            get { return _ListContrat; }
            set { _ListContrat = value;
                RaisePropertyChanged();
            }
        }

        private contrat _SelectedContrat;

        public contrat SelectedContrat
        {
            get { return _SelectedContrat; }
            set { _SelectedContrat = value;
                RaisePropertyChanged();
                if (value != null)
                    SetArticlesContrat();
            }
        }

        public void SetArticlesContrat()
        {
            List<contratarticle> ArtList = _db.GetContratArticles(SelectedContrat);
            Articles = new MvxObservableCollection<Article>();


            foreach (contratarticle art in ArtList)
            {
                Articles.Add(FullArticles.First(artc => artc.id == art.article));
            }
        }

        public void UpdateContratList()
        {
            ListContrat = new MvxObservableCollection<contrat>(_db.GetActiveContrats());
        }

        private bool _IsContrat;

        public bool IsContrat
        {
            get { return _IsContrat; }
            set { _IsContrat = value;
                RaisePropertyChanged();
                if (!IsContrat)
                    ResetArticles();
            }
        }
        public void ResetArticles()
        {
            UpdateArticleList();
        }


        private string _HistJour;

        public string HistJour
        {
            get { return _HistJour; }
            set { _HistJour = value;
                RaisePropertyChanged();
            }
        }


        private List<string> InstalledPrinters;
        public void GetInstalledPrinters()
        {
            InstalledPrinters =System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            var DefaultPrinter = _db.GetImprimant();
            if (DefaultPrinter.Count == 0)
                return;
            if (InstalledPrinters.FirstOrDefault(prnt => prnt.Equals(DefaultPrinter[0].nom))!=null)
            {
                SelectedPrinter = DefaultPrinter[0].nom;
            }
        }
        
    private string _SelectedPrinter;

        public string SelectedPrinter
        {
            get { return _SelectedPrinter; }
            set { _SelectedPrinter = value;
                RaisePropertyChanged();
                if(value!=null)
                    UpdateDefaultPrinter();
            }
        }
        
        public void UpdateDefaultPrinter()
        {
            imprimant mprinter = new imprimant();
            mprinter.nom = SelectedPrinter;
            var DefaultPrinter = _db.GetImprimant();
            if(DefaultPrinter.Count>0)
            {
                mprinter.id = DefaultPrinter[0].id;
                _db.UpdateDefaultImprimant(mprinter);
            }
            else
            {
                _db.AddDefaultImprimant(mprinter);
            }
        }

        private string _PageWidth="10.5cm";

        public string PageWidth
        {
            get { return _PageWidth; }
            set { _PageWidth = value;
                RaisePropertyChanged();
            }
        }

        private string _LeftMargin="0.1cm";

        public string LeftMargin
        {
            get { return _LeftMargin; }
            set { _LeftMargin = value;
                RaisePropertyChanged();
            }
        }

        private string _RightMargin = "0.1cm";

        public string RightMargin
        {
            get { return _RightMargin; }
            set { _RightMargin = value;
                RaisePropertyChanged();
            }
        }

        private string _PageWidth2 = "12cm";

        public string PageWidth2
        {
            get { return _PageWidth2; }
            set
            {
                _PageWidth2 = value;
                RaisePropertyChanged();
            }
        }

        private IMvxCommand _PrintCmd;

        public IMvxCommand PrintCmd
        {
            get {
                _PrintCmd = new MvxCommand<Grid>(StartPrinting);
                return _PrintCmd; }
        }

        

        public void StartPrinting(Grid Ticket)
        {
            try
            {
                if (SelectedPrinter == null || string.IsNullOrWhiteSpace(SelectedPrinter) || SelectedArticle == null
                || SelectedRaisonImpr == null)
                    return;
                UIServices.SetBusyState();
                PrintDialog printDlg = new PrintDialog();
                printDlg.PrintTicket.PageMediaSize = new PageMediaSize(Ticket.Width, Ticket.Height);
                printDlg.UserPageRangeEnabled = true;
                printDlg.PrintQueue = new PrintQueue(new PrintServer(), SelectedPrinter);
                printDlg.PrintTicket.CopyCount = Convert.ToInt32(NbrPrint);
                printDlg.PrintVisual(Ticket, "etiquette");
                PrintHistory printhist = new PrintHistory();
                printhist.heure = DateTime.Now.TimeOfDay.ToString();
                printhist.date = DateTime.Now.ToShortDateString();
                printhist.article = SelectedArticle.id;
                printhist.archive = 0;
                var f = new NumberFormatInfo { NumberGroupSeparator = " " };
                printhist.condi = Convert.ToInt32(qte.Replace(" ",""));
                printhist.raisonimpr = SelectedRaisonImpr.id;
                printhist.nbr = NbrPrint;
                _db.AddNewPrintHistory(printhist);
                var dbCred = _db.GetDBCred();
                if (dbCred.Count > 0)
                {
                    var mdbCred = dbCred[0];
                    mdbCred.novhistory = 1;
                    _db.UpdateNovHistory(mdbCred);
                }


                }
            catch(Exception ex)
            {
                
            }
            
        }

        private MvxObservableCollection<PrintCategorie> _RaisonImpression;

        public MvxObservableCollection<PrintCategorie> RaisonImpression
        {
            get { return _RaisonImpression; }
            set { _RaisonImpression = value;
                RaisePropertyChanged();
            }
        }

        private PrintCategorie _SelectedRaisonImpr;

        public PrintCategorie SelectedRaisonImpr
        {
            get { return _SelectedRaisonImpr; }
            set { _SelectedRaisonImpr = value; RaisePropertyChanged(); }
        }
        private int _NbrPrint=1;

        public int NbrPrint
        {
            get { return _NbrPrint; }
            set { _NbrPrint = value; }
        }

        public void UpdatePrintCategories()
        {
            RaisonImpression = new MvxObservableCollection<PrintCategorie>(_db.GetPrintCategories());
            SelectedRaisonImpr = RaisonImpression.FirstOrDefault();
        }

        private MvxObservableCollection<Article> _Articles;

        public MvxObservableCollection<Article> Articles
        {
            get { return _Articles; }
            set { _Articles = value;
                RaisePropertyChanged(); }
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

        private Article _SelectedArticle;

        public Article SelectedArticle
        {
            get { return _SelectedArticle; }
            set { _SelectedArticle = value;
                RaisePropertyChanged();
                if (value != null)
                    SetPrintingArticle();
            }
        }

        private string _designation;

        public string designation
        {
            get { return _designation; }
            set { _designation = value;
                RaisePropertyChanged();
            }
        }
        private string _EditDesignation;

        public string EditDesignation
        {
            get { return _EditDesignation; }
            set
            {
                _EditDesignation = value;
                RaisePropertyChanged();
                if (SelectedArticle != null)
                    SetPrintingArticle();
            }
        }

        private string _couleur;

        public string couleur
        {
            get { return _couleur; }
            set { _couleur = value;
                RaisePropertyChanged();
            }
        }

        private string _EditCouleur;

        public string EditCouleur
        {
            get { return _EditCouleur; }
            set { _EditCouleur = value;
                RaisePropertyChanged();
                if (SelectedArticle != null)
                    SetPrintingArticle();
            }
        }


        private string _largeur;

        public string largeur
        {
            get { return _largeur; }
            set { _largeur = value;
                RaisePropertyChanged();
            }
        }

        private string _EditLarg;

        public string EditLarg
        {
            get { return _EditLarg; }
            set { _EditLarg = value;
                RaisePropertyChanged();
                if (SelectedArticle != null)
                    SetPrintingArticle();
            }
        }

        private string _EditQte;

        public string EditQte
        {
            get { return _EditQte; }
            set { _EditQte = value;
                RaisePropertyChanged();
                if (SelectedArticle != null)
                    SetPrintingArticle();
            }
        }



        private string _qte;

        public string qte
        {
            get { return _qte; }
            set { _qte = value;
                RaisePropertyChanged();
            }
        }
        private string _unite;

        public string unite
        {
            get { return _unite; }
            set
            {
                _unite = value;
                RaisePropertyChanged();
            }
        }
        private string _composition;

        public string composition
        {
            get { return _composition; }
            set { _composition = value;

                RaisePropertyChanged();
            }
        }

        private string _EditComp;

        public string EditComp
        {
            get { return _EditComp; }
            set { _EditComp = value;
                RaisePropertyChanged();
                if(SelectedArticle!=null)
                    SetPrintingArticle();
            }
        }






        public void SetPrintingArticle()
        {
            
            List<PrintHistory> ArticleHistory= _db.GetHistoriqueArticleJour(SelectedArticle.id);
            int sumNbr = 0;
           foreach(PrintHistory mArticle in ArticleHistory)
            {
                sumNbr = sumNbr + mArticle.nbr;
            }
            HistJour = sumNbr.ToString();
            var f = new NumberFormatInfo { NumberGroupSeparator = " " };
            if (EditProp)
            {
                if(EditDesignation != null && !string.IsNullOrWhiteSpace(EditDesignation))
                {
                    designation = EditDesignation;
                }
                else
                {
                    designation = SelectedArticle.designation;
                }
                if(EditLarg!=null && !string.IsNullOrWhiteSpace(EditLarg))
                {
                    largeur = EditLarg;
                }
                else
                {
                    largeur = SelectedArticle.largeur.ToString();
                }
                var QteVariant = 0;
                if (EditQte != null && !string.IsNullOrWhiteSpace(EditQte) && Int32.TryParse(EditQte, out QteVariant))
                {
                    
                    qte = QteVariant.ToString("n0", f);
                }
                else
                {
                    qte = SelectedArticle.condi.ToString("n0", f);
                }
                
                
                
                unite = SelectedArticle.unite;
                if (SelectedArticle.couleurObj != null)
                {
                    if(EditCouleur != null && !string.IsNullOrWhiteSpace(EditCouleur))
                    {
                        couleur = EditCouleur;
                    }
                    else
                    {
                        couleur = SelectedArticle.couleurObj.nom;
                    }
                    
                }
                    
                if (SelectedArticle.compositionObj != null)
                {
                    if (EditComp != null && !string.IsNullOrWhiteSpace(EditComp))
                    {
                        composition = EditComp; 
                    }
                    else
                    {
                        composition = SelectedArticle.compositionObj.nom;
                    }
                }
                
            }
            else
            {
                designation = SelectedArticle.designation;
                largeur = SelectedArticle.largeur.ToString();
                qte = SelectedArticle.condi.ToString("n0", f);
                unite = SelectedArticle.unite;
                if (SelectedArticle.couleurObj != null)
                    couleur = SelectedArticle.couleurObj.nom;
                if (SelectedArticle.compositionObj == null)
                    return;
                composition = SelectedArticle.compositionObj.nom;
            }
            
        }

        private bool _EditProp;

        public bool EditProp
        {
            get { return _EditProp; }
            set { _EditProp = value;
                RaisePropertyChanged();
                if (SelectedArticle != null)
                    SetPrintingArticle();
            }
        }


        public void UpdateArticleList()
        {
            FullArticles = new MvxObservableCollection<Article>(_db.GetArticlesDetails());
            Articles = new MvxObservableCollection<Article>();
            Articles.ReplaceWith(FullArticles);
        }

        private MvxObservableCollection<Categorie> _categories;

        public MvxObservableCollection<Categorie> categories
        {
            get { return _categories; }
            set { _categories = value;
                RaisePropertyChanged();
            }
        }

        private Categorie _SelectedCategorie;

        public Categorie SelectedCategorie
        {
            get { return _SelectedCategorie; }
            set { _SelectedCategorie = value;
                RaisePropertyChanged();
                if (value != null)
                    FilterArticles();
            }
        }





        

        public void FilterArticles()
        {
           Articles.ReplaceWith(FullArticles.Where(art => art.categorie == SelectedCategorie.id).ToList());
            
        }

        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value;
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
                if(SelectedCategorie!=null)
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
                        || art.designation.ToLower().ToString().Contains(SearchText.ToLower())
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


        public void UpdateCategorieList()
        {
            categories = new MvxObservableCollection<Categorie>(_db.GetCategorie());
        }
    }
}
