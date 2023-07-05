using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class companyinfo : INotifyPropertyChanged
    {
        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value;
                NotifyPropertyChanged();
            }
        }

        private string _nom;

        public string nom
        {
            get { return _nom; }
            set { _nom = value; NotifyPropertyChanged(); }
        }

        private string _facebook;

        public string facebook
        {
            get { return _facebook; }
            set { _facebook = value; NotifyPropertyChanged(); }
        }
        private string _homephone;

        public string homephone
        {
            get { return _homephone; }
            set { _homephone = value; NotifyPropertyChanged(); }
        }

        private string _whatsapp;

        public string whatsapp
        {
            get { return _whatsapp; }
            set { _whatsapp = value; NotifyPropertyChanged(); }
        }

        private string _email;

        public string email
        {
            get { return _email; }
            set { _email = value; NotifyPropertyChanged(); }
        }

        private string _logo;

        public string logo
        {
            get { return _logo; }
            set { _logo = value; NotifyPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
