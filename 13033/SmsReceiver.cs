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
namespace _13033
{
    [BroadcastReceiver]
    [IntentFilter(new[] { "SENT" })]
    public class SmsReceiver : BroadcastReceiver
    {
        readonly Context Context;
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
                    TextsPerDay texts = new TextsPerDay(Context);
                    texts.AddToCount();
                    assist = new Android.Support.V7.App.AlertDialog.Builder(Context)
                    .SetTitle(Resource.String.Attention).SetMessage(Context.Resources.GetString(Resource.String.AttentionMessage) + " " + DateTime.Now.AddHours(3).ToString("hh:mm")).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) => { }).Show();
                    break;
                default:
                    assist = new Android.Support.V7.App.AlertDialog.Builder(Context)
                   .SetTitle(Resource.String.Error).SetIcon(Resource.Drawable.Error).SetMessage(Resource.String.SmsFailed).SetPositiveButton(Resource.String.OK, (object o, DialogClickEventArgs arg) => { }).Show();
                    break;
            }
        }
    }
}