using BackEnd.CustomClass;
using BackEnd.Data;
using BackEnd.model;
using Microsoft.Office.Interop.Excel;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BackEnd.viewmodel
{
    public class BusyIndicatorViewModel:MvxViewModel<string>
    {
        public readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public MvxInteraction<string> ShowError { get; } = new MvxInteraction<string>();
        public BusyIndicatorViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            
            _StockRefArticle = new List<string>();
            _StockRefArticle.Add("ttr");
            _StockRefArticle.Add("tt");
            _StockRefArticle.Add("sr58");
            _StockRefArticle.Add("tr");
            _StockRefArticle.Add("ss");
            _StockRefArticle.Add("re");
            _StockRefArticle.Add("gc");
            _StockRefArticle.Add("gb");
            _StockRefArticle.Add("rm");
            ListCategorie = new MvxObservableCollection<Categorie>(_db.GetCategorie());
            ListComposition = new MvxObservableCollection<composition>(_db.GetCompositions());
            ListCouleur = new MvxObservableCollection<Couleur>(_db.GetCouleurs());
        }
        private MvxObservableCollection<composition> _ListComposition;

        public MvxObservableCollection<composition> ListComposition
        {
            get { return _ListComposition; }
            set { _ListComposition = value; RaisePropertyChanged(); }
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
            set { _ListCategorie = value; RaisePropertyChanged(); }
        }
        private List<string> _StockRefArticle;


        private string _IndicatorMsg;

        public string IndicatorMsg
        {
            get { return _IndicatorMsg; }
            set { _IndicatorMsg = value;
                RaisePropertyChanged(); }
        }

        private int _ProgressValue;

        public int ProgressValue
        {
            get { return _ProgressValue; }
            set { _ProgressValue = value;
                RaisePropertyChanged();
            }
        }

        private int _TotalArticle;

        public int TotalArticle
        {
            get { return _TotalArticle; }
            set
            {
                _TotalArticle = value; RaisePropertyChanged();
            }
        }
        private int _DownloadedArticle;

        public int DownloadedArticle
        {
            get { return _DownloadedArticle; }
            set
            {
                _DownloadedArticle = value;
                RaisePropertyChanged();
            }
        }

        public void SetIndicatorValues(object o, ProgressChangedEventArgs  args)
        {
            int prog = args.ProgressPercentage;
            IndicatorMsg = "Téléchargement Article " + DownloadedArticle + "/ " + TotalArticle;
            ProgressValue =Convert.ToInt32(((double)DownloadedArticle / TotalArticle) * 100);
        }
        private DispatcherTimer dispatcherTimer;
        private BackgroundWorker BW;

        
        public override void ViewAppeared()
        {
            base.ViewAppeared();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += StartBackGroundWorker;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

          


        }
        
        public void StartBackGroundWorker(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            BW = new BackgroundWorker();
            BW.DoWork += ExtractArticles;
            BW.RunWorkerCompleted += CloseView;

            BW.WorkerReportsProgress = true;
            BW.ProgressChanged += SetIndicatorValues;
            BW.RunWorkerAsync();
        }

        public void CloseView(object sender, EventArgs e)
        {
            _navigationService.Close(this);
        }

        public void ExtractArticles(object sender, EventArgs e)
        {

            BackgroundWorker worker = (BackgroundWorker)sender;

            int RowIndex = 0;
                int ColumnIndex = 0;
                Application excelApp = excelApp = new Application();
                Workbook wk = excelApp.Workbooks.Open(FullPath); ;
                List<string> Headers = new List<string>();

                try
                {


                    _Worksheet sheet = (_Worksheet)wk.Worksheets["A"];
                    Range rang = sheet.ListObjects["art"].DataBodyRange.Rows;


                    for (int j = 1; j < rang.Columns.Count; j++)
                    {
                        var col = (rang.Cells[0, j] as Range).Value2;
                        Headers.Add(col.ToString());
                    }
                    Range column = rang;
                TotalArticle = rang.Rows.Count;
                    foreach (Range c in rang)
                    {
                    RowIndex++;
                    DownloadedArticle = RowIndex;
                    worker.ReportProgress((RowIndex * 100) / rang.Count);
                    if (c == null)
                            return;
                        column = c;
                        Article art = new Article();
                        int CategorieIndex = Headers.IndexOf(Headers.First(h => h.ToLower().Equals("classe1"))) + 1;
                        var class1 = (c.Cells[1, CategorieIndex] as Range).Value2;
                        if (Convert.ToInt32(class1) != 2)
                            continue;
                        ColumnIndex++;



                        int IDIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("idarticle")));
                        var cell = (c.Cells[1, IDIndex + 1] as Range).Value2;
                        art.idarticle = Convert.ToInt32(cell);
                        if (_db.GetArticleByIDArticle(art.idarticle).Count > 0)
                            continue;
                        int RefIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("ref")));
                        cell = (c.Cells[1, RefIndex + 1] as Range).Value2;
                        art.refarticle = cell.ToString();
                        
                        if (_StockRefArticle.FirstOrDefault(refstock => art.refarticle.ToLower().Contains(refstock)) != null)
                        {

                            art.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("stock")).id;
                            var colNum=art.refarticle.Substring(art.refarticle.Count() - 2);
                        int ColNumber = 0;
                        bool feasible = Int32.TryParse(colNum, out ColNumber);
                            if(feasible)
                        {
                            var col = ListCouleur.FirstOrDefault(coul => coul.numero == ColNumber);
                            if (col != null)
                                art.couleur = col.id;
                        }
                        }
                        else
                        {
                            art.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("diver")).id;
                        }
                        int DesIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("designation")));
                        cell = (c.Cells[1, DesIndex + 1] as Range).Value2;
                        art.designation = cell.ToString();
                        art.nom = cell.ToString();
                        if(art.designation.ToLower().Contains("coton"))
                        {
                        var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("cot"));
                        if(compObj!=null)
                            art.composition = compObj.id;

                        }else if(art.designation.ToLower().Contains("elastique"))
                        {
                        var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("gom"));
                        if (compObj != null)
                            art.composition = compObj.id;
                    }
                     else  
                     {
                        var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("pp") && comp.nom.ToLower().Contains("ps"));
                        if (compObj != null)
                            art.composition = compObj.id;
                    }
                    string resultString = Regex.Match(art.designation, @"\d+").Value;
                        int larg = 0;
                        bool b = Int32.TryParse(resultString, out larg);
                        if (b)
                        {
                            art.largeur = larg;
                            
                        }else if(art.refarticle.ToLower().Contains("gb"))
                        {
                         art.largeur = 25;
                        }
                    int StockIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("stockq")));
                        cell = (c.Cells[1, StockIndex + 1] as Range).Value2;
                        art.qtestock = Convert.ToInt32(cell);
                        int StockInitIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("stockinvq")));
                        cell = (c.Cells[1, StockInitIndex + 1] as Range).Value2;
                        art.qtestockinit = Convert.ToInt32(cell);

                        int unitIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("unit")));
                        cell = (c.Cells[1, unitIndex + 1] as Range).Value2;
                        art.unite = cell.ToString();
                        int venteIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("ventq")));
                        cell = (c.Cells[1, venteIndex + 1] as Range).Value2;
                        art.vente = Convert.ToInt32(cell);
                        int condiIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("condi")));
                        cell = (c.Cells[1, condiIndex + 1] as Range).Value2;
                        art.condi = Convert.ToInt32(cell);
                        int clientIndex = Headers.IndexOf(Headers.FirstOrDefault(h => h.ToLower().Equals("code")));
                        cell = (c.Cells[1, clientIndex + 1] as Range).Value2;
                        art.client = cell.ToString();
                        if (art.client.ToLower().Contains("sv"))
                        {

                            art.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("stock")).id;
                        }
                        else if (art.client.ToLower().Contains("ehc"))
                        {
                            art.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("ehc")).id;
                        }
                        _db.AddNewArticleFromExcel(art);


                    }

                }
                catch (Exception ex)
                {

                    ShowError.Raise("index col:" + ColumnIndex + " Index Row:" + RowIndex + ex.ToString());
                }
                finally
                {
                    wk.Close(0);
                    excelApp.Quit();
                    
                }
           
        }
        public string FullPath;
        public override void Prepare(string parameter)
        {
            FullPath =parameter;
            
        }
    }
}
