using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace _13033.SharedPrefs
{
    public class RememberPrefs
    {
        public const string FileName = "RememberPrefs";
        public const string Remember = "Remember";
        public const string Surname = "Surname";
        public const string Name = "Name";
        public const string Address = "Address";

        ISharedPreferences Prefs;
        public RememberPrefs(Context Context)
        {
            Prefs = Context.GetSharedPreferences(FileName, FileCreationMode.Private);
        }

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