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

namespace HelpingHand.Adapter
{
    class CustomGridViewAdapter : BaseAdapter
    {

        Context context;
        private string[] gridViewString;
        private int[] gridViewImage;

        public CustomGridViewAdapter(Context context, string[] gridViewstr, int[] gridViewImg)
        {
            this.context = context;
            gridViewString = gridViewstr;
            this.gridViewImage = gridViewImg;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return 0;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = new View(context);
                view = inflater.Inflate(Resource.Layout.gridview_layout, null);
                TextView txtView = view.FindViewById<TextView>(Resource.Id.textView);
                ImageView imgView = view.FindViewById<ImageView>(Resource.Id.imageView);
                txtView.Text = gridViewString[position];
                imgView.SetImageResource(gridViewImage[position]);
            }
            else
            {
                view = (View)convertView;
            }
            return view;

        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return gridViewString.Length;
            }
        }

    }

    class CustomGridViewAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}