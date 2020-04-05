using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using _13033.SharedPrefs;

namespace _13033.Manager
{
    public class LocaleManager
    {
        public static Context SetLocale(Context c)
        {
            return SetNewLocale(c, GetLanguage(c));
        }

        public static Context SetNewLocale(Context c, string language)
        {
            PersistLanguage(c, language);
            return UpdateResources(c, language);
        }

        public static string GetLanguage(Context c)
        {
            LanguagePrefs prefs = new LanguagePrefs(c);
            return prefs.GetLanguageCode();
        }

        private static void PersistLanguage(Context c, string language)
        {
            LanguagePrefs prefs = new LanguagePrefs(c);
            prefs.SetLanguageCode(language);
        }
        private static Context UpdateResources(Context context, string language)
        {
            Locale locale = new Locale(language);
            Resources res = context.Resources;
            Configuration config = new Configuration(res.Configuration);
            if (Build.VERSION.SdkInt >= (BuildVersionCodes)17)
            {
                config.SetLocale(locale);
                context = context.CreateConfigurationContext(config);
            }
            else
            {
                config.Locale = locale;
                res.UpdateConfiguration(config, res.DisplayMetrics);
            }
            return context;
        }
    }
}