using System;
using System.Collections.Generic;
using _13033.Model;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using _13033.Adapters;
using _13033.SharedPrefs;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Xamarin.Essentials;

namespace _13033
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        public static bool langChanged = false;
        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(Manager.LocaleManager.SetLocale(@base));
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            Manager.LocaleManager.SetLocale(this);
        }

        private void ResetTitle()
        {
            try
            {

                int label = PackageManager.GetActivityInfo(ComponentName, PackageInfoFlags.MetaData).LabelRes;
                if (label != 0)
                {
                    SetTitle(label);
                }
            }
            catch (PackageManager.NameNotFoundException) { }
        }

        protected override void OnResume()
        {
            if (langChanged)
            {
                langChanged = false;
                Recreate();
            }
            base.OnResume();
        }

        TextView ReasonSelectedTV;
        EditText Surname, Name, Address;
        ListView CodeList;
        Switch RememberSW;
        FloatingActionButton Send;
        RememberPrefs Prefs;
        SharedPrefs.TextsPerDay texts;

        List<CodeMeta> Data;

        int ReasonSelected;

        public object LocaleManager { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ResetTitle();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FindViews();
            SetAdapter();
            Prefs = new RememberPrefs(this);
            texts = new TextsPerDay(this);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            RememberSW.Checked = Prefs.GetRemember();
            HandleEvents();
            SetViews();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_Lang)
            {
                LanguagePrefs prefs = new LanguagePrefs(this);
                if(prefs.GetLanguageCode() == "gr")
                    prefs.SetLanguageCode("en");
                else
                    prefs.SetLanguageCode("gr");
                langChanged = true;
                this.Recreate();
                return true;
            }
            else if(id == Resource.Id.action_Info)
            {
                Android.Support.V7.App.AlertDialog info = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle(Resource.String.InfoTitle)
                    .SetMessage(Resource.String.InfoMessage).SetPositiveButton(Resource.String.OK,(object o, DialogClickEventArgs args) => { }).Show();
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void FindViews()
        {
            Surname = FindViewById<EditText>(Resource.Id.SurnameET);
            Name = FindViewById<EditText>(Resource.Id.NameET);
            Address = FindViewById<EditText>(Resource.Id.AddressET);
            CodeList = FindViewById<ListView>(Resource.Id.TransportCode);
            RememberSW = FindViewById<Switch>(Resource.Id.RememberSW);
            Send = FindViewById<FloatingActionButton>(Resource.Id.Send);
            ReasonSelectedTV = FindViewById<TextView>(Resource.Id.SelectedTV);
        }

        private void HandleEvents()
        {
            Send.Click += Send_Click;
            RememberSW.CheckedChange += RememberSW_CheckedChange;
            CodeList.ItemClick += CodeList_ItemClick;
        }

        private void CodeList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ReasonSelected = Data[e.Position].Code;
            ReasonSelectedTV.Text = Resources.GetString(Resource.String.SelectedText) + " " + ReasonSelected.ToString();
            ReasonSelectedTV.Visibility = ViewStates.Visible;
        }

        private void RememberSW_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Prefs.ToggleRemember(e.IsChecked);
        }

        private void SetAdapter()
        {
            Data = new List<CodeMeta>();
            string[] Descriptions = Resources.GetStringArray(Resource.Array.Description);
            for (int i = 0; i < Descriptions.Length; i++)
                Data.Add(new CodeMeta() { Code = i + 1, Description = Descriptions[i] });

            CodeList.Adapter = new Codes(this, Data);
        }

        private void SetViews()
        {
            if (!Prefs.GetRemember())
                return;

            Surname.Text = Prefs.GetSurname();
            Name.Text = Prefs.GetName();
            Address.Text = Prefs.GetAddress();
        }

        private void Send_Click(object sender, EventArgs e)
        {          
            Model.Model Message = new Model.Model(ReasonSelected, Surname.Text, Name.Text, Address.Text);
            if (!Message.ConfirmMessage())
            {
                Android.Support.V7.App.AlertDialog dialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetMessage(Resource.String.ConfirmError)
                    .SetTitle(Resource.String.Error)
                    .SetIcon(Resource.Drawable.Error)
                    .SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs args) => { })
                    .Show();
                return;
            }
            if (Prefs.GetRemember())
                Prefs.SetData(Surname.Text, Name.Text, Address.Text);
            string final = Message.GetMessage();
            if (texts.GetDay() == DateTime.Now.ToShortDateString())
            {
                if (texts.GetCount() >= 2)
                {
                    Android.Support.V7.App.AlertDialog confirm = new Android.Support.V7.App.AlertDialog.Builder(this)
                        .SetTitle(Resource.String.Attention).SetMessage(Resource.String.MessagesOverload).SetIcon(Resource.Drawable.Error)
                        .SetPositiveButton(Resource.String.Proceed, (object o, DialogClickEventArgs arg) => { SendText(final); })
                        .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs arg) => { }).Create();
                    confirm.Show();
                }
                else SendText(final);
            }
            else
            { 
                texts.SetDay(DateTime.Now.ToShortDateString());
                SendText(final);
            }



        }

        public void SendText(string final)
        {
            Android.Support.V7.App.AlertDialog confirmationDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
    .SetTitle(Resource.String.Confirmation).SetMessage(final)
    .SetPositiveButton(Resource.String.Proceed, (object o, DialogClickEventArgs args) => {
        try
        {
            var message = new SmsMessage(final, new[] { "13033" });
            Sms.ComposeAsync(message).Wait(2000);
            texts.AddToCount();
            Android.Support.V7.App.AlertDialog assist = new Android.Support.V7.App.AlertDialog.Builder(this)
            .SetTitle(Resource.String.Attention).SetMessage(Resources.GetString(Resource.String.AttentionMessage) + " " + DateTime.Now.AddHours(2).ToString("hh:mm")).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) => { }).Show();
        }
        catch (FeatureNotSupportedException)
        {
            Toast.MakeText(this, Resource.String.SmsNotSupportedException, ToastLength.Long).Show();
        }
        catch (Exception ex)
        {
            Android.Support.V7.App.AlertDialog error = new Android.Support.V7.App.AlertDialog.Builder(this).SetTitle(Resource.String.Error).SetMessage(ex.Message).SetIcon(Resource.Drawable.Error).Show();
        }
    })
    .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs args) => { Toast.MakeText(this, Resource.String.Aborted, ToastLength.Long).Show(); }).Show();
        }

    }
}