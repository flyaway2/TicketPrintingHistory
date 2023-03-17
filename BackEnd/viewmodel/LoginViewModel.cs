using BackEnd.CustomClass;
using BackEnd.Data;
using BackEnd.model;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Windows.Controls;

namespace BackEnd.viewmodel
{
    public class LoginViewModel:MvxViewModel<MvxViewModel>
    {
        private readonly SqliteData _db;

        private readonly IMvxNavigationService _navigationService;
        private IMvxCommand _RedacteurCmd;

        private IMvxCommand _VerificateurCmd;

        private user UserSession;

        public LoginViewModel(IMvxNavigationService _navSer)
        {
            _navigationService = _navSer;
            _db = Mvx.IoCProvider.Resolve<SqliteData>();

        }


        public override void ViewAppeared()
        {
            base.ViewAppeared();
            //_navigationService.DidNavigate+=CloseWindow;
        }

        public MvxInteraction<YesNoQuestion> ConfirmAction { get; } = new MvxInteraction<YesNoQuestion>();

        public MvxInteraction<string> SendNotification { get; } = new MvxInteraction<string>();

        public string UsernameRed { get; set; }

        public string UsernameVer { get; set; }

        

        public IMvxCommand RedacteurCmd
        {
            get
            {
                _RedacteurCmd = new MvxCommand<object>(RedacteurLogin);
                return _RedacteurCmd;
            }
        }

        public void RedacteurLogin(object obj)
        {
            var passW = obj as PasswordBox;

            if (UsernameRed != null && passW.Password != null && !string.IsNullOrWhiteSpace(UsernameRed) &&
                !string.IsNullOrWhiteSpace(passW.Password))
            {
                var CorrectAuth = false;
                var userlist = _db.GetUsers();
                foreach (var user in userlist)
                        if (user.username.Equals(UsernameRed) && user.password.Equals(passW.Password))
                        {
                            CorrectAuth = true;
                            UserSession = user;
                        }

                if (CorrectAuth)
                {
                    UserSession.LoginViewM= this;
                    _navigationService.Navigate<HomepageViewModel, user>(UserSession);
                }
                else
                {
                    SendNotification.Raise("Utilisateur ou mode passe incorrect");
                }
            }
            else
            {
                SendNotification.Raise("Champ vide");
            }
        }
        public async void CloseWindow()
        {
            var b = await _navigationService.Close(this);
            if (SplashScreen != null)
               b = await _navigationService.Close(SplashScreen);
        }

        public override void Prepare(MvxViewModel parameter)
        {
            SplashScreen = parameter;
        }

        private MvxViewModel SplashScreen;
    }

}
