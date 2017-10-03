using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SuperNotesWiFi3D.Model
{
    public class Database
    {
        private SQLiteAsyncConnection _dbConnection;


        public Database(string databaFilePath)
        {
            _dbConnection = new SQLiteAsyncConnection(databaFilePath);
            _dbConnection.CreateTableAsync<DBNote>(CreateFlags.None);
            _dbConnection.CreateTableAsync<DBUser>(CreateFlags.None);
        }

        public async Task<DBUser> FetchOrAddUserAsync(string UUID, string email, string name)
        {
            DBUser resultUser = null;

            string sqlQueryFind = string.Format("SELECT * FROM Users WHERE UUID == '{0}'", UUID);
            var existingUsers = await _dbConnection.QueryAsync<DBUser>(sqlQueryFind);
            if (existingUsers.Count > 0)
            {
                resultUser = existingUsers[0];
            }
            else
            {
                resultUser = new DBUser()
                {
                    UUID = UUID,
                    Email = email,
                    Name = name
                };
                await _dbConnection.InsertAsync(resultUser);
                return resultUser;
            }

            return resultUser;
        }

        public async Task<List<DBNote>> FetchNotesAsync(int userID)
        {
            string sqlQuery = string.Format("SELECT * FROM Notes WHERE UserID = {0} ORDER BY CreationDate DESC", userID);
            List<DBNote> notes = await _dbConnection.QueryAsync<DBNote>(sqlQuery);
            return notes;
        }

        public async Task<int> CountNotesAsync(int userID)
        {
            string sqlQuery = string.Format("SELECT COUNT(*) FROM Notes WHERE UserID = {0}", userID);
            return await _dbConnection.ExecuteScalarAsync<int>(sqlQuery);
        }

        public async Task AddNoteAsync(int userID, DBNote newNote)
        {
            newNote.UserID = userID;
            await _dbConnection.InsertOrReplaceAsync(newNote);
        }

        public async Task SetNotesAsync(int userID, List<DBNote> newNotes)
        {
            // delete previous ones
            string sqlQueryDelete = string.Format("DELETE FROM Notes WHERE UserID = {0}", userID);
            var deletedCount = await _dbConnection.ExecuteScalarAsync<int>(sqlQueryDelete);

            // add new ones
            foreach (var note in newNotes)
            {
                note.UserID = userID;
            }
            await _dbConnection.InsertAllAsync(newNotes);
        }

        public async Task<DBNote> GetNoteAsync(int noteID)
        {
            return await _dbConnection.GetAsync<DBNote>(noteID);
        }
    }
}
