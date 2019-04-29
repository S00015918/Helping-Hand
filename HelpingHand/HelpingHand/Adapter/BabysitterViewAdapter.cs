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

namespace HelpingHand.Adapter
{
    class BabysitterViewAdapter : BaseAdapter
    {

        Activity activity;
        public List<BabySitter> originalData;
        public List<BabySitter> lstBabysitters;
        LayoutInflater inflater;

        public BabysitterViewAdapter(Activity activity, List<BabySitter> lstbabySitters)
        {
            this.activity = activity;
            this.lstBabysitters = lstbabySitters;

            Filter = new BabysitterFilter(this);
        }

        public override int Count
        {
            get { return lstBabysitters.Count; }
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

        public override View GetView(int position, View convertView, ViewGroup babysitter)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.list_Item, null);

            var name = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var city = itemView.FindViewById<TextView>(Resource.Id.list_city);

            if (lstBabysitters.Count > 0)
            {
                name.Text = lstBabysitters[position].name;
                city.Text = lstBabysitters[position].city;
                //imguser.SetImageDrawable(ImageManager.Get(babySitter.Context, lstSitters[position].ImageUrl));
            }

            return itemView;
        }
    }

    internal class BabysitterFilter : Filter
    {
        private BabysitterViewAdapter babysitterViewAdapter;

        public BabysitterFilter(BabysitterViewAdapter adapter)
        {
            babysitterViewAdapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<BabySitter>();
            if (babysitterViewAdapter.originalData == null)
                babysitterViewAdapter.originalData = babysitterViewAdapter.lstBabysitters;

            if (constraint == null) return returnObj;

            if (babysitterViewAdapter.originalData != null && babysitterViewAdapter.originalData.Any())
            {
                results.AddRange(
                    babysitterViewAdapter.originalData.Where(
                        user => user.city.ToLower().Contains(constraint.ToString())));
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
                babysitterViewAdapter.lstBabysitters = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<BabySitter>()).ToList();

                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}