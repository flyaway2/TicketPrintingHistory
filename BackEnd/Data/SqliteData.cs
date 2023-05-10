using BackEnd.Database;
using BackEnd.model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;

namespace BackEnd.Data
{
    public class SqliteData
    {
        private readonly string PrincipalFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string connectionStringName;
        private readonly SqliteDataAccess db;
        public SqliteData()
        {
            connectionStringName = "Data Source=TicketProd.db";
            db = new SqliteDataAccess();
        }
        public List<user> GetUsers()
        {
            return db.LoadData<user, dynamic>("select * from user ", null,
                connectionStringName);
        }
        public List<contrat> GetContrats()
        {
            return db.LoadData<contrat, dynamic>("select * from contrat", null,
                connectionStringName);
        }
        public List<contrat> GetActiveContrats()
        {
            return db.LoadData<contrat, dynamic>("select * from contrat where activer=1", null,
                connectionStringName);
        }
        public void AddNewContrat(contrat mcontrat)
        {
            var stm = "Insert into contrat(nomcontrat,activer) values(@nomcontrat,1)";
            db.SaveData<dynamic>(stm, new { mcontrat.nomcontrat }, connectionStringName);
        }
        public void UpdateContratState(contrat mcontrat)
        {
            var stm = "update contrat set activer=@activer where id=@id";
            db.SaveData<dynamic>(stm, new { mcontrat.activer, mcontrat.id }, connectionStringName);
        }
        public void UpdateContrat(contrat mcontrat)
        {
            var stm = "update contrat set nomcontrat=@nomcontrat where id=@id";
            db.SaveData<dynamic>(stm, new { mcontrat.nomcontrat, mcontrat.id }, connectionStringName);
        }
        public void DeleteContrat(contrat mcontrat)
        {
            var stm = "delete from contrat where id=@id";
            db.SaveData<dynamic>(stm, new { mcontrat.id }, connectionStringName);
        }
        public void AddNewArticleContrat(contratarticle mcontrat)
        {
            var stm = "Insert into contratarticle(idcontrat,article) values(@idcontrat,@article)";
            db.SaveData<dynamic>(stm, new {mcontrat.idcontrat, mcontrat.article }, connectionStringName);
        }
        public void DeleteArticleContrat(contratarticle mcontrat)
        {
            var stm = "delete from contratarticle where idcontrat=@idcontrat and article=@article";
            db.SaveData<dynamic>(stm, new { mcontrat.idcontrat, mcontrat.article }, connectionStringName);
        }
        public void AddNewCouleur(Couleur col)
        {
            var stm = "Insert into couleur(numero,nom) values(@num,@des)";
            db.SaveData<dynamic>(stm, new { des = col.nom,num=col.numero }, connectionStringName);
        }
        public void AddEcart(Article article)
        {
            var stm = "Insert into ecart(idarticle,refarticle,designation,stockwmanager,qtestock,colistock) " +
                "values(@idarticle,@refarticle,@designation,@stockWmanager,@qtestock,@colistock)";
            db.SaveData<dynamic>(stm, new { article.idarticle,article.refarticle,article.designation
                , article.stockWmanager,article.qtestock,article.colistock }, connectionStringName);
        }
        public void UpdateDefaultImprimant(imprimant NovImprimant)
        {
            var stm = "update printer set nom=@des where id=@id";
            db.SaveData<dynamic>(stm, new { des = NovImprimant.nom, NovImprimant.id }, connectionStringName);
        }
        public void AddDefaultImprimant(imprimant NovImprimant)
        {
            var stm = "Insert into printer(nom) values(@des)";
            db.SaveData<dynamic>(stm, new { des = NovImprimant.nom }, connectionStringName);
        }
        public List<imprimant> GetImprimant()
        {
            return db.LoadData<imprimant, dynamic>("Select * from printer", null, connectionStringName);
        }

        public void UpdateDefaultDBCred(DBCred NovdbRed)
        {
            var stm = "update dbcred set source=@source,catalog=@catalog where id=@id";
            db.SaveData<dynamic>(stm, new { NovdbRed.catalog, NovdbRed.source, NovdbRed.id }, connectionStringName);
        }
        public void UpdateNovHistory(DBCred NovdbRed)
        {
            var stm = "update dbcred set novhistory=@hist where id=@id";
            db.SaveData<dynamic>(stm, new { hist=  NovdbRed.novhistory, NovdbRed.id }, connectionStringName);
        }
        public void AddDefaultDBCred(DBCred NovdbRed)
        {
            var stm = "Insert into dbcred(source,catalog) values(@source,@catalog)";
            db.SaveData<dynamic>(stm, new { NovdbRed.source , NovdbRed.catalog}, connectionStringName);
        }
        public List<DBCred> GetDBCred()
        {
            return db.LoadData<DBCred, dynamic>("Select * from dbcred", null, connectionStringName);
        }

        public void AddNewComposition(composition NovComp)
        {
            var stm = "Insert into composition(nom) values(@Designation)";
            db.SaveData<dynamic>(stm, new { Designation = NovComp.nom }, connectionStringName);
        }
        public void AddNewCategorie(Categorie NovCat)
        {
            var stm = "Insert into categoriearticle(name) values(@Designation)";
            db.SaveData<dynamic>(stm, new { Designation = NovCat.name }, connectionStringName);
        }
        public void AddNewPrintCategorie(PrintCategorie NovCat)
        {
            var stm = "Insert into raisonimpr(name) values(@Designation)";
            db.SaveData<dynamic>(stm, new { Designation = NovCat.name }, connectionStringName);
        }
        public void AddNewPrintHistory(PrintHistory histPrint)
        {
            var stm = "Insert into histimpr(date,heure,article,raisonimpr,nbr,condi,archive) values(@dat,@hr,@art,@raison,@nbr,@condi,0)";
            db.SaveData<dynamic>(stm, new { dat = histPrint.date,
                hr=histPrint.heure,art=histPrint.article,raison= histPrint.raisonimpr,nbr= histPrint.nbr,histPrint.condi
            }, connectionStringName);
        }
        public void UpdateHistory(PrintHistory histPrint)
        {
            db.SaveData<dynamic>("update histimpr set raisonimpr=@cat,nbr=@nbr where id=@id",
               new { histPrint.id, histPrint.nbr, cat= histPrint.raisonimpr }, connectionStringName);
        }
        public void UpdateHistoryArticle(PrintHistory histPrint)
        {
            db.SaveData<dynamic>("update histimpr set raisonimpr=@cat,nbr=@nbr,article=@article where id=@id",
               new { histPrint.id, histPrint.nbr,
                   cat = histPrint.raisonimpr,
                   article=histPrint.article

               }, connectionStringName);
        }
        public void ArchiveHistory()
        {
            db.SaveData<dynamic>("update histimpr set archive=1",
               new {  }, connectionStringName);
        }
        public void AddNewArticleFromExcel(Article NovArticle)
        {
            var stm = "Insert into article(idarticle,refarticle,client,designation,nom" +
                ",qtestock,qtestockinit,vente,categorie,largeur" +
                ",unite,condi,composition,couleur,qteprod) values(@id,@refarticle,@cl,@des,@name,@qtestock,@qtestockinit,@vente,@cat,@larg,@unite,@cond,@comp,@col,@qteprod)";
            db.SaveData<dynamic>(stm, new { id = NovArticle.idarticle,
                refarticle=NovArticle.refarticle,cl=NovArticle.client
            ,des=NovArticle.designation,
                name=NovArticle.nom,
                qtestock=NovArticle.qtestock,NovArticle.qtestockinit
            ,NovArticle.vente,cat=NovArticle.categorie,larg=NovArticle.largeur,NovArticle.unite,cond=NovArticle.condi,
            col=NovArticle.couleur,comp=NovArticle.composition,NovArticle.qteprod}, connectionStringName);
        }
        public void UpdateArticleProd(Article art)
        {
            db.SaveData<dynamic>("update article set qteprod=@prod where id=@id",
               new { prod = art.qteprod,  art.id }, connectionStringName);
        }
        public void UpdateArticlePrintProperties(Article art)
        {
             db.SaveData<dynamic>("update article set composition=@comp,largeur=@larg,couleur=@coul,categorie=@cat,nom=@nom,condi=@condi where id=@id",
                new { comp=art.compositionObj.id,larg=art.largeur,coul=art.couleurObj.id,cat=art.categorieObj.id,art.id,art.nom ,art.condi}, connectionStringName);
        }
        public List<Article> GetArticleByIDArticle(int  idArticle)
        {
            return db.LoadData<Article, dynamic>("Select * from article where idarticle=@idArticle", new { idArticle }, connectionStringName);
        }
        public List<PrintHistory> GetHistoriques()
        {
            return db.LoadData<PrintHistory, dynamic>("Select * from histimpr", null, connectionStringName);
        }
        public List<Article> GetEtatProduction()
        {
            return db.LoadData<Article, dynamic>("Select article.id,article.idarticle,histimpr.condi,article.refarticle,article.qtestock,article.qtestockinit,designation,qteprod as prodinit,sum(nbr) as qteprod from histimpr,article where article.id=histimpr.article and histimpr.archive=0 and histimpr.raisonimpr=1 group by article.id;", null, connectionStringName);
        }

        public List<PrintHistory> GetHistoriqueArticle(int idArticle)
        {
            return db.LoadData<PrintHistory, dynamic>("Select * from histimpr where article=@id",new { id= idArticle }, connectionStringName);
        }
        public List<PrintHistory> GetHistoriqueArticleJour(int idArticle)
        {
            return db.LoadData<PrintHistory, dynamic>("Select * from histimpr where article=@id and date=@dat", new { id = idArticle,dat= DateTime.Now.ToShortDateString() }, connectionStringName);
        }
        public List<Categorie> GetCategorie(string desingation)
        {
            return db.LoadData<Categorie, dynamic>("Select * from categoriearticle where name=@des", new { des=desingation}, connectionStringName);
        }
        public List<composition> GetComposition(string desingation)
        {
            return db.LoadData<composition, dynamic>("Select * from composition where nom=@des", new { des = desingation }, connectionStringName);
        }

        public List<Couleur> GetCouleur(Couleur col)
        {
            return db.LoadData<Couleur, dynamic>("Select * from couleur where numero=@num or nom=@des", new { des = col.nom,num=col.numero }, connectionStringName);
        }
        public List<PrintCategorie> GetPrintCategorie(PrintCategorie printCat)
        {
            return db.LoadData<PrintCategorie, dynamic>("Select * from raisonimpr where name=@nom ", new {nom=printCat.name}, connectionStringName);
        }
        public List<PrintCategorie> GetPrintCategories()
        {
            return db.LoadData<PrintCategorie, dynamic>("Select * from raisonimpr", null, connectionStringName);
        }
        public List<Couleur> GetCouleurs()
        {
            return db.LoadData<Couleur, dynamic>("Select * from couleur", null, connectionStringName);
        }
        public List<composition> GetCompositions()
        {
            return db.LoadData<composition, dynamic>("Select * from composition", null, connectionStringName);
        }
        public List<Categorie> GetCategorie()
        {
            return db.LoadData<Categorie, dynamic>("Select * from categoriearticle", null, connectionStringName);
        }
        public List<Article> GetArticles()
        {
            return db.LoadData<Article, dynamic>("Select * from article", null, connectionStringName);
        }
        public List<Article> GetArticlesByCategorie(Categorie cat)
        {
            return db.LoadData<Article, dynamic>("Select * from article where categorie=@id", new { cat.id}, connectionStringName);
        }
        public List<contratarticle> GetContratArticles(contrat Mcontrat)
        {
            return db.LoadData<contratarticle, dynamic>("Select * from contratarticle where idcontrat=@id", new { Mcontrat.id }, connectionStringName);
        }
        public List<Article> GetEcartArticles()
        {
            return db.LoadData<Article, dynamic>("Select * from ecart", null, connectionStringName);
        }
        public void ViderEcartArticles()
        {
            db.SaveData<dynamic>("delete from ecart ",
               null, connectionStringName);
        }
        public List<PrintHistory> GetHistoriqueDetails()
        {
            using (var conn = new SQLiteConnection(connectionStringName))
            {
                string stm = "Select *  from (Select *  from histimpr  as hist where hist.archive=0) as hist " +
                    "left join article as art on art.id=hist.article " +
                    "left join raisonimpr as raison on raison.id=hist.raisonimpr ";

                var result2 = conn.Query<PrintHistory, Article, PrintCategorie, PrintHistory>(stm, (printhist, art, printcat) =>
                {

                    printhist.articleObj = art;
                    printhist.raisonimprObj = printcat;
                    return printhist;
                }).AsList<PrintHistory>();
                return result2;
            }
        }
        public List<Article> GetArticlesDetails()
        {
            using (var conn = new SQLiteConnection(connectionStringName))
            {
                string stm = "Select * from article as art left join categoriearticle as cat on art.categorie=cat.id " +
                    "left join couleur as col on col.id=art.couleur " +
                    "left join composition as comp on comp.id=art.composition";
                //db.LoadData<Article, dynamic>("Select * from article as art,categorie as cat where art.categorie=cat.id", null, connectionStringName);
                var result2 = conn.Query<Article, Categorie,Couleur,composition, Article>(stm, (art, cat,col, comp) =>
                {

                    art.categorieObj = cat;
                    art.couleurObj = col;
                    art.compositionObj = comp;
                    return art;
                }).AsList<Article>();
                return result2;
            }
        }
    }
}
