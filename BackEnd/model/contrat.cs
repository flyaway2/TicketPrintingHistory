using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackEnd.model
{
    public class contrat : INotifyPropertyChanged
    {



        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value;
                NotifyPropertyChanged();
            }
        }

        private string _nomcontrat;

        public string nomcontrat
        {
            get { return _nomcontrat; }
            set { _nomcontrat = value;
                NotifyPropertyChanged();
            }
        }

        private int _activer;

        public int activer
        {
            get { return _activer; }
            set { _activer = value;
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
