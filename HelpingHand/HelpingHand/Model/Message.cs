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
    public class Message
    {
        public string Recipient { get; set; }
        public string BabysitterEmail { get; set; }
        public string ParentEmail { get; set; }
    }
}