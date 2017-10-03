using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;


namespace SuperNotesWiFi3D.Model
{
    public class FirebaseApiClient
    {
        private readonly FirebaseConfig _firebaseConfig;
        private readonly FirebaseAuthProvider _firebaseAuthProvider;
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;

        private readonly Func<User> _currentUserProvider;
        private readonly Func<Task> _loginSequence;

        private const string Key_Users = "users";


        #region Initialization

        public FirebaseApiClient(Func<User> currentUserProvider, Func<Task> loginSequence)
        {
            _currentUserProvider = currentUserProvider;
            _loginSequence = loginSequence;

            _firebaseConfig = new FirebaseConfig("AIzaSyAqHIXOmzA0UgyaYYV7IGTjzzcxeNm9YZk");

            _firebaseAuthProvider = new FirebaseAuthProvider(_firebaseConfig);

            var databaseOptions = new FirebaseOptions();
            databaseOptions.AuthTokenAsyncFactory = async delegate
            {
                if (currentUserProvider() == null)
                {
                    await loginSequence();
                }

                FirebaseAuthLink authData = null;
                User currentUser = currentUserProvider();
                if (currentUser.authData == null)
                {
                    // silent login
                    if (currentUser.email != null && currentUser.password != null)
                    {
                        authData = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(currentUser.email, currentUser.password);
                    }
                    else if (currentUser.facebookToken != null)
                    {
                        authData = await _firebaseAuthProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, currentUser.facebookToken);
                    }
                    currentUser.authData = authData;
                }
                else
                {
                    authData = currentUser.authData;
                }

                return authData?.FirebaseToken;
            };

            databaseOptions.SyncPeriod = TimeSpan.Zero;

            _firebaseClient = new FirebaseClient("https://supernoteswifi3d.firebaseio.com/", databaseOptions);

            var storageOptions = new FirebaseStorageOptions();
            storageOptions.AuthTokenAsyncFactory = databaseOptions.AuthTokenAsyncFactory;
            _firebaseStorage = new FirebaseStorage("supernoteswifi3d.appspot.com", storageOptions);
        }

        #endregion


        #region Auth

        public async Task SignInWithEmailAndPasswordAsync(string email, string password, Action<FirebaseAuthLink, Error> completionHandler)
        {
            FirebaseAuthLink authData = null;
            Error error = null;

            try
            {
                authData = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception e)
            {
                error = ErrorFromFibaseException(e);
            }

            completionHandler(authData, error);
        }

        public async Task CreateNewUserWithEmailAndPasswordAsync(string email, string password, Action<FirebaseAuthLink, Error> completionHandler)
        {
            FirebaseAuthLink authData = null;
            Error error = null;

            // create user
            try
            {
                authData = await _firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception e)
            {
                error = ErrorFromFibaseException(e);
            }

            completionHandler(authData, error);
        }

        public async Task SignInWithFacebookTokenAsync(string token, Action<FirebaseAuthLink, Error> completionHandler)
        {
            FirebaseAuthLink authData = null;
            Error error = null;

            try
            {
                authData = await _firebaseAuthProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, token);
            }
            catch (Exception e)
            {
                error = ErrorFromFibaseException(e);
            }

            completionHandler(authData, error);
        }

        #endregion


        #region Data Requests

        public async Task RequestUserDataAsync(string userID, Action<UserData, Error> completionHandler)
        {
            UserData userData = null;
            Error error = null;

            try
            {
                userData = await _firebaseClient
                    .Child(Key_Users)
                    .Child(userID)
                    .OnceSingleAsync<UserData>();
            }
            catch (Exception e)
            {
                error = ErrorFromFibaseException(e);
            }

            // done
            completionHandler(userData, error);
        }

        public async Task UpdateUserDataAsync(string userID, string email, string displayName, Action<UserData, Error> completionHandler)
        {
            UserData userData = null;
            Error error = null;

            try
            {
                userData = new UserData()
                {
                    name = displayName,
                    email = email
                };

                await _firebaseClient
                        .Child("users")
                        .Child(userID)
                        .PutAsync(userData);
            }
            catch (FirebaseException e)
            {
                error = new Error(ErrorCode.UnknownError, e.StatusCode.ToEnumString());
            }
            catch (Exception)
            {
                error = new Error(ErrorCode.UnknownError, "Unknown error. Please retry.");
            }

            completionHandler(userData, error);
        }

        internal async Task RequestNotesListAsync(Action<List<DBNote>, Error> completionHandler)
        {
            List<DBNote> responseNotes = new List<DBNote>();
            Error error = null;

            if (_currentUserProvider() == null)
            {
                await _loginSequence();
            }
            string userID = _currentUserProvider().UUID;

            try
            {
                var notesList = await _firebaseClient
                    .Child("notes")
                    .Child(userID)
                    .OnceAsync<NoteData>();

                foreach (var item in notesList)
                {
                    responseNotes.Add(DBNote.FromNoteData(item.Key, item.Object));
                }
            }
            catch (Exception e)
            {
                error = ErrorFromFibaseException(e);
            }

            completionHandler(responseNotes, error);
        }

        internal async Task AddNoteAsync(DateTime time,
                                         string name,
                                         System.IO.Stream imageStream,
                                         string text,
                                         Action<DBNote, Error> completionHandler)
        {
            DBNote newNote = null;
            Error error = null;

            // get user ID
            if (_currentUserProvider() == null)
            {
                await _loginSequence();
            }
            string userID = _currentUserProvider().UUID;

            // upload image to firebase storage
            string uploadedImageURL = null;
            if (error == null && imageStream != null)
            {
                try
                {
                    string imageUUID = Guid.NewGuid().ToString() + ".jpg";
                    var uploadTask = _firebaseStorage
                        .Child("user")
                        .Child(userID)
                        .Child(imageUUID)
                        .PutAsync(imageStream);
                    uploadedImageURL = await uploadTask;
                }
                catch (Exception e)
                {
                    error = ErrorFromFibaseException(e);
                }
            }

			// create note entity in database
			if (error == null)
            {
                try
                {
                    NoteData noteData = new NoteData();
                    noteData.creationDate = time;
                    noteData.name = name;
                    noteData.imageURLstring = uploadedImageURL;
                    noteData.text = text;

                    var postedNoteData = await _firebaseClient
                        .Child("notes")
                        .Child(userID)
                        .PostAsync(noteData, false);
                    newNote = DBNote.FromNoteData(postedNoteData.Key, postedNoteData.Object);
                }
                catch (Exception e)
                {
                    error = ErrorFromFibaseException(e);
                }
            }

            // done
            completionHandler(newNote, error);
        }

        #endregion


        #region Error handling

        private Error ErrorFromFibaseException(Exception e)
        {
            ErrorCode errorCode = ErrorCode.UnknownError;
            string message = null;
            string responseString = null;

            if (e is FirebaseAuthException)
            {
                responseString = ((FirebaseAuthException)e).ResponseData;
                errorCode = ErrorCode.Firebase_Auth;
            }
            else if (e is FirebaseException)
            {
                responseString = ((FirebaseException)e).ResponseData;
                errorCode = ErrorCode.Firebase_Client;
            }
            else if (e is FirebaseStorageException)
            {
                responseString = ((FirebaseStorageException)e).ResponseData;
                errorCode = ErrorCode.Firebase_Storage;
            }

            Error error = null;
            if (errorCode != ErrorCode.UnknownError && message == null)
            {
                try
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                    if (response != null)
                    {
                        if (response.error is string)
                        {
                            message = response.error;
                        }
                        else
                        {
                            message = response.error.message;
                        }
                    }
                }
                catch (Exception)
                {
                    message = "Unknown error. Please retry";
                }
            }

            if (message == null)
            {
                message = "Unknwn error. Please retry.";
            }
            error = new Error(errorCode, message);

            return error;
        }

        #endregion
    }
}
