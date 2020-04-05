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
    public class TextsPerDay
    {
        public const string TextsPref = "TextsPerDay";
        public const string Day = "Day";
        public const string Count = "Count";
        ISharedPreferences Prefs;
        public TextsPerDay(Context Context)
        {
            Prefs = Context.GetSharedPreferences(TextsPref, FileCreationMode.Private);
        }
        public string GetDay()
        {
            return Prefs.GetString(Day, string.Empty);
        }
        public void SetDay(string date)
        {
            using ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutString(Day, date);
            editor.Apply();
            editor.Commit();
            ClearCount();
        }

        public int GetCount()
        {
            return Prefs.GetInt(Count, 0);
        }

        private void ClearCount()
        {
            using ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutInt(Count, 0);
            editor.Apply();
            editor.Commit();
        }

        public void AddToCount()
        {
            using ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutInt(Count, GetCount() + 1);
            editor.Apply();
            editor.Commit();
        }
    }
}