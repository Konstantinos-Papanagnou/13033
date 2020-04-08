using Android.Content;

namespace _13033.SharedPrefs
{
    /// <summary>
    /// Handles the maximum limit
    /// </summary>
    public class TextsPerDay
    {
        public const string TextsPref = "TextsPerDay";//filename
        public const string Day = "Day";//Key that holds the last day something has been sent
        public const string Count = "Count";//holds the count of messages sent at the day specified above
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
        /// <summary>
        /// Clears the count each time the day changes
        /// </summary>
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