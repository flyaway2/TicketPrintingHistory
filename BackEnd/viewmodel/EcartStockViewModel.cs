using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace BackEnd.viewmodel
{
    public class EcartStockViewModel:MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public EcartStockViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;

            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            
           
            
           
        }
        private DispatcherTimer dispatcherTimer;
        public override void ViewAppeared()
        {
            base.ViewAppeared();
             dbCred = _db.GetDBCred();
            if (dbCred.Count > 0)
            {
                mDBCred = new DBCred();
                mDBCred.source = dbCred[0].source;
                mDBCred.catalog = dbCred[0].catalog;
                mDBCred.novhistory = dbCred[0].novhistory;
                SetConnectionString();
          
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += PopulateList;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();
               

                



            }

        }
        private List<DBCred> dbCred;
        public void PopulateList(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
           
            
                try
                {
                   
                    _navigationService.Navigate<UploadProdGapViewModel>();
                GetEcartArticles();

            }
                catch (Exception ex)
                {
                    ShowMsg.Raise(ex.ToString());
                }

        }


        private MvxObservableCollection<Article> _Articles=new MvxObservableCollection<Article>();

        public MvxObservableCollection<Article> Articles
        {
            get { return _Articles;
                
            }
            set { _Articles = value; RaisePropertyChanged(); }
        }
        private DBCred mDBCred;
        private MvxObservableCollection<Article> _ProdArticles;

        public MvxObservableCollection<Article> ProdArticles
        {
            get { return _ProdArticles; }
            set { _ProdArticles = value; }
        }
        private MvxObservableCollection<Article> _AllArticles;

        public MvxObservableCollection<Article> AllArticles
        {
            get { return _AllArticles; }
            set { _AllArticles = value; }
        }
        public void GetEcartArticles()
        {
            Articles = new MvxObservableCollection<Article>(_db.GetEcartArticles());
        }
        public void GetProduction()
        {
            ProdArticles = new MvxObservableCollection<Article>(_db.GetEtatProduction());
        }

        private IMvxCommand _ResetCalculeCmd;

        public IMvxCommand ResetCalculeCmd
        {
            get {
                _ResetCalculeCmd = new MvxCommand(UploadProduction);
                return _ResetCalculeCmd; }
        }
        private string connectionString;

        public void SetConnectionString()
        {
            connectionString = @"Provider=PCSOFT.HFSQL;Data Source=" + mDBCred.source + ";user id=admin;Initial Catalog="+ mDBCred.catalog;
        }

        public void UploadProduction()
        {
            _navigationService.Navigate<UploadProductionViewModel>();
        }

        public void ResetCalcule()
        {
           try
            {
                AllArticles = new MvxObservableCollection<Article>(_db.GetArticles());

                string sql = @"SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_" + DateTime.Now.Year +"_Article where classe1=2"; //MyTable = The .FIC file

                DataTable table = new DataTable();
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
                    {
                        adapter.Fill(table); //Fill the table with the extracted data
                        foreach (DataRow row in table.Rows)
                        {
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
                    }
                }
            }
            catch(Exception ex)
            {
                ShowMsg.Raise(ex.ToString());
            }
        }
        public MvxInteraction<string> ShowMsg { get; } = new MvxInteraction<string>();

        public void ConnectToWmanagerDB()
        {
            try
            {
                string sql = @"SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_" + DateTime.Now.Year+"_Article where classe1=2"; //MyTable = The .FIC file
                string sql2 = "SELECT IDArticle,ref,designation,stockq,ventq,stockinvq FROM BL_2023_Article"; //MyTable = The .FIC file

                DataTable table = new DataTable(); 
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
                    {
                        adapter.Fill(table); //Fill the table with the extracted data
                        foreach (DataRow row in table.Rows)
                        {
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
                            Marticle.colistock = (int)Math.Abs(Marticle.qtestock- Marticle.stockWmanager) / ProdArticles.FirstOrDefault(prArt => prArt.idarticle == Marticle.idarticle).condi;
                            if (Marticle.qtestock != Marticle.stockWmanager)
                                Articles.Add(Marticle);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ShowMsg.Raise(ex.ToString());
            }
           
        }
    }
}
