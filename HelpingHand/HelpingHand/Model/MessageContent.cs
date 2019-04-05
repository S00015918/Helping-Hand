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
    internal class MessageContent
    {
        public string Recipient { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
    }
}