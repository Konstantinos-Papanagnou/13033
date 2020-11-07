using _13033.Adapters;
using _13033.Model;
using _13033.SharedPrefs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
//using Android.Telephony;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace _13033
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        #region Language Handler
        //Indicator whether we changed the language
        public static bool langChanged = false;
        /// <summary>
        /// grab and attach the correct Resource
        /// </summary>
        /// <param name="base"></param>
        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(Manager.LocaleManager.SetLocale(@base));
        }
        /// <summary>
        /// Changes the language when some configuration changes
        /// </summary>
        /// <param name="newConfig"></param>
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            Manager.LocaleManager.SetLocale(this);
        }
        /// <summary>
        /// Resets the app title
        /// </summary>
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
        /// <summary>
        /// Attaches the correct Resource when the app Resumes
        /// </summary>
        protected override void OnResume()
        {
            if (langChanged)
            {
                langChanged = false;
                Recreate();
            }
            base.OnResume();
        }
        #endregion

        TextView ReasonSelectedTV;
        EditText Surname, Name, Address;
        ListView CodeList;
        Switch RememberSW;
        FloatingActionButton Send;
        RememberPrefs Prefs; //Remember me
        //SharedPrefs.TextsPerDay texts; //Handler for the sms limit service

        List<CodeMeta> Data; //Transportation reasons (Could be hardcoded but we chose to grab them from arrays on resource

        int ReasonSelected;

       // SmsReceiver receiver; //The receiver that gets triggered when the sms is successfully sent

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Initialize everything
            base.OnCreate(savedInstanceState);
            ResetTitle();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FindViews();
            SetAdapter();
            Prefs = new RememberPrefs(this);
            //texts = new TextsPerDay(this);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            RememberSW.Checked = Prefs.GetRemember();
            HandleEvents();
            SetViews();
            //Configure the receiver to get triggered
           // receiver = new SmsReceiver(this);
           // RegisterReceiver(receiver, new IntentFilter("SENT"));
        }
        /// <summary>
        /// Sets up the Options menu at the top right of the toolbar
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        /// <summary>
        /// Handle click events on options menu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_Lang)
            {
                //Toggle Languages
                LanguagePrefs prefs = new LanguagePrefs(this);
                if (prefs.GetLanguageCode() == "el")
                    prefs.SetLanguageCode("en");
                else
                    prefs.SetLanguageCode("el");
                langChanged = true;
                //And recreate the app for changes to take effect
                this.Recreate();
                return true;
            }
            else if (id == Resource.Id.action_Info)
            {
                Android.Support.V7.App.AlertDialog info = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle(Resource.String.InfoTitle)
                    .SetMessage(Resource.String.InfoMessage).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs args) => { }).Show();
            }

            return base.OnOptionsItemSelected(item);
        }
        /// <summary>
        /// Find the Views
        /// </summary>
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
        /// <summary>
        /// Handle the click events
        /// </summary>
        private void HandleEvents()
        {
            Send.Click += Send_Click;
            RememberSW.CheckedChange += RememberSW_CheckedChange;
            CodeList.ItemClick += CodeList_ItemClick;
        }

        private void CodeList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Set the Reason selected to the new code the user provided
            ReasonSelected = Data[e.Position].Code;
            //Set the textView to let the user know we received his request
            ReasonSelectedTV.Text = Resources.GetString(Resource.String.SelectedText) + " " + ReasonSelected.ToString();
            //And finally make the TextView visible to the user
            ReasonSelectedTV.Visibility = ViewStates.Visible;
        }

        private void RememberSW_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            //Toggle the remember state
            Prefs.ToggleRemember(e.IsChecked);
        }

        private void SetAdapter()
        {
            //Load and configure our data
            Data = new List<CodeMeta>();
            string[] Descriptions = Resources.GetStringArray(Resource.Array.Description);
            for (int i = 0; i < Descriptions.Length; i++)
                Data.Add(new CodeMeta() { Code = i + 1, Description = Descriptions[i] });

            //Set the adapter
            CodeList.Adapter = new Codes(this, Data);
        }

        private void SetViews()
        {
            //Set Views should only work when the user wants to be remembered
            if (!Prefs.GetRemember())
                return;

            Surname.Text = Prefs.GetSurname();
            Name.Text = Prefs.GetName();
            Address.Text = Prefs.GetAddress();
        }

        private void Send_Click(object sender, EventArgs e)
        {
            BeginSend();
            //When we click the send button we need to make sure that we have the required permissions
            //RequestPermissions();
        }

        public void SendText(string final)
        {
            //Create the confirmation dialog to ask the user if they want to procceed
            Android.Support.V7.App.AlertDialog confirmationDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
    .SetTitle(Resource.String.Confirmation).SetMessage(final)
    .SetPositiveButton(Resource.String.Proceed, (object o, DialogClickEventArgs args) =>
    {
        try
        {
            //Send the sms 

            //var sms = SmsManager.Default;
            //We need to create a Pending Intent with the Intent SENT in order to call our receiver because the method SendTextMessage only accepts PendingIntent
            //PendingIntent pi = PendingIntent.GetBroadcast(this, 0, new Intent("SENT"), 0);
            //sms.SendTextMessage("13033", null, final, pi, null);
            var message = new SmsMessage(final, new[] { "13033" });
            Sms.ComposeAsync(message).Wait();
            //EnableTimer();

            /* Since the EnableTimer option is removed this code below will warn the user about the precautions
               * Remove the following piece of code if you want back the enable timer function!*/
            //Notify the user that his request has been successfully sent to 13033
            Android.Support.V7.App.AlertDialog assist = new Android.Support.V7.App.AlertDialog.Builder(this)
            .SetTitle(Resource.String.Attention).SetMessage(Resource.String.AttentionMessage).Show();
        }
        catch (FeatureNotSupportedException)
        {
            Toast.MakeText(this, Resource.String.SmsNotSupportedException, ToastLength.Long).Show();
        }
        catch (Exception ex)
        {
            //Let the user know the error
            Android.Support.V7.App.AlertDialog error = new Android.Support.V7.App.AlertDialog.Builder(this).SetTitle(Resource.String.Error).SetMessage(ex.Message).SetIcon(Resource.Drawable.Error).Show();
        }
    })
    .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs args) => { Toast.MakeText(this, Resource.String.Aborted, ToastLength.Long).Show(); }).Show();
        }


        //private void RequestPermissions()
        //{
        //    //Request the permissions if not already granted
        //    //if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(this, Manifest.Permission.SendSms) != (int)Permission.Granted)
        //    //{
        //    //    Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.SendSms }, 0);
        //    //}
        //    //else
        //    //{
        //        BeginSend();
        //    //}
        //}

        private void BeginSend()
        {
            //Model our message
            Model.Model Message = new Model.Model(ReasonSelected, Surname.Text, Name.Text, Address.Text);
            if (!Message.ConfirmMessage())
            {
                //if the message is missing fields alert the user
                Android.Support.V7.App.AlertDialog dialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetMessage(Resource.String.ConfirmError)
                    .SetTitle(Resource.String.Error)
                    .SetIcon(Resource.Drawable.Error)
                    .SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs args) => { })
                    .Show();
                return;
            }
            //Set the data to remember
            if (Prefs.GetRemember())
                Prefs.SetData(Surname.Text, Name.Text, Address.Text);
            //get the final message to send
            string final = Message.GetMessage();

            /*==============================================================================
             * This method is obsolute based on government decision
             * Keep the code Commented out in case the government changes it's mind
             * ===============================================================================*/

            //if we find out that the user has sent another sms this day
            /* if (texts.GetDay() == DateTime.Now.ToShortDateString())
             {
                 //Check if he has reached the limit
                 if (texts.GetCount() >= 2)
                 {
                     //if he has reached the limit warn him
                     Android.Support.V7.App.AlertDialog confirm = new Android.Support.V7.App.AlertDialog.Builder(this)
                         .SetTitle(Resource.String.Attention).SetMessage(Resource.String.MessagesOverload).SetIcon(Resource.Drawable.Error)
                         .SetPositiveButton(Resource.String.Proceed, (object o, DialogClickEventArgs arg) => { SendText(final); })
                         .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs arg) => { }).Create();
                     confirm.Show();
                 }
                 //else send the text
                 else SendText(final);
             }
             else
             {
                 //if its his first sms the current day then configure it
                 texts.SetDay(DateTime.Now.ToShortDateString());
                 SendText(final);
             }*/
            SendText(final);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //if (requestCode == 0)
            //{
            //    //Check if the only required permission has been granted
            //    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
            //    {
            //        Toast.MakeText(this, "Permission Granted", ToastLength.Long).Show();
            //        //Begin the process
            //        BeginSend();
            //    }
            //    else
            //    {
            //        Toast.MakeText(this, "Permission Not Granted!", ToastLength.Long).Show();
            //    }
            //}
            //else
            //{
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //}
        }


        /*Timer Options Deactivated due to Government's Decision
        
        int time = 0; // index
        readonly int[] AvailableTime = { 15, 30, 60 };
        View view;
        DateTime sentAt;
        Android.Support.V7.App.AlertDialog alertDialog;

        public void EnableTimer()
        {
            //Grab the file to add the sms sent
            TextsPerDay texts = new TextsPerDay(this);
            texts.AddToCount();
            //set the timer to 3 hours ahead
            sentAt = DateTime.Now.AddHours(3);
            //Notify the user that his request has been successfully sent to 13033
            Android.Support.V7.App.AlertDialog assist = new Android.Support.V7.App.AlertDialog.Builder(this)
            .SetTitle(Resource.String.Attention).SetMessage(Resources.GetString(Resource.String.AttentionMessage) + " " + sentAt.ToString("hh:mm")).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) =>
            {
                        //ask the user if he wants to be alerted
                        view = LayoutInflater.Inflate(Resource.Layout.AlertDialog, null);
                ListView list = view.FindViewById<ListView>(Resource.Id.AlertList);
                list.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, new string[] { "15 " + Resources.GetString(Resource.String.mins), "30 " + Resources.GetString(Resource.String.mins), "1 " + Resources.GetString(Resource.String.Hour) });
                list.ItemClick += List_ItemClick;
                alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this).SetTitle(Resource.String.AlertTitle)
                .SetMessage(Resource.String.AlertMessage).SetView(view)
                .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs e) => { }).Show();
            }).Show();
        }

        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            time = e.Position;
            SetAlarm(sentAt);
        }

        private void SetAlarm(DateTime sentAt)
        {
            Intent i = new Intent(AlarmClock.ActionSetAlarm);
            TimeSpan span = sentAt.TimeOfDay;
            span = span.Subtract(TimeSpan.FromMinutes(AvailableTime[time]));
            i.PutExtra(AlarmClock.ExtraHour, span.Hours);
            i.PutExtra(AlarmClock.ExtraMinutes, span.Minutes);
            i.PutExtra(AlarmClock.ExtraSkipUi, true);
            i.PutExtra(AlarmClock.ExtraMessage, Resources.GetString(Resource.String.app_name) + " " + AvailableTime[time].ToString() + " " + Resources.GetString(Resource.String.mins) + " " + Resources.GetString(Resource.String.Left));
            i.PutExtra(AlarmClock.ExtraVibrate, true);

            StartActivity(i);
            alertDialog.Dismiss();
        }
        */
    }
}