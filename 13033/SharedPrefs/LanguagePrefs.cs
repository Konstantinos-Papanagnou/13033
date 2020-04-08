using Android.Content;

namespace _13033.SharedPrefs
{
    public class LanguagePrefs
    {
        private const string LANGUAGE_PREF = "LanguagePrefs";//file name
        private const string SELECTEDLANG_KEY = "SelectedLang";//key entry that holds the lang
        private readonly ISharedPreferences prefs;
        public LanguagePrefs(Context context)
        {
            prefs = context.GetSharedPreferences(LANGUAGE_PREF, FileCreationMode.Private);
        }

        public string GetLanguageCode()
        {
            return prefs.GetString(SELECTEDLANG_KEY, "el");
        }

        public void SetLanguageCode(string code)
        {
            using (ISharedPreferencesEditor edit = prefs.Edit())
            {
                edit.PutString(SELECTEDLANG_KEY, code);
                edit.Apply();
                edit.Commit();
            }
        }
    }
}