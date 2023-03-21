using BackEnd.Data;
using BackEnd.model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.viewmodel
{
    public class HomepageViewModel : MvxViewModel<user>
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly SqliteData db;
        private user UserSession;
        private IMvxCommand _ActivateSuperUser;

        public IMvxCommand ActivateSuperUser
        {
            get {
                _ActivateSuperUser = new MvxCommand(ActivatingsuperUser);
                return _ActivateSuperUser; }
        }
        private int CountClick = 0;
        public void ActivatingsuperUser()
        {
            CountClick++;
            if(CountClick==3)
            {
                UserImg = @"..\Asset\administrator.png";
            }
            if(CountClick>3)
            {
                CountClick = 0;
                UserImg = @"..\Asset\user.png";
            }
        }
        private string _UserImg= @"..\Asset\user.png";

        public string UserImg
        {
            get { return _UserImg; }
            set { _UserImg = value;
                RaisePropertyChanged();
            }
        }


        public HomepageViewModel(IMvxNavigationService navser)
        {
            _navigationService = navser;
        }
        public override void Prepare(user parameter)
        {

            UserSession = parameter;
           
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            if (UserSession.LoginViewM != null)
                UserSession.LoginViewM.CloseWindow();
            StartPrintView();
        }

        public void StartPrintView()
        {
            _navigationService.Navigate<ImpressionViewModel>();
        }
        private IMvxCommand _PrintBtn;

        public IMvxCommand PrintBtn
        {
            get
            {
                _PrintBtn =new MvxCommand(NavigateToPrintView);
                return _PrintBtn;
            }
        }

        private IMvxCommand _CmdHistorique;

        public IMvxCommand CmdHistorique
        {
            get {
                _CmdHistorique = new MvxCommand(NavigateToHistoriqueView);
                return _CmdHistorique; }
        }

        private IMvxCommand _ProdCmd;

        public IMvxCommand ProdCmd
        {
            get {
                _ProdCmd = new MvxCommand(NavigateToProductionView);
                return _ProdCmd; }

            }

        private IMvxCommand _GapCmd;

        public IMvxCommand GapCmd
        {
            get {
                _GapCmd = new MvxCommand(NavigateToEcartStock);
                return _GapCmd; }
        }

        public void NavigateToEcartStock()
        {
            if(CountClick==3)
                _navigationService.Navigate<EcartStockViewModel>();
        }

        public void NavigateToProductionView()
        {
            _navigationService.Navigate<EtatStockViewModel>();
        }
    


        public void NavigateToHistoriqueView()
        {
            _navigationService.Navigate<HistoriqueViewModel>();
        }
        private IMvxCommand _ArticleBtn;

        public IMvxCommand ArticleBtn
        {
            get
            {
                _ArticleBtn = new MvxCommand(NavigateToArticleView);
                return _ArticleBtn;
            }
        }
        public void NavigateToArticleView()
        {
            if (CountClick == 3)
            {
                _navigationService.Navigate<ArticleViewModel, string>("superuser");
            }
            else
            {
                _navigationService.Navigate<ArticleViewModel, string>("");
            }
            
        }
        public void NavigateToPrintView()
        {
            _navigationService.Navigate<ImpressionViewModel>();
        }
    }
}
