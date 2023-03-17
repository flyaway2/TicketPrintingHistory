using BackEnd.viewmodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class user:INotifyPropertyChanged
    {
        public enum UserType
        {
            verificateur = 1,
            redacteur = 2,
            superuser = 3
        }
        private LoginViewModel _LoginViewM;

        public LoginViewModel LoginViewM
        {
            get
            {
                return _LoginViewM;
            }
            set
            {
                _LoginViewM = value;
                NotifyPropertyChanged();
            }
        }
        private int _ID;

        private string _password;
        private UserType _type;

        private string _username;

        public int ID
        {
            get => _ID;
            set
            {
                _ID = value;
                NotifyPropertyChanged();
            }
        }

        public string username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyPropertyChanged();
            }
        }

        public string password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChanged();
            }
        }

        public UserType type
        {
            get => _type;
            set
            {
                _type = value;
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
