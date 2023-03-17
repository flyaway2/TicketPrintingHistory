using BackEnd.Data;
using MvvmCross;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd
{

    public class App:MvxApplication 
    {
       
        public override void Initialize()
        {
            base.Initialize();
            Mvx.IoCProvider.RegisterSingleton(new SqliteData());
            //RegisterAppStart<HomepageViewModel>();
            RegisterCustomAppStart<AppStart>();
        }
    }
}
