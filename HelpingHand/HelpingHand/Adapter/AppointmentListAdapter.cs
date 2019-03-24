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
    public class AppointmentListAdapter : BaseAdapter
    {
        Activity activity;
        List<Appointment> originalData;
        List<Appointment> lstAppointments;
        LayoutInflater inflater;

        public AppointmentListAdapter(Activity activity, List<Appointment> appointments)
        {
            this.activity = activity;
            this.lstAppointments = appointments;

            Filter = new AppointmentFilter(this);
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
            View view = inflater.Inflate(Resource.Layout.list_appointments, null);

            var babysitter = view.FindViewById<TextView>(Resource.Id.list_city);
            //var date = view.FindViewById<TextView>(Resource.Id.list_date);
            //var start = view.FindViewById<TextView>(Resource.Id.list_start);
            //var end = view.FindViewById<TextView>(Resource.Id.list_end);

            if (lstAppointments.Count > 0)
            {
                babysitter.Text = lstAppointments[position].City;                
            }

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get { return lstAppointments.Count; }
        }

        internal class AppointmentFilter : Filter
        {
            private AppointmentListAdapter appointmentAdapter;

            public AppointmentFilter(AppointmentListAdapter adapter)
            {
                appointmentAdapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Appointment>();
                if (appointmentAdapter.originalData == null)
                    appointmentAdapter.originalData = appointmentAdapter.lstAppointments;
                if (constraint == null) return returnObj;

                if (appointmentAdapter.originalData != null && appointmentAdapter.originalData.Any())
                {
                    results.AddRange(
                        appointmentAdapter.originalData.Where(
                            user => user.City.Contains(constraint.ToString())));
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
                    appointmentAdapter.lstAppointments = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<Appointment>()).ToList();
                    constraint.Dispose();
                    results.Dispose();
                }
            }
        }
    }

    class AppointmentListAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}