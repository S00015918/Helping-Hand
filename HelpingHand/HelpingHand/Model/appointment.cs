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

namespace HelpingHand.Model
{
    public class appointment
    {
        public string appointmentID { get; set; }
        public DateTime appointmentTime;
        public string Uid { get; set; }
    }
}