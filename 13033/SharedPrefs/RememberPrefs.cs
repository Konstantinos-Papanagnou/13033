using Android.Content;

namespace _13033.SharedPrefs
{
    public class RememberPrefs
    {
        public const string FileName = "RememberPrefs";//filename
        public const string Remember = "Remember";//key that holds the value remember
        public const string Surname = "Surname";// key that holds the value surname
        public const string Name = "Name";//key that holds the value name
        public const string Address = "Address";//key that holds the value address

        ISharedPreferences Prefs;
        public RememberPrefs(Context Context)
        {
            Prefs = Context.GetSharedPreferences(FileName, FileCreationMode.Private);
        }
        /// <summary>
        /// Toggles in between remember 
        /// </summary>
        /// <param name="remember">The value to put</param>
        public void ToggleRemember(bool remember)
        {
            using ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutBoolean(Remember, remember);
            editor.Apply();
            editor.Commit();
        }

        public bool GetRemember()
        {
            return Prefs.GetBoolean(Remember, false);
        }

        public string GetSurname()
        {
            return Prefs.GetString(Surname, string.Empty);
        }
        public string GetName()
        {
            return Prefs.GetString(Name, string.Empty);
        }
        public string GetAddress()
        {
            return Prefs.GetString(Address, string.Empty);
        }
        /// <summary>
        /// Sets and saves all the data
        /// </summary>
        /// <param name="surname"></param>
        /// <param name="name"></param>
        /// <param name="address"></param>
        public void SetData(string surname, string name, string address)
        {
            using ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutString(Surname, surname);
            editor.PutString(Name, name);
            editor.PutString(Address, address);
            editor.Apply();
            editor.Commit();
        }
    }
}