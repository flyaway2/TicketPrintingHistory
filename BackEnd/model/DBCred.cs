using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class DBCred : INotifyPropertyChanged
    {
        private int _id;
        public int id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }
        private int _novhistory;

        public int novhistory
        {
            get { return _novhistory; }
            set { _novhistory = value; NotifyPropertyChanged(); }
        }


        private string _source;
        public string source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged();
            }
        }
        private string _catalog;
        public string catalog
        {
            get => _catalog;
            set
            {
                _catalog = value;
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
