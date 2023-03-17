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

namespace BackEnd.viewmodel
{
    public class HistoriqueViewModel:MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData _db;
        public HistoriqueViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();
            Historiques = new MvxObservableCollection<PrintHistory>();
            UpdateHistoriqueList();
            UpdatePrintCategorieList();
        }

        private DateTime? _SelectedStartDate;

        public DateTime? SelectedStartDate
        {
            get { return _SelectedStartDate; }
            set { _SelectedStartDate = value;
                RaisePropertyChanged();
            }
        }
        private DateTime? _SelectedEndDate;

        public DateTime? SelectedEndDate
        {
            get { return _SelectedEndDate; }
            set { _SelectedEndDate = value;
                RaisePropertyChanged();
            }
        }
        private IMvxCommand _FilterCmd;

        public IMvxCommand FilterCmd
        {
            get {
                _FilterCmd = new MvxCommand(SetFilter);
                return _FilterCmd; }
           
        }


        public void SetFilter()
        {
            if(SelectedPrintCatSearch!=null
                && SelectedStartDate != null && SelectedEndDate == null)
            {
                Historiques.ReplaceWith(FullHistoriques.Where(hist => Convert.ToDateTime(hist.date).Date >= SelectedStartDate.Value.Date
                && Convert.ToDateTime(hist.date).Date <= SelectedEndDate.Value.Date && hist.raisonimprObj.id == SelectedPrintCatSearch.id));
            }
            else if(SelectedPrintCatSearch != null)
            {
                Historiques.ReplaceWith(FullHistoriques.Where(hist => hist.raisonimprObj.id== SelectedPrintCatSearch.id));
            }
            else if (SelectedStartDate != null && SelectedEndDate == null)
            {
                Historiques.ReplaceWith(FullHistoriques.Where(hist => Convert.ToDateTime(hist.date).Date >= SelectedStartDate.Value.Date
                && Convert.ToDateTime(hist.date).Date <= SelectedEndDate.Value.Date));
            }
        }

        private MvxObservableCollection<PrintHistory> _Historiques;

        public MvxObservableCollection<PrintHistory> Historiques
        {
            get { return _Historiques; }
            set { _Historiques = value;
                RaisePropertyChanged();
            }
        }
        private MvxObservableCollection<PrintHistory> _FullHistoriques;

        public MvxObservableCollection<PrintHistory> FullHistoriques
        {
            get { return _FullHistoriques; }
            set
            {
                _FullHistoriques = value;
                RaisePropertyChanged();
            }
        }
        private PrintHistory _SelectedHistorique;

        public PrintHistory SelectedHistorique
        {
            get { return _SelectedHistorique; }
            set { _SelectedHistorique = value;
                RaisePropertyChanged();
                if (value != null)
                    SetHistoryProp();
            }
        }

        public void SetHistoryProp()
        {
            EditNum = SelectedHistorique.nbr.ToString();
            HistoryID = SelectedHistorique.id;
            if (SelectedHistorique.raisonimprObj!=null)
                SelectedPrintCategorie = PrintCategorie.FirstOrDefault(cat => cat.id ==SelectedHistorique.raisonimprObj.id);
        }

        public void UpdatePrintCategorieList()
        {
            PrintCategorie = new MvxObservableCollection<PrintCategorie>(_db.GetPrintCategories());
        }
        public void UpdateHistoriqueList()
        {
            FullHistoriques = new MvxObservableCollection<PrintHistory>(_db.GetHistoriqueDetails());
            Historiques.ReplaceWith(FullHistoriques);
        }

        private string _EditNum;

        public string EditNum
        {
            get { return _EditNum; }
            set { _EditNum = value;
                RaisePropertyChanged();
            }
        }

        private PrintCategorie _SelectedPrintCatSearch;

        public PrintCategorie SelectedPrintCatSearch
        {
            get { return _SelectedPrintCatSearch; }
            set { _SelectedPrintCatSearch = value;
                RaisePropertyChanged();
            }
        }


        private MvxObservableCollection<PrintCategorie> _PrintCategorie;

        public MvxObservableCollection<PrintCategorie> PrintCategorie
        {
            get { return _PrintCategorie; }
            set { _PrintCategorie = value;
                RaisePropertyChanged();
            }
        }

        private PrintCategorie _SelectedPrintCategorie;

        public PrintCategorie SelectedPrintCategorie
        {
            get { return _SelectedPrintCategorie; }
            set { _SelectedPrintCategorie = value;
                RaisePropertyChanged();
            }
        }

        private int HistoryID;


        private IMvxCommand _EditHistCmd;

        public IMvxCommand EditHistCmd
        {
            get {
                _EditHistCmd = new MvxCommand(EditHistory);
                return _EditHistCmd; }
        }

        public void EditHistory()
        {
            var Printhist = new PrintHistory();
            Printhist.id = HistoryID;
            var NumColis = 0;
            if(Int32.TryParse(EditNum,out NumColis))
            {
                Printhist.nbr = NumColis;
                Printhist.raisonimpr = SelectedPrintCategorie.id;
                _db.UpdateHistory(Printhist);
                var dbCred = _db.GetDBCred();
                if (dbCred.Count > 0)
                {
                    var mdbCred = dbCred[0];
                    mdbCred.novhistory = 1;
                    _db.UpdateNovHistory(mdbCred);
                }
                UpdateHistoriqueList();
                SelectedPrintCategorie = null;
                EditNum = "";
            }
            
        }


    }
}
