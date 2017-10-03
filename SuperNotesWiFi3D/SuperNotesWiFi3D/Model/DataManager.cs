using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Xamarin.Forms;
using Firebase.Auth;
using SuperNotesWiFi3D.Pages;


namespace SuperNotesWiFi3D.Model
{
    /// <summary>
    /// Main singleton of the app.
    /// </summary>
    public class DataManager
    {
        public ApplicationState CurrentApplicationState;

        private User _currentUser;
        private FirebaseApiClient _firebaseApiClient;
        private IFacebookApiClient _facebookApiClient;
        private Database _database;

        public event EventHandler UserChangedEventHandler;
        public event EventHandler UserUpdatedEventHandler;
        public event EventHandler NoteAddedEventHandler;        // new note added to the top
        public event EventHandler NoteListChangedEventHandler;  // list changed completly


        #region Initialization

        private DataManager()
        {
            CurrentApplicationState = new ApplicationState();

            _currentUser = null;
            _firebaseApiClient = new FirebaseApiClient(CurrentUser, LoginSequence);
            _facebookApiClient = App.FacebookApiClient;
            _facebookApiClient.SetFacebookAppID("351245998633700");
            _database = new Database(App.FileHelper.PersistentLocalFilePath("mainSQLite.db3"));
        }

		#endregion


		public async Task LoginSequence()
		{
            SemaphoreSlim sem = new SemaphoreSlim(0, 1);

            Device.BeginInvokeOnMainThread(delegate () 
            {
                LoginPage page = new LoginPage();
                page.loggedInHandler = delegate ()
                {
                    sem.Release();
                };
                ((NavigationPage)Application.Current.MainPage).Navigation.PushModalAsync(page, true);
            });

            await sem.WaitAsync();
		}


        #region Singleton

        private static volatile DataManager _sharedInstance;
        private static object syncRoot = new Object();

        public static DataManager SharedInstace
        {
            get
            {
                if (_sharedInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (_sharedInstance == null)
                        {
                            _sharedInstance = new DataManager();
                        }
                    }
                }
                return _sharedInstance;
            }
        }

        #endregion


        #region User Actions

		/// <summary>
		/// Creates the new user on server and performs login with it. 
		/// </summary>
		/// <param name="email">email of the user.</param>
		/// <param name="password">user password.</param>
		/// <param name="displayName">display name of the user.</param>
		/// <param name="completionHandler">Completion handler.</param>
		public void CreateNewUser(string email, string password, string displayName, Action<Error> completionHandler)
		{
            var _ = _firebaseApiClient.CreateNewUserWithEmailAndPasswordAsync(email, password, async delegate (FirebaseAuthLink authData, Error createUserError)
            {
                if (createUserError == null)
                {
                    await ChangeUserAsync(authData.User.LocalId, email, password, null, null, authData);

                    var _2 = _firebaseApiClient.UpdateUserDataAsync(authData.User.LocalId, email, displayName, delegate (UserData userData, Error updateUserDataError)
                    {
                        if (updateUserDataError == null)
                        {
                            _currentUser.userData = userData;

                            if (UserUpdatedEventHandler != null)
                            {
                                UserUpdatedEventHandler.Invoke(this, EventArgs.Empty);
                            }
                        }
                        completionHandler(updateUserDataError);
                    });
                }
                else
                {
                    completionHandler(createUserError);
                }
            });
		}

        /// <summary>
        /// Sign in with provided auth credentials.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <param name="completionHandler">Completion handler. Will be called from background thread.</param>
        public void SignInWithEmailAndPassword(string email, string password, Action<Error> completionHandler)
        {
            var _ = _firebaseApiClient.SignInWithEmailAndPasswordAsync(email, password, async delegate (FirebaseAuthLink firebaseAuthLink, Error error)
            {
                if (error == null)
                {
                    await ChangeUserAsync(firebaseAuthLink.User.LocalId, email, password, null, null, firebaseAuthLink);
                }

                // done
                completionHandler(error);
            });
        }

        public void SignInWithFacebook(Action<bool, Error> completionHandler)
        {
            _facebookApiClient.SignIn(delegate (string tokenString, bool isCancelled, Error facebookError)
            {
                if (isCancelled || facebookError != null)
                {
                    completionHandler(isCancelled, facebookError);
                }
                else
                {
                    var _ = _firebaseApiClient.SignInWithFacebookTokenAsync(tokenString, async delegate (FirebaseAuthLink firebaseAuthLink, Error firebaseError)
                    {
                        if (firebaseError == null)
                        {
                            await ChangeUserAsync(firebaseAuthLink.User.LocalId, null, null, tokenString, null, firebaseAuthLink);
                        }

                        completionHandler(false, firebaseError);
                    });
                }
            });
        }

        public User CurrentUser()
        {
            return _currentUser;
        }

        /// <summary>
        /// Restore previous user.
        /// </summary>
        public async Task RestoreUserAsync(Action<Error> completionHandler)
        {
            if (_currentUser == null)
            {
                string userID = null;
                string email = null;
                string password = null;
                string fbToken = null;

                // try to restore user from provious session
                userID = Application.Current.Properties.ContainsKey("userID") ? (string)Application.Current.Properties["userID"] : null;
                email = Application.Current.Properties.ContainsKey("email") ? (string)Application.Current.Properties["email"] : null;
                password = Application.Current.Properties.ContainsKey("password") ? (string)Application.Current.Properties["password"] : null;
                fbToken = Application.Current.Properties.ContainsKey("fbToken") ? (string)Application.Current.Properties["fbToken"] : null;

                if (!(string.IsNullOrWhiteSpace(userID) &&
                      string.IsNullOrWhiteSpace(email) &&
                      string.IsNullOrWhiteSpace(password) &&
                      string.IsNullOrWhiteSpace(fbToken)))
                {
                    await ChangeUserAsync(userID, email, password, fbToken, null, null);
                }
            }

            if (_currentUser != null)
            {
                var _ = _firebaseApiClient.RequestUserDataAsync(_currentUser.UUID, async delegate (UserData userData, Error error)
                {
                    if (error == null)
                    {
                        await ChangeUserAsync(_currentUser.UUID, _currentUser.email, _currentUser.password, _currentUser.facebookToken, userData, _currentUser.authData);
                    }
                    completionHandler(error);
                });
            }
            else
            {
                completionHandler(null);
            }
        }

        public async Task SignOutAsync()
        {
            if (_currentUser != null)
            {
                await ChangeUserAsync(null, null, null, null, null, null);
            }
        }

        /// <summary>
        /// Create new note on server. Parse server's response. Update local data model.
        /// </summary>
        /// <param name="completionHandler">Completion handler. Will be called on background thread.</param>
        public void AddNote(DateTime time, string name, System.IO.Stream imageStream, string text, Action<Error> completionHandler)
        {
            var _ = _firebaseApiClient.AddNoteAsync(time, name, imageStream, text, async delegate (DBNote addedNote, Error error)
            {
                if (error == null)
                {
                    // update local DB
                    await _database.AddNoteAsync(_currentUser.ID, addedNote);

					// notificate
					if (NoteAddedEventHandler != null)
					{
						NoteAddedEventHandler.Invoke(this, EventArgs.Empty);
					}
                }

                // done
                completionHandler(error);
            });
        }

        public void RefreshNotesList(Action<Error> completionHandler)
        {
            var _ = _firebaseApiClient.RequestNotesListAsync(async delegate (List<DBNote> responseNotesList, Error error) 
            {
                if (error == null)
                {
                    // update local DB
                    await _database.SetNotesAsync(_currentUser.ID, responseNotesList);

                    // notificate
                    if (NoteListChangedEventHandler != null)
                    {
                        NoteListChangedEventHandler.Invoke(this, EventArgs.Empty);
                    }
                }

                completionHandler(error);
            });
        }

        public async Task<List<DBNote>> CurrentUserNotesAsync()
        {
            List<DBNote> notes;

            if (_currentUser == null)
            {
                notes = new List<DBNote>();
            }
            else
            {
                notes = await _database.FetchNotesAsync(_currentUser.ID);
            }

            return notes;
        }

		public async Task<int> CurrentUserNotesCountAsync()
		{
            int count = 0;
            if (_currentUser != null)
            {
                count = await _database.CountNotesAsync(_currentUser.ID);
			}
            return count;
		}

        public async Task<DBNote> GetNoteAsync(int noteID)
        {
            return await _database.GetNoteAsync(noteID);
        }

        #endregion


        #region Private

        private async Task ChangeUserAsync(string UUID, string email, string password, string facebookToken, UserData userData, FirebaseAuthLink authData)
        {
            // update user
            User newUser = null;
            if (UUID != null || email != null || password != null || facebookToken != null || userData != null || authData != null)
            {
                DBUser userFromDB = await _database.FetchOrAddUserAsync(UUID, userData?.email, userData?.name);
                if (userFromDB != null)
                {
                    newUser = new User(UUID, userFromDB.ID, email, password, facebookToken, userData);
					newUser.authData = authData;
                }
            }
            _currentUser = newUser;
            SaveUserCredentials(UUID, email, password, facebookToken);

            // notificate
            if (UserChangedEventHandler != null)
            {
                UserChangedEventHandler.Invoke(this, EventArgs.Empty);
            }
        }

        private void SaveUserCredentials(string userID, string email, string password, string facebookToken)
        {
			if (Application.Current.Properties.ContainsKey("userID"))
			{
				Application.Current.Properties["userID"] = userID;
			}
			else
			{
				Application.Current.Properties.Add("userID", userID);
			}
            if (Application.Current.Properties.ContainsKey("email"))
            {
                Application.Current.Properties["email"] = email;
            }
            else 
            {
                Application.Current.Properties.Add("email", email);
            }
            if (Application.Current.Properties.ContainsKey("password"))
            {
                Application.Current.Properties["password"] = password;
            }
            else
            {
                Application.Current.Properties.Add("password", password);
            }
			if (Application.Current.Properties.ContainsKey("fbToken"))
			{
                Application.Current.Properties["fbToken"] = facebookToken;
			}
			else
			{
				Application.Current.Properties.Add("fbToken", facebookToken);
			}
            Application.Current.SavePropertiesAsync();

			//using Xamarin.Auth;
			//Account account = new Account(email);
			//account.Properties.Add("password", password);
			//AccountStore.Create().Save(account, App.AppName);
		}

        #endregion
    }
}
