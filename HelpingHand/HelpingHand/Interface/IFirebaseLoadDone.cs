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

namespace HelpingHand.Interface
{
    public interface IFirebaseLoadDone
    {
        void OnFirebaseLoadSuccess(List<Model.BabySitter> babySitters);
        void OnFirebaseLoadFailed(string message);
    }
}