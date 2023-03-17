using BackEnd.Data;
using BackEnd.model;
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
using System.Windows.Threading;

namespace BackEnd.viewmodel
{
   public class UploadProdGapViewModel:MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;

        public UploadProdGapViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;

            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            getCred();
            GetProduction();

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
            BW.DoWork += ConnectToWmanagerDB;
            BW.RunWorkerCompleted += CloseView;

            BW.WorkerReportsProgress = true;
            BW.ProgressChanged += SetIndicatorValues;
            BW.RunWorkerAsync();
        }
        private MvxObservableCollection<Article> _ProdArticles;

        public MvxObservableCollection<Article> ProdArticles
        {
            get { return _ProdArticles; }
            set { _ProdArticles = value; }
        }
       
        public void GetProduction()
        {
            ProdArticles = new MvxObservableCollection<Article>(_db.GetEtatProduction());
        }
        public void ConnectToWmanagerDB(object sender, EventArgs e)
        {
            try
            {
                BackgroundWorker worker = (BackgroundWorker)sender;
                string sql = @"SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_" + DateTime.Now.Year + "_Article where classe1=2"; //MyTable = The .FIC file
                string sql2 = "SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_2023_Article"; //MyTable = The .FIC file
                int RowIndex = 0;
                DataTable table = new DataTable();
                _db.ViderEcartArticles();
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
                            if (ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle) == null)
                                continue;
                            Marticle.refarticle = row["ref"].ToString();
                            Marticle.qteprod = ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).qteprod;

                            Marticle.designation = row["designation"].ToString();
                            Marticle.stockWmanager = (double)row["stockq"];
                            Marticle.vente = Convert.ToInt32(row["ventq"].ToString());
                            Marticle.qtestock = ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).qtestockinit +
                                ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).prodinit +
                                (Marticle.qteprod * ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).condi) - Marticle.vente;
                            Marticle.colistock = (int)Math.Abs(Marticle.qtestock - Marticle.stockWmanager) / ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).condi;
                            if (Marticle.qtestock != Marticle.stockWmanager)
                            {
                                _db.AddEcart(Marticle);
                            }
                               
                        }
                        mDBCred.novhistory = 0;
                        _db.UpdateNovHistory(mDBCred);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMsg.Raise(ex.ToString());
            }

        }
        public void SetIndicatorValues(object o, ProgressChangedEventArgs args)
        {
            int prog = args.ProgressPercentage;
            IndicatorMsg = "Téléchargement Stock " + DownloadedArticle + "/ " + TotalArticle;
            ProgressValue = Convert.ToInt32(((double)DownloadedArticle / TotalArticle) * 100);
        }
        public void CloseView(object sender, EventArgs e)
        {
            _navigationService.Close(this);
        }
        private int _ProgressValue;

        public int ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
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
        private string _IndicatorMsg;

        public string IndicatorMsg
        {
            get { return _IndicatorMsg; }
            set
            {
                _IndicatorMsg = value;
                RaisePropertyChanged();
            }
        }
        public void getCred()
        {
            var dbCred = _db.GetDBCred();
            if (dbCred.Count > 0)
            {
                try
                {
                    mDBCred = new DBCred();
                    mDBCred.id = dbCred[0].id;
                    mDBCred.source = dbCred[0].source;
                    mDBCred.catalog = dbCred[0].catalog;
                    SetConnectionString();
                }
                catch (Exception ex)
                {
                    ShowMsg.Raise(ex.ToString());
                }

            }
        }
        public MvxInteraction<string> ShowMsg { get; } = new MvxInteraction<string>();

        private string connectionString;
        private DBCred mDBCred;
        public void SetConnectionString()
        {
            connectionString = @"Provider=PCSOFT.HFSQL;Data Source=" + mDBCred.source + ";user id=admin;Initial Catalog=" + mDBCred.catalog;
        }
    }
}
