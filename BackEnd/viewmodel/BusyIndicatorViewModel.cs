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
using System.Data;
using System.Data.OleDb;
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
            if(FullPath.Equals("superuser"))
            {
                    var dbCred = _db.GetDBCred();
                    if (dbCred.Count > 0)
                    {
                        
                            mDBCred = new DBCred();
                            mDBCred.id = dbCred[0].id;
                            mDBCred.source = dbCred[0].source;
                            mDBCred.catalog = dbCred[0].catalog;
                            SetConnectionString();
                        

                    
                }
                BW.DoWork += ExtractArticlesFromDB;
            }
            else
            {
                BW.DoWork += ExtractArticles;
            }
           
            BW.RunWorkerCompleted += CloseView;

            BW.WorkerReportsProgress = true;
            BW.ProgressChanged += SetIndicatorValues;
            BW.RunWorkerAsync();
        }

        public void CloseView(object sender, EventArgs e)
        {
            _navigationService.Close(this);
        }
        private string connectionString;
        private DBCred mDBCred;
        public void SetConnectionString()
        {
            connectionString = @"Provider=PCSOFT.HFSQL;Data Source=" + mDBCred.source + ";user id=admin;Initial Catalog=" + mDBCred.catalog;
        }
        private MvxObservableCollection<Article> _AllArticles;

        public MvxObservableCollection<Article> AllArticles
        {
            get { return _AllArticles; }
            set { _AllArticles = value; }
        }
        public void ExtractArticlesFromDB(object sender, EventArgs e)
        {

            BackgroundWorker worker = (BackgroundWorker)sender;
            try
            {
                AllArticles = new MvxObservableCollection<Article>(_db.GetArticles());
                string sql = @"SELECT IDArticle,ref,designation,stockq,ventq,stockinvq,code,unit,condi FROM BL_" + DateTime.Now.Year + "_Article where classe1=2"; //MyTable = The .FIC file
                int RowIndex = 0;
                System.Data.DataTable table = new System.Data.DataTable();
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
                    {
                        adapter.Fill(table); //Fill the table with the extracted data
                        TotalArticle = table.Rows.Count;
                        foreach (DataRow row in table.Rows)
                        {
                            RowIndex++;
                            DownloadedArticle = RowIndex;
                            worker.ReportProgress((RowIndex * 100) / table.Rows.Count);
                            var Marticle = new Article();
                            Marticle.idarticle = Convert.ToInt32(row["IDArticle"].ToString());
                            Marticle.refarticle = row["ref"].ToString();
                            if (AllArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle) != null)
                                continue;
                            if (_StockRefArticle.FirstOrDefault(refstock => Marticle.refarticle.ToLower().Contains(refstock)) != null)
                            {

                                Marticle.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("stock")).id;
                                var colNum = Marticle.refarticle.Substring(Marticle.refarticle.Count() - 2);
                                int ColNumber = 0;
                                bool feasible = Int32.TryParse(colNum, out ColNumber);
                                if (feasible)
                                {
                                    var col = ListCouleur.FirstOrDefault(coul => coul.numero == ColNumber);
                                    if (col != null)
                                        Marticle.couleur = col.id;
                                }
                            }
                            else
                            {
                                Marticle.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("diver")).id;
                            }
                            Marticle.designation = row["designation"].ToString();
                            Marticle.nom = row["designation"].ToString();

                            if (Marticle.designation.ToLower().Contains("coton"))
                            {
                                var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("cot"));
                                if (compObj != null)
                                    Marticle.composition = compObj.id;

                            }
                            else if (Marticle.designation.ToLower().Contains("elastique"))
                            {
                                var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("gom"));
                                if (compObj != null)
                                    Marticle.composition = compObj.id;
                            }
                            else
                            {
                                var compObj = ListComposition.FirstOrDefault(comp => comp.nom.ToLower().Contains("pp") && comp.nom.ToLower().Contains("ps"));
                                if (compObj != null)
                                    Marticle.composition = compObj.id;
                            }
                            string resultString = Regex.Match(Marticle.designation, @"\d+").Value;
                            int larg = 0;
                            bool b = Int32.TryParse(resultString, out larg);
                            if (b)
                            {
                                Marticle.largeur = larg;

                            }
                            else if (Marticle.refarticle.ToLower().Contains("gb"))
                            {
                                Marticle.largeur = 25;
                            }

                            Marticle.client = row["code"].ToString();
                            if (Marticle.client.ToLower().Contains("sv"))
                            {

                                Marticle.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("stock")).id;
                            }
                            else if (Marticle.client.ToLower().Contains("ehc"))
                            {
                                Marticle.categorie = ListCategorie.FirstOrDefault(cat => cat.name.ToLower().Equals("ehc")).id;
                            }

                            Marticle.unite = row["unit"].ToString();
                            if(Marticle.unite.ToLower().Equals("pair"))
                            {
                                Marticle.unite = "Pr";
                            }

                            Marticle.condi =Convert.ToInt32(Convert.ToDouble(row["condi"].ToString()));


                           
                            
                            

                            Marticle.qtestock = Convert.ToInt32(row["stockq"].ToString());
                            Marticle.qtestockinit = Convert.ToInt32(row["stockinvq"].ToString());
                            Marticle.vente = Convert.ToInt32(row["ventq"].ToString());
                            Marticle.qteprod = +Marticle.qtestock + Marticle.vente - Marticle.qtestockinit;
                            _db.UpdateArticleProd(Marticle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError.Raise(ex.ToString());
            }
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
                        if (art.unite.ToLower().Equals("pair"))
                         {
                              art.unite = "Pr";
                         }

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
