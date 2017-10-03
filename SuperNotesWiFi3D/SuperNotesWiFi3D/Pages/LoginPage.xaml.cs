using System;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.Pages
{
    public partial class LoginPage : ContentPage
    {
        public Action loggedInHandler = null;

        public LoginPage()
        {
            InitializeComponent();
        }


        #region Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DataManager.SharedInstace.CurrentApplicationState.LoginState.isShown = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            DataManager.SharedInstace.CurrentApplicationState.LoginState.isShown = false;
        }

        #endregion


        #region Interface Callbacks

        internal void OnSignInButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_emailEntry.Text))
            {
                DisplayAlert("Error", "Please enter 'email'", "OK");
            }
            else if (string.IsNullOrEmpty(_passwordEntry.Text))
            {
                DisplayAlert("Error", "Please enter 'password'", "OK");
            }
            else 
            {
				_signInSpinner.IsRunning = true;
				_signInButton.IsVisible = false;
				_signUpButton.IsVisible = false;
                _facebookButton.IsVisible = false;
				_emailEntry.IsEnabled = false;
				_passwordEntry.IsEnabled = false;

				DataManager.SharedInstace.SignInWithEmailAndPassword(_emailEntry.Text, _passwordEntry.Text, delegate (Error error) {
					Device.BeginInvokeOnMainThread(delegate
					{
						if (error == null)
						{
							_signInSpinner.IsRunning = false;

							((NavigationPage)Application.Current.MainPage).Navigation.PopModalAsync(true);

                            loggedInHandler();
						}
						else
						{
							_signInSpinner.IsRunning = false;
							_signInButton.IsVisible = true;
							_signUpButton.IsVisible = true;
                            _facebookButton.IsVisible = true;
							_emailEntry.IsEnabled = true;
							_passwordEntry.IsEnabled = true;

							DisplayAlert("Error", error.Message, "OK");
						}
					});
				});
            }
        }

        internal void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            SignUpPage signUpPage = new SignUpPage();
            signUpPage.loggedInHandler = loggedInHandler;
            ((NavigationPage)Application.Current.MainPage).Navigation.PushModalAsync(signUpPage, true);
        }

        internal void FacebookButtonClicked(object sender, EventArgs e)
        {
            // lock UI
			_signInSpinner.IsRunning = true;
			_signInButton.IsVisible = false;
			_signUpButton.IsVisible = false;
			_facebookButton.IsVisible = false;
			_emailEntry.IsEnabled = false;
			_passwordEntry.IsEnabled = false;

            DataManager.SharedInstace.SignInWithFacebook(delegate (bool isCancelled, Error error)
            {
                if (isCancelled || error != null)
                {
					_signInSpinner.IsRunning = false;
					_signInButton.IsVisible = true;
					_signUpButton.IsVisible = true;
					_facebookButton.IsVisible = true;
					_emailEntry.IsEnabled = true;
					_passwordEntry.IsEnabled = true;

                    if (error != null)
                    {
                        DisplayAlert("Error", error.Message, "OK");
                    }
                }
                else
                {
					// success
					((NavigationPage)Application.Current.MainPage).Navigation.PopModalAsync(true);

					loggedInHandler();
                }
            });
        }

        #endregion
    }
}
