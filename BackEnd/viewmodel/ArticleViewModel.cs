using BackEnd.CustomClass;
using BackEnd.Data;
using BackEnd.model;
using Microsoft.Office.Interop.Excel;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackEnd.viewmodel
{
    public class ArticleViewModel:MvxViewModel<string>
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;

        private BusyIndicatorViewModel _IndicatorViewModel;

        public BusyIndicatorViewModel IndicatorViewModel
        {
            get { return _IndicatorViewModel; }
            set { _IndicatorViewModel = value; }
        }

        private IMvxCommand _ImportExcelColor;

        public IMvxCommand ImportExcelColor
        {
            get {
                _ImportExcelColor = new MvxAsyncCommand(StartBackGroundWorker);
                return _ImportExcelColor; }

        }
        private IMvxCommand _ImportExcelRaison;

        public IMvxCommand ImportExcelRaison
        {
            get
            {
                _ImportExcelRaison = new MvxAsyncCommand(StartBackGroundWorkerRaison);
                return _ImportExcelRaison;
            }

        }
        public async Task StartBackGroundWorkerRaison()
        {

            var req = new UploadFile
            {
                UploadCallback = (FullPath, FileName, ok) =>
                {
                    if (ok)
                    {
                        FilePath = FullPath;

                    }
                }

            };
            GetFilePath.Raise(req);

            BackgroundWorker BW = new BackgroundWorker();
            BW.DoWork += ImportExcelPrintCategorie;
            BW.RunWorkerCompleted += UpdateCategoriePrintList;
            BW.RunWorkerAsync();
        }
        public void ImportExcelPrintCategorie(object sender, EventArgs e)
        {
            if (FilePath == null || string.IsNullOrWhiteSpace(FilePath))
                return;
            Application excelApp = excelApp = new Application();
            Workbook wk = excelApp.Workbooks.Open(FilePath); ;

            try
            {


                _Worksheet sheet = (_Worksheet)wk.Worksheets["raison"];
                Range rang = sheet.ListObjects["raison"].DataBodyRange.Rows;



                Range column = rang;
                foreach (Range c in rang)
                {
                    if (c == null)
                        return;
                    column = c;
                    PrintCategorie mCatPrint = new PrintCategorie();
                    var Nomcat = (c.Cells[1, 1] as Range).Value2;
                    mCatPrint.name = Nomcat.ToString();

                    _db.AddNewPrintCategorie(mCatPrint);
                }

            }
            catch (Exception ex)
            {

                ShowError.Raise(ex.ToString());
            }
            finally
            {
                wk.Close(0);
                excelApp.Quit();

            }
        }

        private string _NumContrat;

        public string NumContrat
        {
            get { return _NumContrat; }
            set { _NumContrat = value;
                RaisePropertyChanged();
            }
        }
        private string _NovNumContrat;

        public string NovNumContrat
        {
            get { return _NovNumContrat; }
            set
            {
                _NovNumContrat = value;
                RaisePropertyChanged();
            }
        }
        private MvxObservableCollection<contrat> _ListContrat;

        public MvxObservableCollection<contrat> ListContrat
        {
            get { return _ListContrat; }
            set { _ListContrat = value;
                RaisePropertyChanged();
            }
        }

        private MvxObservableCollection<Article> _ListLibre;

        public MvxObservableCollection<Article> ListLibre
        {
            get { return _ListLibre; }
            set { _ListLibre = value;
                RaisePropertyChanged();
            }
        }
        private MvxObservableCollection<Article> _EHCArticles;

        public MvxObservableCollection<Article> EHCArticles
        {
            get { return _EHCArticles; }
            set
            {
                _EHCArticles = value;
                RaisePropertyChanged();
            }
        }
        private MvxObservableCollection<Article> _ListAffecter;

        public MvxObservableCollection<Article> ListAffecter
        {
            get { return _ListAffecter; }

            set { _ListAffecter = value;
                RaisePropertyChanged();
            }
        }
        private Article _SelectedLibreArticle;

        public Article SelectedLibreArticle
        {
            get { return _SelectedLibreArticle; }
            set { _SelectedLibreArticle = value;
                RaisePropertyChanged();
            }
        }

        private Article _SelectedAffecterArticle;

        public Article SelectedAffecterArticle
        {
            get { return _SelectedAffecterArticle; }
            set { _SelectedAffecterArticle = value;
                RaisePropertyChanged();
            }
        }



        private contrat _SelectedContrat;

        public contrat SelectedContrat
        {
            get { return _SelectedContrat; }
            set { _SelectedContrat = value;
                if (value != null)
                    SetContratArticles();
                RaisePropertyChanged(); }
        }
        private IMvxCommand _EnleverCmd;

        public IMvxCommand EnleverCmd
        {
            get
            {
                _EnleverCmd = new MvxCommand(LibererArticle);
                return _EnleverCmd;
            }
        }
        public void LibererArticle()
        {
            if (SelectedAffecterArticle == null || SelectedContrat == null)
                return;
            var ConArt = new contratarticle();
            ConArt.idcontrat = SelectedContrat.id;
            ConArt.article = SelectedAffecterArticle.id;
            _db.DeleteArticleContrat(ConArt);
            ListLibre.Add(SelectedAffecterArticle);
            ListAffecter.Remove(SelectedAffecterArticle);
        }
        private IMvxCommand _AffecterCmd;

        public IMvxCommand AffecterCmd
        {
            get {
                _AffecterCmd = new MvxCommand(AffectNewArticle);
                return _AffecterCmd; }
        }
        public void AffectNewArticle()
        {
            if (SelectedLibreArticle == null || SelectedContrat == null)
                return;
            var ConArt = new contratarticle();
            ConArt.idcontrat = SelectedContrat.id;
            ConArt.article = SelectedLibreArticle.id;
            _db.AddNewArticleContrat(ConArt);
            ListAffecter.Add(SelectedLibreArticle);
            ListLibre.Remove(SelectedLibreArticle);
            
        }
        public void SetContratArticles()
        {
           var ehcCat= ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Contains("ehc"));
            if (ehcCat == null)
                return;
            EHCArticles = new MvxObservableCollection<Article>(_db.GetArticlesByCategorie(ehcCat));
            List<contratarticle> affectedArt =_db.GetContratArticles(SelectedContrat);
            ListAffecter = new MvxObservableCollection<Article>();
            ListLibre = new MvxObservableCollection<Article>();
            foreach (Article art in EHCArticles)
            {
                if(affectedArt.FirstOrDefault(aa=>aa.article==art.id)!=null)
                {
                    ListAffecter.Add(art);
                }
                else
                {
                    ListLibre.Add(art);
                }
            }
           


        }
        private IMvxCommand _AjouterNovContrat;

        public IMvxCommand AjouterNovContrat
        {
            get {
                _AjouterNovContrat = new MvxCommand(AjouterNouveauContrat);
                return _AjouterNovContrat; }
        }

        public void AjouterNouveauContrat()
        {
            if (NumContrat == null
                || string.IsNullOrWhiteSpace(NumContrat))
                return;
            var mContrat = new contrat();
            mContrat.nomcontrat = NumContrat;
            _db.AddNewContrat(mContrat);
            UpdateContratList();

        }

        private IMvxCommand _CancelEditContrat;

        public IMvxCommand CancelEditContrat
        {

            get {
                _CancelEditContrat = new MvxCommand(CancelContratEdit);
                return _CancelEditContrat; }
        }
        public void CancelContratEdit()
        {
            EnableContratEdit = false;
            NovNumContrat = "";
        }

        private IMvxCommand _SaveContratChange;

        public IMvxCommand SaveContratChange
        {
            get {
                _SaveContratChange = new MvxCommand(UpdateContrat);
                return _SaveContratChange; }
        }
        private bool _EnableContratEdit;

        public bool EnableContratEdit
        {
            get { return _EnableContratEdit; }
            set { _EnableContratEdit = value;
                RaisePropertyChanged();

            }
        }
        private int EditContratID;
        public void UpdateContrat()
        {
            if (NovNumContrat== null || string.IsNullOrWhiteSpace(NovNumContrat))
                return;
            var mContrat = new contrat();
            mContrat.id = EditContratID;
            mContrat.nomcontrat = NovNumContrat;
            _db.UpdateContrat(mContrat);
            NovNumContrat = "";
            EnableContratEdit = false;
            UpdateContratList();
            
        }

        public void UpdateContratList()
        {
            ListContrat = new MvxObservableCollection<contrat>(_db.GetContrats());
        }
        private IMvxCommand _ActiverContrat;

        public IMvxCommand ActiverContrat
        {
            get
            {
                _ActiverContrat = new MvxCommand(ActivationContrat);
                return _ActiverContrat;
            }
        }
        private IMvxCommand _ModifierContrat;

        public IMvxCommand ModifierContrat
        {
            get {
                _ModifierContrat = new MvxCommand(SetupEditFields);
                return _ModifierContrat; }
        }

        public void SetupEditFields()
        {
            if (SelectedContrat == null)
                return;
            NovNumContrat = SelectedContrat.nomcontrat;
            EditContratID = SelectedContrat.id;
            EnableContratEdit = true;
        }

        private IMvxCommand _SupprimerContrat;

        public IMvxCommand SupprimerContrat
        {
            get {
                _SupprimerContrat = new MvxCommand(DeleteContrat);
                return _SupprimerContrat; }
        }

        public void DeleteContrat()
        {
            if (SelectedContrat == null)
                ShowError.Raise("Séléctionnez une contrat");
            var confirm = new YesNoQuestion
            {
                Question = "êtes vous sur de vouloir supprimer cette contrat",
                YesNoCallback = (ok) =>
                {
                    if (ok)
                    {
                        _db.DeleteContrat(SelectedContrat);
                        UpdateContratList();
                    }
                }



            };
            ConfirmAction.Raise(confirm);
        }


        private IMvxCommand _DisactiverContrat;

        public IMvxCommand DisactiverContrat
        {
            get {
                _DisactiverContrat = new MvxCommand(DésactiverContrat);
                return _DisactiverContrat; }
        }
        public void DésactiverContrat()
        {
            if (SelectedContrat == null)
                return;
            SelectedContrat.activer = 0;
            _db.UpdateContratState(SelectedContrat);

        }
        public void ActivationContrat()
        {
            if (SelectedContrat == null)
                return;
            SelectedContrat.activer = 1;
            _db.UpdateContratState(SelectedContrat);
        }

        public void UpdateCategoriePrintList(object sender, EventArgs e)
        {
            ListPrintCategorie = new MvxObservableCollection<PrintCategorie>(_db.GetPrintCategories());

        }
        private IMvxCommand _ImportExcelComposition;

        public IMvxCommand ImportExcelComposition
        {
            get
            {
                _ImportExcelComposition = new MvxAsyncCommand(StartBackGroundWorkerComposition);
                return _ImportExcelComposition;
            }

        }
        public async Task StartBackGroundWorkerComposition()
        {

            var req = new UploadFile
            {
                UploadCallback = (FullPath, FileName, ok) =>
                {
                    if (ok)
                    {
                        FilePath = FullPath;

                    }
                }

            };
            GetFilePath.Raise(req);

            BackgroundWorker BW = new BackgroundWorker();
            BW.DoWork += ImportExcelCompositions;
            BW.RunWorkerCompleted += UpdateCompositionList;
            BW.RunWorkerAsync();
        }
        public void ImportExcelCompositions(object sender, EventArgs e)
        {
            if (FilePath == null || string.IsNullOrWhiteSpace(FilePath))
                return;
            Application excelApp = excelApp = new Application();
            Workbook wk = excelApp.Workbooks.Open(FilePath); ;

            try
            {


                _Worksheet sheet = (_Worksheet)wk.Worksheets["composition"];
                Range rang = sheet.ListObjects["composition"].DataBodyRange.Rows;



                Range column = rang;
                foreach (Range c in rang)
                {
                    if (c == null)
                        return;
                    column = c;
                    composition mComp = new composition();
                    var NomComp = (c.Cells[1, 1] as Range).Value2;
                    mComp.nom = NomComp.ToString();

                    _db.AddNewComposition(mComp);
                }

            }
            catch (Exception ex)
            {

                ShowError.Raise(ex.ToString());
            }
            finally
            {
                wk.Close(0);
                excelApp.Quit();

            }
        }
        public void UpdateCompositionList(object sender, EventArgs e)
        {
            ListComposition = new MvxObservableCollection<composition>(_db.GetCompositions());
        }
        private string FilePath = "";
        public async Task StartBackGroundWorker()
        {
            
            var req = new UploadFile
            {
                UploadCallback = (FullPath, FileName, ok) =>
                {
                    if (ok)
                    {
                        FilePath = FullPath;

                    }
                }

            };
            GetFilePath.Raise(req);

            BackgroundWorker BW = new BackgroundWorker();
            BW.DoWork += ImportExcelColors;
            BW.RunWorkerCompleted += UpdateColorList;
            BW.RunWorkerAsync();
        }
        public void UpdateColorList(object sender, EventArgs e)
        {
            var listColor = _db.GetCouleurs();
            foreach (var col in listColor) col.nom = col.nom.ToUpper();


            ListCouleur = new MvxObservableCollection<Couleur>(listColor.ToList());
        }
        public void ImportExcelColors(object sender, EventArgs e)
        {
            if (FilePath == null || string.IsNullOrWhiteSpace(FilePath))
                return;
            Application excelApp = excelApp = new Application();
            Workbook wk = excelApp.Workbooks.Open(FilePath); ;

            try
            {


                _Worksheet sheet = (_Worksheet)wk.Worksheets["couleur"];
                Range rang = sheet.ListObjects["couleur"].DataBodyRange.Rows;


                
                Range column = rang;
                foreach (Range c in rang)
                {
                    if (c == null)
                        return;
                    column = c;
                    Couleur mCouleur = new Couleur();
                    var NumCouleur = (c.Cells[1, 1] as Range).Value2;
                    var NomCouleur = (c.Cells[1, 2] as Range).Value2;
                    mCouleur.numero =Convert.ToInt32(NumCouleur);
                    mCouleur.nom = NomCouleur.ToString();

                    _db.AddNewCouleur(mCouleur);
                }

            }
            catch (Exception ex)
            {

                ShowError.Raise(ex.ToString());
            }
            finally
            {
                wk.Close(0);
                excelApp.Quit();

            }
        }

        public MvxInteraction<UploadFile> GetFilePath { get; } = new MvxInteraction<UploadFile>();
        public MvxInteraction<string> ShowError { get; } = new MvxInteraction<string>();

        public MvxInteraction<YesNoQuestion> ConfirmAction { get; } = new MvxInteraction<YesNoQuestion>();
        public ArticleViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            UpdateCategorieList();
            UpdateArticleList();
            UpdateCompositionList();
            UpdateColorList();
            UpdateCategoriePrintList();
            UpdateContratList();
            _StockRefArticle = new List<string>();
            _StockRefArticle.Add("ttr");
            _StockRefArticle.Add("tt");
            _StockRefArticle.Add("tr");
            _StockRefArticle.Add("ss");
            _StockRefArticle.Add("re");
            _StockRefArticle.Add("gc");
            _StockRefArticle.Add("gb");
            _StockRefArticle.Add("rm");
        }
        private MvxObservableCollection<PrintCategorie> _ListPrintCategorie;

        public MvxObservableCollection<PrintCategorie> ListPrintCategorie
        {
            get { return _ListPrintCategorie; }
            set
            {
                _ListPrintCategorie = value;
                RaisePropertyChanged();

            }
        }
        private string _PrintCategorieName;

        public string PrintCategorieName
        {
            get { return _PrintCategorieName; }
            set { _PrintCategorieName = value;
                RaisePropertyChanged(); }
        }

        private IMvxCommand _AjouterPrintCategorie;

        public IMvxCommand AjouterPrintCategorie
        {
            get {
                _AjouterPrintCategorie = new MvxCommand(AddNewPrintCategorie);
                return _AjouterPrintCategorie; }

        }

        public void AddNewPrintCategorie()
        {
            if (PrintCategorieName == null &&
                string.IsNullOrWhiteSpace(PrintCategorieName))
                return;
            PrintCategorie printCat = new PrintCategorie();
            printCat.name = PrintCategorieName;
            if (_db.GetPrintCategorie(printCat).Count > 0)
            {
                ShowError.Raise("Catégorie existe déja");
                return;
            }
            _db.AddNewPrintCategorie(printCat);
            PrintCategorieName = "";
            UpdateCategoriePrintList();

        }

        private string _Designation;

        public string Designation
        {
            get
            {
                return _Designation;
            }
            set
            {
                _Designation = value;
                RaisePropertyChanged();
            }
        }
        private string _NovDesignation;

        public string NovDesignation
        {
            get
            {
                return _NovDesignation;
            }
            set
            {
                _NovDesignation = value;
                RaisePropertyChanged();
            }
        }

        private Categorie _SelectedCat;

        public Categorie SelectedCat
        {
            get { return _SelectedCat; }
            set { _SelectedCat = value;
                RaisePropertyChanged();
            }
        }
        private Categorie _SelectedCatSearch;

        public Categorie SelectedCatSearch
        {
            get { return _SelectedCatSearch; }
            set { _SelectedCatSearch = value; RaisePropertyChanged();
                if (value != null)
                    FilterArticles();
            }
        }

        public void FilterArticles()
        {
            Articles =new MvxObservableCollection<Article>(FullArticles.Where(art => art.categorieObj.id == SelectedCatSearch.id).ToList());
        }


        private Couleur _SelectedCoul;

        public Couleur SelectedCoul
        {
            get { return _SelectedCoul; }
            set { _SelectedCoul = value;
                RaisePropertyChanged();
            }
        }

        private composition _SelectedComp;

        public composition SelectedComp
        {
            get { return _SelectedComp; }
            set { _SelectedComp = value;
                RaisePropertyChanged();
            }
        }

        private string _LargeurValue;

        public string LargeurValue
        {
            get { return _LargeurValue; }
            set { _LargeurValue = value;
                RaisePropertyChanged();
            }
        }
        private string _ProdName;

        public string ProdName
        {
            get { return _ProdName; }
            set
            {
                _ProdName = value;
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
                    SetSelectedArticleProperties();
            }
        }

        public void SetSelectedArticleProperties()
        {
            if (SelectedArticle.categorieObj != null)
                SelectedCat = ListCategorie.FirstOrDefault(cat => cat.id == SelectedArticle.categorieObj.id);
            else
            {
                SelectedCat = null;
            }
            if (SelectedArticle.compositionObj != null)
                SelectedComp = ListComposition.FirstOrDefault(compo => compo.id == SelectedArticle.compositionObj.id);
            else
            {
                SelectedComp = null;
            }

            if (SelectedArticle.couleurObj != null)
                SelectedCoul = ListCouleur.FirstOrDefault(coul => coul.id == SelectedArticle.couleurObj.id);
            else
            {
                SelectedCoul = null;
            }
            ProdName = SelectedArticle.nom;
            LargeurValue = SelectedArticle.largeur.ToString();
        }

        private IMvxCommand _EditArticleCmd;

        public IMvxCommand EditArticleCmd
        {
            get {
                _EditArticleCmd = new MvxCommand(AppliquerModificationArticle);
                return _EditArticleCmd; }
        }

        public void AppliquerModificationArticle()
        {
            try
            {
                if(SelectedCat==null)
                {
                    ShowError.Raise("Séléctionnez un catégorie");
                    return;
                }
                if (SelectedComp == null)
                {
                    ShowError.Raise("Séléctionnez une composition");
                    return;
                }
                if (SelectedCoul == null)
                {
                    ShowError.Raise("Séléctionnez une Couleur");
                    return;
                }
                SelectedArticle.categorieObj = SelectedCat;
                SelectedArticle.compositionObj = SelectedComp;
                SelectedArticle.couleurObj = SelectedCoul;
                SelectedArticle.nom = ProdName;
                SelectedArticle.largeur = Convert.ToInt32(LargeurValue);
                _db.UpdateArticlePrintProperties(SelectedArticle);
                UpdateArticleList();
                ResetPrintingProperties();
            }
            catch(Exception ex)
            {
                ShowError.Raise(ex.ToString());
            }
        }
        public void ResetPrintingProperties()
        {
            SelectedCat = null;
            SelectedComp = null;
            SelectedCoul = null;
            LargeurValue = "0";
            ProdName = "";
        }

        private MvxObservableCollection<Couleur> _ListCouleur;

        public MvxObservableCollection<Couleur> ListCouleur
        {
            get { return _ListCouleur; }
            set { _ListCouleur = value; RaisePropertyChanged(); }
        }

        private MvxObservableCollection<Categorie> _ListCategorie;

        public MvxObservableCollection<Categorie> ListCategorie
        {
            get { return _ListCategorie; }
            set { _ListCategorie = value;RaisePropertyChanged(); }
        }

        private MvxObservableCollection<Article> _FullArticles;

        public MvxObservableCollection<Article> FullArticles
        {
            get { return _FullArticles; }
            set { _FullArticles = value; }
        }


        private MvxObservableCollection<Article> _Articles;

        public MvxObservableCollection<Article> Articles
        {
            get { return _Articles; }
            set { _Articles = value; RaisePropertyChanged(); }
        }
        private IMvxCommand _AjouterNovComposition;

        public IMvxCommand AjouterNovComposition
        {
            get
            {
                _AjouterNovComposition = new MvxCommand(AjouterNouvelleComposition);
                return _AjouterNovComposition;

            }
        }

        private IMvxCommand _AjouterNovCategorie;

        public IMvxCommand AjouterNovCategorie
        {
            get
            {
                _AjouterNovCategorie = new MvxCommand(AjouterNouvelleCategorie);
                return _AjouterNovCategorie;

            }
        }
        private string _CompNom;

        public string CompNom
        {
            get { return _CompNom; }
            set { _CompNom = value;
                RaisePropertyChanged();
            }
        }

        public void AjouterNouvelleComposition()
        {
            if (CompNom == null &&
                string.IsNullOrWhiteSpace(CompNom))
                return;
            composition comp = new composition();
            comp.nom = CompNom;
            if (_db.GetComposition(CompNom).Count > 0)
            {
                ShowError.Raise("Composition existe déja");
                return;
            }

            _db.AddNewComposition(comp);
            CompNom = "";
            UpdateCompositionList();



        }

        private string _Numero;

        public string Numero
        {
            get { return _Numero; }
            set { _Numero = value;
                RaisePropertyChanged();
            }
        }

        private string _CouleurNom;

        public string CouleurNom
        {
            get { return _CouleurNom; }
            set { _CouleurNom = value;
                RaisePropertyChanged();
            }
        }

        private IMvxCommand _AjouterNovCol;

        public IMvxCommand AjouterNovCol
        {
            get {
                _AjouterNovCol = new MvxCommand(AjouterNouvelleCouleur);
                return _AjouterNovCol; }
        }
        public bool IsAddFieldsEmpty()
        {
            return Numero == null || CouleurNom == null || string.IsNullOrWhiteSpace(Numero) ||
                   string.IsNullOrWhiteSpace(CouleurNom);
        }
        public bool IsNumero(string Num)
        {
            try
            {
                Convert.ToInt32(Num);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void  UpdateColorList()
        {
            var listColor = _db.GetCouleurs();
            foreach (var col in listColor) col.nom = col.nom.ToUpper();


            ListCouleur = new MvxObservableCollection<Couleur>(listColor.ToList());
        }
        public void AjouterNouvelleCouleur()
        {
            

                if (!IsAddFieldsEmpty() && IsNumero(Numero) &&
                    _db.GetCouleur(new Couleur(Convert.ToInt32(Numero), CouleurNom)).Count==0)
                {
                    var NewColour = new Couleur();
                    NewColour.numero = Convert.ToInt32(Numero);
                    NewColour.nom = CouleurNom;
                    _db.AddNewCouleur(NewColour);
                     UpdateColorList();
                    Numero = "";
                    CouleurNom = "";
                }
                else if (IsAddFieldsEmpty())
                {
                    ShowError.Raise("Remplit tout les champs");
                }
                else if (!IsNumero(Numero))
                {
                ShowError.Raise("Choisir un numero correct");
                }
                else
                {
                ShowError.Raise("Couleur existe déja");
                }
            }


        public void AjouterNouvelleCategorie()
        {
            if (Designation == null &&
                string.IsNullOrWhiteSpace(Designation))
                return;
            Categorie cat = new Categorie();
            cat.name = Designation;
           if( _db.GetCategorie(Designation).Count>0)
            {
                ShowError.Raise("Catégorie existe déja");
                return;
            }

            _db.AddNewCategorie(cat);
            Designation = "";
            UpdateCategorieList();
           


        }

        private List<string> _StockRefArticle;

        private MvxObservableCollection<composition> _ListComposition;

        public MvxObservableCollection<composition> ListComposition
        {
            get { return _ListComposition; }
            set { _ListComposition = value;
                RaisePropertyChanged();
            }
        }


        public void UpdateCompositionList()
        {
            ListComposition = new MvxObservableCollection<composition>(_db.GetCompositions());
        }
        public void UpdateCategoriePrintList()
        {
            ListPrintCategorie = new MvxObservableCollection<PrintCategorie>(_db.GetPrintCategories());
          
        }
        public void UpdateCategorieList()
        {
            ListCategorie=new MvxObservableCollection<Categorie>(_db.GetCategorie());
        }

       



        public void UpdateArticleList()
        {
            FullArticles = new MvxObservableCollection<Article>(_db.GetArticlesDetails());
            if(SelectedCatSearch!=null)
                Articles = new MvxObservableCollection<Article>(FullArticles.Where(art=>art.categorieObj.id==SelectedCatSearch.id));
            else
            {
                 Articles = FullArticles;
             }
        }
        private IMvxAsyncCommand _ImportCmd;

        public IMvxAsyncCommand ImportCmd
        {
            get
            {
                _ImportCmd = new MvxAsyncCommand(ImportationExcel);
                return _ImportCmd;
                    
            }
        }
        
       
        public async Task ImportationExcel()
        {
            if(PassCode.Equals("superuser"))
            {
               


                await _navigationService.Navigate<BusyIndicatorViewModel, string>("superuser");

                UpdateArticleList();
            }
            else
            {
                string FilePath = "";
                var req = new UploadFile
                {
                    UploadCallback = (FullPath, FileName, ok) =>
                    {
                        if (ok)
                        {
                            FilePath = FullPath;

                        }
                    }

                };
                GetFilePath.Raise(req);
                if(FilePath==null || string.IsNullOrEmpty(FilePath))
                {
                    return;
                }
                await _navigationService.Navigate<BusyIndicatorViewModel, string>(FilePath);

                UpdateArticleList();
            }
           
        }
        private string PassCode;
        public bool IsEditArticle;
        public override void Prepare(string parameter)
        {
            PassCode = parameter;
        }

        #region Methods
        public void AjouterArticle()
        {
            IsEditArticle = false;
            _navigationService.Navigate<AddArticleViewModel,ArticleViewModel>(this);
        }
        public void ModifierArticle()
        {
            if(SelectedArticle==null)
            {
                ShowError.Raise("Aucun Article Séléctionner");
                return;
            }
            IsEditArticle = true;
            _navigationService.Navigate<AddArticleViewModel, ArticleViewModel>(this);
        }
        public void SupprimerArticle()
        {
            var req = new YesNoQuestion
            {
                Question = "êtes vous sur de vouloir supprimer cet article",
                YesNoCallback = (ok) =>
                {
                    if (ok)
                    {
                        _db.DeleteArticleByID(SelectedArticle.id);
                        UpdateArticleList();
                    }
                }

            };
            ConfirmAction.Raise(req);
        }
        #endregion
    }

}
