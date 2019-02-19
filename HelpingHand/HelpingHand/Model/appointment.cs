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
    class Appointment
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string User { get; set; }
        public string Address { get; set; }
        public string Eircode { get; set; }
        public string City { get; set; }
        public Color Color { get; set; }
    }
}