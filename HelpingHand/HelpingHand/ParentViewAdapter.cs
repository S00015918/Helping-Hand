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
using Java.Lang;

namespace HelpingHand
{
    public class ParentViewAdapter : BaseAdapter
    {
        Activity activity;
        List<Parent> lstParents;
        LayoutInflater inflater;
        public ParentViewAdapter(Activity activity, List<Parent> lstParents)
        {
            this.activity = activity;
            this.lstParents = lstParents;
        }
        public override int Count
        {
            get { return lstParents.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.profilePage, null);
            var name = itemView.FindViewById<EditText>(Resource.Id.name);
            var email = itemView.FindViewById<EditText>(Resource.Id.email);
            //var password = itemView.FindViewById<TextView>(Resource.Id.password);
            var phone = itemView.FindViewById<EditText>(Resource.Id.phone);
            var eircode = itemView.FindViewById<EditText>(Resource.Id.eircode);
            var address = itemView.FindViewById<EditText>(Resource.Id.address);
            var city = itemView.FindViewById<EditText>(Resource.Id.city);

            return itemView;
        }
    }
}