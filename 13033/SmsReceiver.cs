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
using _13033.SharedPrefs;
using Android.Provider;
using Android.Support.Design.Widget;

namespace _13033
{
    [BroadcastReceiver]
    [IntentFilter(new[] { "SENT" })]
    public class SmsReceiver : BroadcastReceiver
    {
        readonly Context Context;
        int time = 0;
        readonly int[] AvailableTime = { 15, 30, 60 };
        View view;
        DateTime sentAt;
        Android.Support.V7.App.AlertDialog alertDialog;
        public SmsReceiver(Context Context)
        {
            this.Context = Context;
        }
        public SmsReceiver() { }
        public override void OnReceive(Context context, Intent intent)
        {
            Android.Support.V7.App.AlertDialog assist;
            switch (ResultCode)
            {
               
                case Result.Ok:
                    if (Context == null)
                        break;
                    TextsPerDay texts = new TextsPerDay(Context);
                    texts.AddToCount();
                    sentAt = DateTime.Now.AddHours(3);
                    assist = new Android.Support.V7.App.AlertDialog.Builder(Context)
                    .SetTitle(Resource.String.Attention).SetMessage(Context.Resources.GetString(Resource.String.AttentionMessage) + " " + sentAt.ToString("hh:mm")).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) => {
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
                    if (Context == null)
                        break;
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