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
        List<Parent> originalData;
        List<Parent> lstParents;
        LayoutInflater inflater;
        public ParentViewAdapter(Activity activity, List<Parent> lstParents)
        {
            this.activity = activity;
            this.lstParents = lstParents;

            Filter = new ParentFilter(this);
        }
        public override int Count
        {
            get { return lstParents.Count; }
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

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.list_Item, null);

            var name = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var city = itemView.FindViewById<TextView>(Resource.Id.list_city);

            if (lstParents.Count > 0)
            {
                name.Text = lstParents[position].name;
                city.Text = lstParents[position].city;
                //imguser.SetImageDrawable(ImageManager.Get(babySitter.Context, lstSitters[position].ImageUrl));

            }

            return itemView;
        }

        internal class ParentFilter : Filter
        {
            private ParentViewAdapter parentViewAdapter;

            public ParentFilter(ParentViewAdapter adapter)
            {
                parentViewAdapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Parent>();
                if (parentViewAdapter.originalData == null)
                    parentViewAdapter.originalData = parentViewAdapter.lstParents;

                if (constraint == null) return returnObj;

                if (parentViewAdapter.originalData != null && parentViewAdapter.originalData.Any())
                {
                    results.AddRange(
                        parentViewAdapter.originalData.Where(
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
                    parentViewAdapter.lstParents = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<Parent>()).ToList();

                    constraint.Dispose();
                    results.Dispose();
                }
            }
        }
    }
}