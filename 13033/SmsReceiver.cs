using _13033.SharedPrefs;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using System;

namespace _13033
{
    [BroadcastReceiver]
    [IntentFilter(new[] { "SENT" })]
    public class SmsReceiver : BroadcastReceiver
    {
        readonly Context Context;
        int time = 0; // index
        readonly int[] AvailableTime = { 15, 30, 60 };
        View view;
        DateTime sentAt;
        Android.Support.V7.App.AlertDialog alertDialog;
        public SmsReceiver(Context Context)
        {
            this.Context = Context;
        }
        public SmsReceiver() { } // We need a default constructor if the system decides to call on this receiver
        public override void OnReceive(Context context, Intent intent)
        {
            Android.Support.V7.App.AlertDialog assist;
            switch (ResultCode)
            {
                //if we receive an OK status it means that our sms has been sent successfully
                case Result.Ok:
                    //Make sure that this is the manually initialized from the activity Receiver and not the system initialized one
                    if (Context == null)
                        break;
                    //Grab the file to add the sms sent
                    TextsPerDay texts = new TextsPerDay(Context);
                    texts.AddToCount();
                    //set the timer to 3 hours ahead
                    sentAt = DateTime.Now.AddHours(3);
                    //Notify the user that his request has been successfully sent to 13033
                    assist = new Android.Support.V7.App.AlertDialog.Builder(Context)
                    .SetTitle(Resource.String.Attention).SetMessage(Context.Resources.GetString(Resource.String.AttentionMessage) + " " + sentAt.ToString("hh:mm")).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) =>
                    {
                        //ask the user if he wants to be alerted
                        view = ((Activity)Context).LayoutInflater.Inflate(Resource.Layout.AlertDialog, null);
                        ListView list = view.FindViewById<ListView>(Resource.Id.AlertList);
                        list.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItem1, new string[] { "15 " + Context.Resources.GetString(Resource.String.mins), "30 " + Context.Resources.GetString(Resource.String.mins), "1 " + Context.Resources.GetString(Resource.String.Hour) });
                        list.ItemClick += List_ItemClick;
                        alertDialog = new Android.Support.V7.App.AlertDialog.Builder(Context).SetTitle(Resource.String.AlertTitle)
                        .SetMessage(Resource.String.AlertMessage).SetView(view)
                        .SetNegativeButton(Resource.String.Abort, (object o, DialogClickEventArgs e) => { }).Show();
                    }).Show();
                    break;
                default:
                    //Make sure that this is the manually initialized from the activity Receiver and not the system initialized one
                    if (Context == null)
                        break;
                    //Alert the user that something went wrong
                    assist = new Android.Support.V7.App.AlertDialog.Builder(Context)
                   .SetTitle(Resource.String.Error).SetIcon(Resource.Drawable.Error).SetMessage(Resource.String.SmsFailed).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) => { }).Show();
                    break;
            }


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
            i.PutExtra(AlarmClock.ExtraMessage, Context.Resources.GetString(Resource.String.app_name) + " " + AvailableTime[time].ToString() + " " + Context.Resources.GetString(Resource.String.mins) + " " + Context.Resources.GetString(Resource.String.Left));
            i.PutExtra(AlarmClock.ExtraVibrate, true);

            Context.StartActivity(i);
            alertDialog.Dismiss();
        }
    }
}