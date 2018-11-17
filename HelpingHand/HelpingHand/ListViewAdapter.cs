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
using HelpingHand.Model;

namespace HelpingHand
{
    public class ListViewAdapter : BaseAdapter
    {
        Activity activity;
        List<BabySitter> lstSitters;
        LayoutInflater inflater;

        public ListViewAdapter(Activity activity, List<BabySitter> lstAccounts)
        {
            this.activity = activity;
            this.lstSitters = lstAccounts;
        }
        public override int Count
        {
            get { return lstSitters.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup babySitter)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.list_Item, null);
            var txtuser = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var txtemail = itemView.FindViewById<TextView>(Resource.Id.list_email);
            if (lstSitters.Count > 0)
            {
                txtuser.Text = lstSitters[position].name;
                txtemail.Text = lstSitters[position].email;
            }
            return itemView;
        }
    }
}