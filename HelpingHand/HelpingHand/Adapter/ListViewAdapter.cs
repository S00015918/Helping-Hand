using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Gms.Common.Images;
using Android.Views;
using Android.Widget;
using HelpingHand.Model;
using Java.Lang;
using static Android.Support.V7.Widget.RecyclerView;

namespace HelpingHand
{
    public class ListViewAdapter : BaseAdapter<BabySitter>, IFilterable
    {
        Activity activity;
        List<BabySitter> originalData;
        List<BabySitter> lstSitters;
        LayoutInflater inflater;

        public ListViewAdapter(Activity activity, List<BabySitter> lstAccounts)
        {
            this.activity = activity;
            this.lstSitters = lstAccounts;

            Filter = new BabysitterFilter(this);
        }

        public override BabySitter this[int position]
        {
            get { return lstSitters[position]; }
        }

        public override int Count
        {
            get { return lstSitters.Count; }
        }

        public Filter Filter
        { get; set; }

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
            var view = convertView;

            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.list_Item, null);
            var txtuser = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var txtcity = itemView.FindViewById<TextView>(Resource.Id.list_city);
            var imguser = itemView.FindViewById<ImageView>(Resource.Id.list_img);

            if (lstSitters.Count > 0)
            {
                txtuser.Text = lstSitters[position].name;
                txtcity.Text = lstSitters[position].city;
                //imguser.SetImageDrawable(ImageManager.Get(babySitter.Context, lstSitters[position].ImageUrl));

            }
            return itemView;
        }

        internal class BabysitterFilter : Filter
        {
            private ListViewAdapter listViewAdapter;

            public BabysitterFilter(ListViewAdapter adapter)
            {
                listViewAdapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<BabySitter>();
                if (listViewAdapter.originalData == null)
                    listViewAdapter.originalData = listViewAdapter.lstSitters;

                if (constraint == null) return returnObj; 

                if (listViewAdapter.originalData != null && listViewAdapter.originalData.Any())
                {
                    results.AddRange(
                        listViewAdapter.originalData.Where(
                            user => user.city.Contains(constraint.ToString())));
                }
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                {
                    listViewAdapter.lstSitters = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<BabySitter>()).ToList();

                    constraint.Dispose();
                    results.Dispose();
                }
            }
        }
    }
}