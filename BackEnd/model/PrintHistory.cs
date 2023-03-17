using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class PrintHistory : INotifyPropertyChanged
    {


        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value;
                NotifyPropertyChanged();
            }
        }
        private string _date;

        public string date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged();
            }
        }
        private string _heure;

        public string heure
        {
            get { return _heure; }
            set
            {
                _heure = value;
                NotifyPropertyChanged();
                if(value!=null)
                    heureSpan= TimeSpan.Parse(heure);
            }
        }
        private TimeSpan _heureSpan;

        public TimeSpan heureSpan
        {
            get { return _heureSpan; }
            set
            {
                _heureSpan = value;
                NotifyPropertyChanged();
            }
        }
        private int _article;

        public int article
        {
            get { return _article; }
            set
            {
                _article = value;
                NotifyPropertyChanged();
            }
        }
        private Article _articleObj;

        public Article articleObj
        {
            get { return _articleObj; }
            set
            {
                _articleObj = value;
                NotifyPropertyChanged();
            }
        }
        private int _raisonimpr;

        public int raisonimpr
        {
            get { return _raisonimpr; }
            set
            {
                _raisonimpr = value;
                NotifyPropertyChanged();
            }
        }
        private PrintCategorie _raisonimprObj;

        public PrintCategorie raisonimprObj
        {
            get { return _raisonimprObj; }
            set
            {
                _raisonimprObj = value;
                NotifyPropertyChanged();
            }
        }
        private int _nbr;

        public int nbr
        {
            get { return _nbr; }
            set
            {
                _nbr = value;
                NotifyPropertyChanged();
            }
        }

        private int _condi;

        public int condi
        {
            get { return _condi; }
            set { _condi = value;
                NotifyPropertyChanged();
            }
        }
        private int _archive;

        public int archive
        {
            get { return _archive; }
            set { _archive = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
