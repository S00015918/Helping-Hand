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
using Java.Sql;

namespace HelpingHand.Model
{
    public class Appointment
    {
        public DateTime Date { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string Babysitter { get; set; }
        public string userEmail { get; set; }
        public string babysitterEmail { get; set; }
        public string Address { get; set; }
        public string Eircode { get; set; }
        public string City { get; set; }
        public decimal cost { get; set; }
    }
}