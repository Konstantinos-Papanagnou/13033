using _13033.SharedPrefs;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Java.Util;

namespace _13033.Manager
{
    public class LocaleManager
    {
        /// <summary>
        /// Get the new Context with the new resources
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A new context with different resources</returns>
        public static Context SetLocale(Context c)
        {
            return SetNewLocale(c, GetLanguage(c));
        }
        /// <summary>
        /// Sets the resources for the context and then returns the context 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="language"></param>
        /// <returns>Returns the context with the specified resources</returns>
        public static Context SetNewLocale(Context c, string language)
        {
            PersistLanguage(c, language);
            return UpdateResources(c, language);
        }

        /// <summary>
        /// Get the language the user wants
        /// </summary>
        /// <param name="c"></param>
        /// <returns>a string with the language code</returns>
        public static string GetLanguage(Context c)
        {
            LanguagePrefs prefs = new LanguagePrefs(c);
            return prefs.GetLanguageCode();
        }

        //Set the shared prefs to remember the current language
        private static void PersistLanguage(Context c, string language)
        {
            LanguagePrefs prefs = new LanguagePrefs(c);
            prefs.SetLanguageCode(language);
        }
        /// <summary>
        /// Actually updates the resources
        /// </summary>
        /// <param name="context"></param>
        /// <param name="language"></param>
        /// <returns>The new Context</returns>
        private static Context UpdateResources(Context context, string language)
        {
            //Magic :D
            //Create a Locale instance to find the resources with the specified lang code (values-el)
            Locale locale = new Locale(language);
            //get the context resources
            Resources res = context.Resources;
            //get the config of the context
            Configuration config = new Configuration(res.Configuration);
            //Check the Version code for compatibility
            if (Build.VERSION.SdkInt >= (BuildVersionCodes)17)
            {
                config.SetLocale(locale);
                context = context.CreateConfigurationContext(config);
            }
            else
            {
                //API 17 and below change the language with res.UpdateConfiguration which is deprecated for API 17 and above
                config.Locale = locale;
                res.UpdateConfiguration(config, res.DisplayMetrics);
            }
            return context;
        }
    }
}