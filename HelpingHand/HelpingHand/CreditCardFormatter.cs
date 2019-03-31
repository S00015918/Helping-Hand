using System;
using Android.Text;
using Android.Widget;
using Java.Lang;

namespace HelpingHand
{
    internal class CreditCardFormatter : ITextWatcher
    {
        private EditText creditCardNumber;
        private bool _lock;

        public CreditCardFormatter(EditText creditCardNumber)
        {
            this.creditCardNumber = creditCardNumber;
        }

        public IntPtr Handle => throw new NotImplementedException();

        public void AfterTextChanged(IEditable s)
        {
            if (_lock || s.Length() > 16)
                {
                    return;
                }
            _lock = true;

            for (int i = 4; i < s.Length(); i += 5)
            {
                if (s.ToString().IndexOf(i.ToString()) != ' ')
                {
                    s.Insert(i, " ");
                }
            }
            _lock = false;
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            throw new NotImplementedException();
        }
    }
}