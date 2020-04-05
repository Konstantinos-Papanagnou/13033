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
    public class LanguagePrefs
    {
        private const string LANGUAGE_PREF = "LanguagePrefs";
        private const string SELECTEDLANG_KEY = "SelectedLang";
        private readonly ISharedPreferences prefs;
        public LanguagePrefs(Context context)
        {
            prefs = context.GetSharedPreferences(LANGUAGE_PREF, FileCreationMode.Private);
        }

        public string GetLanguageCode()
        {
            return prefs.GetString(SELECTEDLANG_KEY, "gr");
        }

        public void SetLanguageCode(string code)
        {
            using(ISharedPreferencesEditor edit = prefs.Edit())
            {
                edit.PutString(SELECTEDLANG_KEY, code);
                edit.Apply();
                edit.Commit();
            }
        }
    }
}