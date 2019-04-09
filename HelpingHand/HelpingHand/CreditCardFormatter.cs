using System;
using Android.Text;
using Android.Widget;
using Java.Lang;

namespace HelpingHand
{
    internal class CreditCardFormatter : Java.Lang.Object, ITextWatcher
    {
        private EditText creditCardNumber;
        private bool _lock;

        public CreditCardFormatter(EditText creditCardNumber)
        {
            this.creditCardNumber = creditCardNumber;
        }

        //public IntPtr Handle => throw new NotImplementedException();

        public void AfterTextChanged(IEditable s)
        {
            if (_lock || s.Length() > 18)
                {
                    return;
                }
            _lock = true;

            if (s.Length() == 4 || s.Length() == 9 || s.Length() == 14)
                { s.Append(" "); };

            _lock = false;
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public new void Dispose()
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }
    }
}