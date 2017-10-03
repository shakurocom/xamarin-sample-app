using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.Pages
{
    public partial class HomePage : ContentPage
    {
        #region Initialization

        public HomePage()
        {
            // init
            InitializeComponent();

            // navigation bar
            NavigationPage.SetHasNavigationBar(this, false);

            // status bar
            _statusBarBackground.HeightRequest = App.StyleHelper.StatusBarBackground_Height;
            _statusBarBackground.IsVisible = App.StyleHelper.StatusBarBackground_IsVisible;

            // top bar
            _topBarTitleLabel.Text = "Home";
            _topBarAddNoteButton.Clicked += TopBarAddNoteButtonClicked;

            // table
            _notesTable.RefreshCommand = new Command(() => { RefreshNotesTable(); }, () => { return true; });
            _notesTable.ItemSelected += NotesTableCellSelected;

            // bottom bar
            _bottomBarShowUserProfileButton.Clicked += ShowUserProfileButtonClicked;

            // update state
            UserChangedEventHandler(null, null);

			// notifications
            DataManager.SharedInstace.UserChangedEventHandler += UserChangedEventHandler;
            DataManager.SharedInstace.NoteAddedEventHandler += NoteAddedEventHandler;
            DataManager.SharedInstace.NoteListChangedEventHandler += NoteListUpdatedEventHandler;
        }

        ~HomePage()
        {
			// notifications
			DataManager.SharedInstace.UserChangedEventHandler -= UserChangedEventHandler;
            DataManager.SharedInstace.NoteAddedEventHandler -= NoteAddedEventHandler;
            DataManager.SharedInstace.NoteListChangedEventHandler -= NoteListUpdatedEventHandler;
        }

        #endregion


        #region Events

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DataManager.SharedInstace.CurrentUser() == null &&
                !DataManager.SharedInstace.CurrentApplicationState.LoginState.isShown)
            {
				// request list of notes to be updated
                var _ = DataManager.SharedInstace.RestoreUserAsync(delegate (Error error)
                {
					// refresh notes
					Device.BeginInvokeOnMainThread(delegate ()
                    {
                        if (error != null && error.Code != ErrorCode.InternetUnreachable)
                        {
                            DisplayAlert("Error", error.Message, "OK");
                        }
                        else if (error == null && DataManager.SharedInstace.CurrentUser() == null)
                        {
                            var _2 = DataManager.SharedInstace.LoginSequence();
                        }
                    });
                });
            }

            // restore state
            ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;
            if (currentState.AddNoteState.needToBeShown && !currentState.AddNoteState.isShown) 
            {
                Navigation.PushAsync(new AddNotePage(), false);
            }
            if (currentState.ProfileState.needToBeShown && !currentState.ProfileState.isShown)
            {
                Navigation.PushAsync(new UserProfilePage(), false);
            }
        }

        #endregion


        #region Interface callbacks 

        public void TopBarAddNoteButtonClicked(object sender, EventArgs e) 
        {
            ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;
            currentState.AddNoteState.needToBeShown = true;
            Navigation.PushAsync(new AddNotePage(), true);
        }

        public void ShowUserProfileButtonClicked(object sender, EventArgs e)
        {
			ApplicationState currentState = DataManager.SharedInstace.CurrentApplicationState;
            currentState.ProfileState.needToBeShown = true;
            Navigation.PushAsync(new UserProfilePage(), true);
        }

        public void NotesTableCellSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // deselect
            _notesTable.SelectedItem = null;

            // show note-specific page
            DBNote note = e.SelectedItem as DBNote;
            if (note != null)
            {
                var notePage = new NotePage();
                notePage.noteID = note.ID;
                Navigation.PushAsync(notePage, true);
            }
        }

        #endregion


        #region Messaging

        void UserChangedEventHandler(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(delegate () {
                User currentUser = DataManager.SharedInstace.CurrentUser();

				_topBarAddNoteButton.IsEnabled = currentUser?.authData != null;
				_bottomBarShowUserProfileButton.IsEnabled = currentUser != null;
                if (currentUser != null)
                {
                    if (currentUser.authData != null)
                    {
                        _notesTable.BeginRefresh(); // request fresh data from server
                    }
                    else
                    {
                        ReloadNotesTable(); // get data that is left in database
                    }
                }
            });
        }

        void NoteAddedEventHandler(object sender, EventArgs e) 
        {
			Device.BeginInvokeOnMainThread(delegate () {
				ReloadNotesTable();
			});
        }

        void NoteListUpdatedEventHandler(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(delegate () {
                ReloadNotesTable();
            });
        }

        #endregion


        #region Private

        private void ReloadNotesTable() 
        {
            _notesTable.ItemsSource = null;
            DataManager.SharedInstace.CurrentUserNotesAsync().ContinueWith(delegate (Task<List<DBNote>> notesTask)
            {
                Device.BeginInvokeOnMainThread(delegate
                {
                    _notesTable.ItemsSource = notesTask.Result;
                });
            });
        }

        private void RefreshNotesTable()
        {
            DataManager.SharedInstace.RefreshNotesList(delegate (Error error) {
                _notesTable.EndRefresh();
            });
        }

        #endregion
    }
}
