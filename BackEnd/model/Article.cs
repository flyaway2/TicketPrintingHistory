using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class Article : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private int _idcontrat;

        public int idcontrat
        {
            get { return _idcontrat; }
            set { _idcontrat = value;
                NotifyPropertyChanged();
            }
        }


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

        private int _idarticle;
        public int idarticle
        {
            get => _idarticle;
            set
            {
                _idarticle = value;
                NotifyPropertyChanged();
            }
        }
        private string _refarticle;
        public string refarticle
        {
            get => _refarticle;
            set
            {
                _refarticle = value;
                NotifyPropertyChanged();
            }
        }
        private string _client;
        public string client
        {
            get => _client;
            set
            {
                _client = value;
                NotifyPropertyChanged();
            }
        }
        private string _designation;
        public string designation
        {
            get => _designation;
            set
            {
                _designation = value;
                NotifyPropertyChanged();
            }
        }
        private string _unite;
        public string unite
        {
            get => _unite;
            set
            {
                _unite = value;
                NotifyPropertyChanged();
            }
        }
        private string _name;
        public string nom
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        private int _qteprod;
        public int qteprod
        {
            get => _qteprod;
            set
            {
                _qteprod = value;
                NotifyPropertyChanged();
            }
        }
        private int _prodinit;
        public int prodinit
        {
            get => _prodinit;
            set
            {
                _prodinit = value;
                NotifyPropertyChanged();
            }
        }
        private int _colisprod;
        public int colisprod
        {
            get => _colisprod;
            set
            {
                _colisprod = value;
                NotifyPropertyChanged();
            }
        }
        private int _qtestock;
        public int qtestock
        {
            get => _qtestock;
            set
            {
                _qtestock = value;
                NotifyPropertyChanged();
            }
        }
        private int _colistock;
        public int colistock
        {
            get => _colistock;
            set
            {
                _colistock = value;
                NotifyPropertyChanged();
            }
        }
        private int _qtestockinit;
        public int qtestockinit
        {
            get => _qtestockinit;
            set
            {
                _qtestockinit = value;
                NotifyPropertyChanged();
            }
        }
        private int _colisstockinit;
        public int colisstockinit
        {
            get => _colisstockinit;
            set
            {
                _colisstockinit = value;
                NotifyPropertyChanged();
            }
        }
        private int _vente;
        public int vente
        {
            get => _vente;
            set
            {
                _vente = value;
                NotifyPropertyChanged();
            }
        }
        private float _condi;
        public float condi
        {
            get => _condi;
            set
            {
                _condi = value;
                NotifyPropertyChanged();
            }
        }
        private int _categorie;
        public int categorie
        {
            get => _categorie;
            set
            {
                _categorie = value;
                NotifyPropertyChanged();
            }
        }

        private Categorie _categorieObj;
        public Categorie categorieObj
        {
            get => _categorieObj;
            set
            {
                _categorieObj = value;
                NotifyPropertyChanged();
            }
        }
        private int _couleur;
        public int couleur
        {
            get => _couleur;
            set
            {
                _couleur = value;
                NotifyPropertyChanged();
            }
        }
        private Couleur _couleurObj;
        public Couleur couleurObj
        {
            get => _couleurObj;
            set
            {
                _couleurObj = value;
                NotifyPropertyChanged();
            }
        }
        private int _largeur;
        public int largeur
        {
            get => _largeur;
            set
            {
                _largeur = value;
                NotifyPropertyChanged();
            }
        }
        private int _composition;
        public int composition
        {
            get => _composition;
            set
            {
                _composition = value;
                NotifyPropertyChanged();
            }
        }
        private composition _compositionObj;
        public composition compositionObj
        {
            get => _compositionObj;
            set
            {
                _compositionObj = value;
                NotifyPropertyChanged();
            }
        }
        private int _contrat;
        public int contrat
        {
            get => _contrat;
            set
            {
                _contrat = value;
                NotifyPropertyChanged();
            }
        }
        private int _prevision;
        public int prevision
        {
            get => _prevision;
            set
            {
                _prevision = value;
                NotifyPropertyChanged();
            }
        }

        private double _stockWmanager;

        public double stockWmanager
        {
            get { return _stockWmanager; }
            set { _stockWmanager = value;
                NotifyPropertyChanged();
            }
        }







    }
}
