using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;
using FFImageLoading.Work;
using FFImageLoading;
using FFImageLoading.Forms;


namespace SuperNotesWiFi3D.Pages
{
    public partial class NotePage : ContentPage
    {
        public int noteID = -1;
        private DBNote _currentNote = null;
        private string pdfFilepath = null;

        public NotePage()
        {
            InitializeComponent();

			// navigation bar
			NavigationPage.SetHasNavigationBar(this, false);

			// status bar
			_statusBarBackground.HeightRequest = App.StyleHelper.StatusBarBackground_Height;
			_statusBarBackground.IsVisible = App.StyleHelper.StatusBarBackground_IsVisible;

			// top bar
			_topBarTitleLabel.Text = "Note";

            // bottom bar
            _bottomBarCreatePDFButton.IsEnabled = false;
            _bottomBarTestButton.IsEnabled = App.TestHelper.isAvailable();

            _spinnerOverlay.IsVisible = false;
		}


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (noteID >= 0)
            {
                var _ = DataManager.SharedInstace.GetNoteAsync(noteID).ContinueWith( delegate (Task<DBNote> task) 
                {
                    Device.BeginInvokeOnMainThread(delegate () 
                    {
                        var note = task.Result;
                        _currentNote = note;
						_contentStackView.BindingContext = note;

                        _imageView.Source = null;
                        _imageView.Source = note.ImageURLString;

                        _bottomBarCreatePDFButton.IsEnabled = App.PDFHelper.IsAvailable();
                    });
                });
            }
        }


        public void TopBarBackButtonClicked(object sender, EventArgs e)
        {
            if (pdfFilepath != null)
            {
                App.FileHelper.DeleteFile(pdfFilepath);
            }
            Navigation.PopAsync(true);
        }

        public void CreatePDFButtonClicked(object sender, EventArgs e)
        {
            try
            {
                _spinnerOverlay.IsVisible = true;
                App.PDFHelper.CreatePDFAsync(_currentNote.Name, _currentNote.CreationDateString, _currentNote.ImageURLString, _currentNote.Text).ContinueWith(delegate (Task<string> pdfTask)
                {
                    pdfFilepath = pdfTask.Result;
                    var userEmail = DataManager.SharedInstace.CurrentUser().email;
                    if (App.PDFHelper.isAndroid())
                    {
						Device.BeginInvokeOnMainThread(delegate ()
						{
                            DisplayAlert("Done", "PDF file was created at '" + pdfFilepath + "'.", "OK");
						});
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(userEmail))
                        {
                            Device.BeginInvokeOnMainThread(delegate ()
                            {
                                _spinnerOverlay.IsVisible = false;
                                App.PDFHelper.OpenPDF(pdfFilepath);
                            });
                        }
                        else
                        {
                            App.EMailHelper.SendFileToEMail(userEmail, "note PDF", "Empty body. See attached file.", pdfFilepath, delegate (bool success)
                            {
                                Device.BeginInvokeOnMainThread(delegate ()
                                {
                                    _spinnerOverlay.IsVisible = false;
                                    if (success)
                                    {
                                        DisplayAlert("Success", "Check your email '" + userEmail + "' for PDF file.", "OK");
                                    }
                                    else
                                    {
                                        DisplayAlert("Error", "Can't send file to email '" + userEmail + "'.", "OK");
                                    }
                                });
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Device.BeginInvokeOnMainThread(delegate ()
                {
                    _spinnerOverlay.IsVisible = false;
                });
            }
        }

        public void TestButtonClicked(object sender, EventArgs e)
        {
            DisplayAlert("Test", App.TestHelper.TestFunc(), "OK");
        }
    }
}
