using System.IO;
using Xamarin.Forms;


namespace SuperNotesWiFi3D.Model
{
    public struct AddNoteState
    {
        public bool needToBeShown;
        public bool isShown;
        public string name;
        public string imagePath;
        public string text;
    }

    public struct ProfileState
    {
        public bool needToBeShown;
        public bool isShown;
    }

    public struct LoginState
    {
        public bool isShown;
    }

    public class ApplicationState
    {
        // -- Home
        // no actual state

        // -- Login
        public LoginState LoginState;

        // -- Add Note
        public AddNoteState AddNoteState;

        // -- Profile
        public ProfileState ProfileState;

        public ApplicationState()
        {
            // login
            LoginState.isShown = false;

            // add note
            AddNoteState.needToBeShown = false;
            AddNoteState.isShown = false;
            AddNoteState.name = null;
            AddNoteState.imagePath = null;
            AddNoteState.text = null;

            // profile
            ProfileState.isShown = false;
            ProfileState.needToBeShown = false;
        }
    }
}
