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
    public class UploadProductionViewModel:MvxViewModel
    {
        public readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;

        public UploadProductionViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            getCred();
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
        public void CloseView(object sender, EventArgs e)
        {
            _navigationService.Close(this);
        }
        public void SetIndicatorValues(object o, ProgressChangedEventArgs args)
        {
            int prog = args.ProgressPercentage;
            IndicatorMsg = "Téléchargement Production " + DownloadedArticle + "/ " + TotalArticle;
            ProgressValue = Convert.ToInt32(((double)DownloadedArticle / TotalArticle) * 100);
        }
        public void StartBackGroundWorker(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            BW = new BackgroundWorker();
            BW.DoWork += ExtractProduction;
            BW.RunWorkerCompleted += CloseView;

            BW.WorkerReportsProgress = true;
            BW.ProgressChanged += SetIndicatorValues;
            BW.RunWorkerAsync();
        }
        private MvxObservableCollection<Article> _AllArticles;

        public MvxObservableCollection<Article> AllArticles
        {
            get { return _AllArticles; }
            set { _AllArticles = value; }
        }
        private string connectionString;
        private DBCred mDBCred;

        public void getCred()
        {
            var dbCred = _db.GetDBCred();
            if (dbCred.Count > 0)
            {
                try
                {
                    mDBCred = new DBCred();
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
        public void SetConnectionString()
        {
            connectionString = @"Provider=PCSOFT.HFSQL;Data Source=" + mDBCred.source + ";user id=admin;Initial Catalog=" + mDBCred.catalog;
        }
        public void ExtractProduction(object sender, EventArgs e)
        {

            BackgroundWorker worker = (BackgroundWorker)sender;
            try
            {
                AllArticles = new MvxObservableCollection<Article>(_db.GetArticles());

                string sql = @"SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_" + DateTime.Now.Year + "_Article where classe1=2"; //MyTable = The .FIC file
                int RowIndex = 0;
                DataTable table = new DataTable();
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
                            if (AllArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle) == null)
                                continue;
                            Marticle.refarticle = row["ref"].ToString();
                            Marticle.qtestock = Convert.ToInt32(row["stockq"].ToString());
                            Marticle.qtestockinit = Convert.ToInt32(row["stockinvq"].ToString());
                            Marticle.vente = Convert.ToInt32(row["ventq"].ToString());
                            Marticle.id = AllArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).id;
                            Marticle.qteprod = +Marticle.qtestock + Marticle.vente - Marticle.qtestockinit;
                            _db.UpdateArticleProd(Marticle);
                        }
                        _db.ArchiveHistory();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMsg.Raise(ex.ToString());
            }
        }
        }
}
