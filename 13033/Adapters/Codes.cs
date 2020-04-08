using _13033.Model;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace _13033.Adapters
{
    class Codes : BaseAdapter
    {

        readonly Context Context;
        readonly List<CodeMeta> Data;


        public Codes(Context Context, List<CodeMeta> Data)
        {
            this.Context = Context;
            this.Data = Data;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return Data[position];
        }

        public override long GetItemId(int position)
        {
            return Data[position].Code;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            CodesViewHolder holder = null;

            if (view != null)
                holder = view.Tag as CodesViewHolder;

            if (holder == null)
            {
                holder = new CodesViewHolder();
                var inflater = Context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.CodeRow, parent, false);
                holder.Code = view.FindViewById<TextView>(Resource.Id.CodeTV);
                holder.Description = view.FindViewById<TextView>(Resource.Id.DescriptionTV);
                view.Tag = holder;
            }


            //fill in your items
            holder.Code.Text = Data[position].Code.ToString();
            holder.Description.Text = Data[position].Description;
            return view;
        }

        public override int Count
        {
            get
            {
                return Data.Count;
            }
        }

    }

    class CodesViewHolder : Java.Lang.Object
    {
        public TextView Code { get; set; }
        public TextView Description { get; set; }
    }
}