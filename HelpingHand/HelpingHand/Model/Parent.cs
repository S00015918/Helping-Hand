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
    public class Parent
    {
        public string id { get; set; }
        public string name { get; set; }
        //public string surname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string eircode { get; set; }
        public int noOfKids { get; set; }
        public string ImageUrl { get; set; }
    }
}