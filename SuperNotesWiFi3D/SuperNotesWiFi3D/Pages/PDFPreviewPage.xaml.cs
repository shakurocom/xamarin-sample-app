using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SuperNotesWiFi3D.Pages
{
    public partial class PDFPreviewPage : ContentPage
    {
        public string PDFfilepath = null;

        public PDFPreviewPage()
        {
            InitializeComponent();

			// navigation bar
			NavigationPage.SetHasNavigationBar(this, false);

			// status bar
			_statusBarBackground.HeightRequest = App.StyleHelper.StatusBarBackground_Height;
			_statusBarBackground.IsVisible = App.StyleHelper.StatusBarBackground_IsVisible;

            _topBarTitleLabel.Text = "Preview PDF";
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();



            _webView.Source = "file://" + PDFfilepath;
        }


        #region Interface callbacks

        public void TopBarBackButtonClicked(object sender, EventArgs e)
        {
            App.FileHelper.DeleteFile(PDFfilepath);
            Navigation.PopAsync(true);
        }

        public void SendPDFButtonClicked(object sender, EventArgs e)
        {
            //TODO: send pdf to email
        }

        #endregion
    }
}
