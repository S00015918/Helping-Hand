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
    class FavouriteBabysitterAdapter : BaseAdapter
    {
        Activity activity;
        public List<Rating> originalData;
        public List<Rating> lstRatings;
        LayoutInflater inflater;

        public FavouriteBabysitterAdapter(Activity activity, List<Rating> lst_ratings)
        {
            this.activity = activity;
            this.lstRatings = lst_ratings;

            Filter = new FavouriteBabysitterFilter(this);
        }

        public override int Count
        {
            get { return lstRatings.Count; }
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
            View itemView = inflater.Inflate(Resource.Layout.list_favourites, null);

            var name = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var rating = itemView.FindViewById<TextView>(Resource.Id.list_rating);

            if (lstRatings.Count > 0)
            {
                name.Text = lstRatings[position].ratedUsersName;
                rating.Text = lstRatings[position].rating.ToString() + " Stars";
            }

            return itemView;
        }
    }
    internal class FavouriteBabysitterFilter : Filter
    {
        private FavouriteBabysitterAdapter favouriteAdapter;

        public FavouriteBabysitterFilter(FavouriteBabysitterAdapter adapter)
        {
            favouriteAdapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnObj = new FilterResults();
            var results = new List<Rating>();
            if (favouriteAdapter.originalData == null)
                favouriteAdapter.originalData = favouriteAdapter.lstRatings;

            if (constraint == null) return returnObj;

            if (favouriteAdapter.originalData != null && favouriteAdapter.originalData.Any())
            {
                results.AddRange(
                    favouriteAdapter.originalData.Where(
                        user => user.ratedUsersName.Contains(constraint.ToString())));
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
                favouriteAdapter.lstRatings = values.ToArray<Java.Lang.Object>()
                    .Select(r => r.ToNetObject<Rating>()).ToList();

                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}