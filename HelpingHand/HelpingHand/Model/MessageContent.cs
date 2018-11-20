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
        public string Email { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public MessageContent() { }
        public MessageContent(string Email, string Message)
        {
            this.Email = Email;
            this.Message = Message;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}