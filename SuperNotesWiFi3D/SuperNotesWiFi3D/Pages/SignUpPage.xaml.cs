using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;
using SuperNotesWiFi3D.Pages.Controls;


namespace SuperNotesWiFi3D.Pages
{
    public partial class SignUpPage : ContentPage
    {
        public Action loggedInHandler = null;


        public SignUpPage()
        {
            InitializeComponent();
        }

		internal void OnSignUpButtonClicked(object sender, EventArgs e)
		{
            if (string.IsNullOrEmpty(_emailField.Text))
            {
                DisplayAlert("Error", "Please enter email", "OK");
            }
            else if (!Regex.IsMatch(_emailField.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                DisplayAlert("Error", "Please enter valid email", "OK");
            }
            else if (string.IsNullOrEmpty(_passwordField.Text))
            {
                DisplayAlert("Error", "Please enter password", "OK");
            }
            else if (_passwordField.Text.Length < 6) 
            {
                DisplayAlert("Error", "Password must be at least 6 letters", "OK");
            }
            else 
            {
				_emailField.IsEnabled = false;
				_passwordField.IsEnabled = false;
                _userNameField.IsEnabled = false;
                _signUpButton.IsVisible = false;
                _cancelButton.IsVisible = false;
				_signUpSpinner.IsRunning = true;

                DataManager.SharedInstace.CreateNewUser(_emailField.Text, _passwordField.Text, _userNameField.Text, delegate (Error error)
                {
                    Device.BeginInvokeOnMainThread(delegate () {
						if (error == null)
						{
                            _signUpSpinner.IsRunning = false;

                            ((NavigationPage)Application.Current.MainPage).Navigation.PopModalAsync(true);
                            ((NavigationPage)Application.Current.MainPage).Navigation.PopModalAsync(true);

                            loggedInHandler();
						}
						else
						{
							_emailField.IsEnabled = true;
							_passwordField.IsEnabled = true;
                            _userNameField.IsEnabled = true;
                            _signUpButton.IsVisible = true;
							_cancelButton.IsVisible = true;
                            _signUpSpinner.IsRunning = false;

                            DisplayAlert("Error", error.Message, "OK");
						}
                    });

                });
            }
		}

        internal void OnCancelButtonClicked(object sender, EventArgs e)
        {
            ((NavigationPage)Application.Current.MainPage).Navigation.PopModalAsync(true);
        }
    }
}
