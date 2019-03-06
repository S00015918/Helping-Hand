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
    internal class MessageAdapter : BaseAdapter
    {
        private MessageActivity messageActivity;
        private List<MessageContent> lstMessage;

        public MessageAdapter(MessageActivity messageActivity, List<MessageContent> lstMessage)
        {
            this.messageActivity = messageActivity;
            this.lstMessage = lstMessage;
        }

        public override int Count
        {
            get { return lstMessage.Count; }
        }

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
            LayoutInflater inflater = (LayoutInflater)messageActivity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View ItemView = inflater.Inflate(Resource.Layout.message_list, null);
            TextView message_user, message_time, message_content;
            message_user = ItemView.FindViewById<TextView>(Resource.Id.message_user);
            message_time = ItemView.FindViewById<TextView>(Resource.Id.message_time);
            message_content = ItemView.FindViewById<TextView>(Resource.Id.message_text);

            message_user.Text = lstMessage[position].Email;
            message_time.Text = lstMessage[position].Time;
            message_content.Text = lstMessage[position].Message;

            return ItemView;
        }
    }
}