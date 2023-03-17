using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class contratarticle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _id;

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        private int _idcontrat;

        public int idcontrat
        {
            get { return _idcontrat; }
            set
            {
                _idcontrat = value;
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
    }
}
