using System;
using System.IO;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.Pages
{
    public partial class AddNotePage : ContentPage
    {
        private string _imagePath = null;


        #region Initialization

        public AddNotePage()
        {
            InitializeComponent();

            // nav bar
            NavigationPage.SetHasNavigationBar(this, false);

			// status bar
			_statusBarBackground.HeightRequest = App.StyleHelper.StatusBarBackground_Height;
			_statusBarBackground.IsVisible = App.StyleHelper.StatusBarBackground_IsVisible;

			// top bar
			_topBarTitleLabel.Text = "Add Note";

            _imageView.CacheType = FFImageLoading.Cache.CacheType.Memory;

            _mainSpinnerBlock.IsVisible = false;
            _addImageFromLibraryButton.IsEnabled = App.CameraPageHelper != null ? App.CameraPageHelper.IsLibraryAvailable() : false;
            _addImageFromCameraButton.IsEnabled = App.CameraPageHelper != null ? App.CameraPageHelper.IsCameraAvailable() : false;
        }

        #endregion


        #region Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // restore & update state
            ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;
            currentState.AddNoteState.isShown = true;
			// restore state ?
			if (currentState.AddNoteState.needToBeShown)
            {
                _nameField.Text = currentState.AddNoteState.name;
                _textField.Text = currentState.AddNoteState.text;
                SetImageSource(currentState.AddNoteState.imagePath);
            }
            else
            {
                throw new Exception("Error. 'add note' page is shown, when 'AddNoteState.needToBeShown' is FALSE.");
            }
        }

        #endregion


        #region Interface Callbacks

        public void AddImageFromLibraryButtonClicked(object sender, EventArgs e)
        {
            ShowModalImagePicker(false);
        }

		public void AddImageFromCameraButtonClicked(object sender, EventArgs e)
		{
            ShowModalImagePicker(true);
		}

        public void TopBarDoneButtonClicked(object sender, EventArgs e)
        {
            AddNewNote();
        }

        public void TopBarBackButtonClicked(object sender, EventArgs e)
        {
			DismissSelf();
        }

		#endregion


		#region Private

		private void ShowModalImagePicker(bool isCamera)
		{
			// save state
			ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;
			currentState.AddNoteState.name = _nameField.Text;
			currentState.AddNoteState.text = _textField.Text;

			// display camera
			App.CameraPageHelper.GetImageModalAsync(isCamera, delegate (string imagePath)
			{
                if (imagePath != null)
                {
                    SetImageSource(imagePath);
                }
                else
                {
					DisplayAlert("Error", "Can't pick image", "OK");
                }
			});
		}

        private void AddNewNote()
        {
            // show spinner
            _mainSpinnerBlock.IsVisible = true;

            // get data
            DateTime time = DateTime.Now;
            string text = _textField.Text;
            string name = _nameField.Text;
            Stream imageStream = null;
            if (_imagePath != null)
            {
                try
                {
                    imageStream = App.FileHelper.OpenFile(_imagePath);
                }
                catch (Exception)
                {
                    DisplayAlert("Error", "Can't open image file. Please retry.", "OK");

                    // hide spinner
                    _mainSpinnerBlock.IsVisible = false;
                }
            }

            // do request to server
            DataManager.SharedInstace.AddNote(time, name, imageStream, text, delegate (Error error)
            {
                if (imageStream != null)
                {
                    imageStream.Dispose();
                }
                Device.BeginInvokeOnMainThread(delegate
                {
                    if (error == null)
                    {
                        App.FileHelper.DeleteFile(_imagePath);

                        // done
                        if (Navigation.NavigationStack.Count > 0)
                        {
                            DismissSelf();
                        }
                    } 
                    else
                    {
                        // fail
                        DisplayAlert("Error", error.Message, "OK");

                        // hide spinner
                        _mainSpinnerBlock.IsVisible = false;
                    }
                });
            });
        }

        private void SetImageSource(string newImagePath)
        {
            _addImageBlock.IsVisible = newImagePath == null;
            _imageView.IsVisible = newImagePath != null;
            _imageView.Source = newImagePath;
            _imagePath = newImagePath;
            DataManager.SharedInstace.CurrentApplicationState.AddNoteState.imagePath = newImagePath;
        }

        private void DismissSelf()
        {
			ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;

			// we are disappering by user's actions - change state
			currentState.AddNoteState.name = null;
			currentState.AddNoteState.imagePath = null;
			currentState.AddNoteState.text = null;
			currentState.AddNoteState.needToBeShown = false;
			currentState.AddNoteState.isShown = false;

			Navigation.PopAsync(true);
        }

        #endregion
    }
}
