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
using Firebase.Auth;
using Firebase.Xamarin.Database;
using HelpingHand.Model;
using Java.Lang;

namespace HelpingHand
{
    class BabysitterApointmentAdapter : BaseAdapter
    {
        Activity activity;
        List<Appointment> lstAppointments;
        LayoutInflater inflater;

        public BabysitterApointmentAdapter(Activity activity, List<Appointment> appointments)
        {
            this.activity = activity;
            this.lstAppointments = appointments;

            //Filter = new AppointmentFilter(this);
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
            View view = inflater.Inflate(Resource.Layout.list_appointments, null);

            var name = view.FindViewById<TextView>(Resource.Id.list_name);
            var dateList = view.FindViewById<TextView>(Resource.Id.list_date);
            var start = view.FindViewById<TextView>(Resource.Id.list_start);
            var end = view.FindViewById<TextView>(Resource.Id.list_end);

            if (lstAppointments.Count > 0)
            {
                name.Text = lstAppointments[position].Address;
                var datetime = lstAppointments[position].Date.ToString().Split(' ');
                string date = datetime[0];
                dateList.Text = date.ToString();
                start.Text = "Start: " + lstAppointments[position].startTime.ToString();
                end.Text = "End: " + lstAppointments[position].endTime.ToString();
            }
            return view;
        }

        public override int Count
        {
            get { return lstAppointments.Count; }
        }
    }

    class BabysitterApointmentAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}