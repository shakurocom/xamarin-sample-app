using System;
using Xamarin.Forms;
using SQLite;


namespace SuperNotesWiFi3D.Model
{
    public class NoteData
    {
        public DateTime creationDate;
        public string name;
        public string imageURLstring;
        public string text;
    }

    [Table("Notes")]
    public class DBNote
    {
        #region saved properties

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }

        public int UserID { get; set; }

        public string UUID { get; set; }

        public DateTime CreationDate { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string ImageURLString { get; set; }

		#endregion


		#region calculated properties

		private string _creationDateString = null;
		[Ignore]
		public string CreationDateString
		{
			get
			{
				if (_creationDateString == null)
				{
					_creationDateString = string.Format("{0:hh:mm:ss dd-MM-yyyy}", CreationDate);
				}
				return _creationDateString;
			}
		}

        [Ignore]
        public bool IsImageAvailable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImageURLString);
            }
        }

        #endregion


        public static DBNote FromNoteData(string UUID, NoteData data)
        {
            return new DBNote
            {
                UUID = UUID,
                CreationDate = data.creationDate,
                Name = data.name,
                Text = data.text,
                ImageURLString = data.imageURLstring
            };
        }
    }
}
