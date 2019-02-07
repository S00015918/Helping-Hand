using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HelpingHand.Model
{
    public class appointment
    {
        public string appointmentID { get; set; }
        public DateTime appointmentTime;
        public string Uid { get; set; }
    }

    public class Appointment
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public bool IsAllDay { get; set; }
        public Color Color { get; set; }
    }
}