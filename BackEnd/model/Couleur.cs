using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class Couleur : INotifyPropertyChanged
    {

        public Couleur()
        {

        }
        public Couleur(int num,string _nom)
        {
            nom = _nom;
            numero = num;
        }
        private int _ID;


        private string _Name;


        private int _Nbr;

        public int id
        {
            get => _ID;
            set
            {
                _ID = value;
                NotifyPropertyChanged();
            }
        }

        public int numero
        {
            get => _Nbr;
            set
            {
                _Nbr = value;
                NotifyPropertyChanged();
            }
        }

        public string nom
        {
            get => _Name;
            set
            {
                _Name = value;
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
