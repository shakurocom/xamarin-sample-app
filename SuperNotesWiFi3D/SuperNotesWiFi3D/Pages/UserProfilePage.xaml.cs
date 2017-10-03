using System;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.Pages
{
    public partial class UserProfilePage : ContentPage
    {
        #region Initialization

        public UserProfilePage()
        {
            InitializeComponent();

            // navigation bar
            NavigationPage.SetHasNavigationBar(this, false);

            // status bar
            _statusBarBackground.HeightRequest = App.StyleHelper.StatusBarBackground_Height;
            _statusBarBackground.IsVisible = App.StyleHelper.StatusBarBackground_IsVisible;

            // top bar
            _topBarTitleLabel.Text = "Profile";

            // main
            var user = DataManager.SharedInstace.CurrentUser();
            _mainUserNameTextField.Text = user?.userData?.name;
        }

        #endregion


        #region Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var appState = DataManager.SharedInstace.CurrentApplicationState;
            appState.ProfileState.isShown = true;
			// restore state ?
            if (!appState.ProfileState.needToBeShown)
            {
				throw new Exception("Error. 'profile' page is shown, when 'ProfileState.needToBeShown' is FALSE.");
			}
        }

        #endregion


        #region Interface callbacks

        public void TopBarBackButtonClicked(object sender, EventArgs e)
		{
            Navigation.PopAsync(true);
		}

        public void MainLogoutButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
            var _ = DataManager.SharedInstace.SignOutAsync();
        }

        #endregion
    }
}
