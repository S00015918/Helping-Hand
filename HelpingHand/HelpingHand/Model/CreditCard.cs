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
    public class CreditCard
    {
        public string CardNumber { get; set; }
        public string HolderName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Cvc { get; set; }

        /// <summary>
        /// Initializes a new instance of the CreditCard class.
        /// </summary>
        public CreditCard()
        {
            CardNumber = "";
            Month = "";
            Year = "";
            Cvc = "";
            HolderName = "";
        }

        /// <summary>
        /// Verifies the credit card info. 
        /// However, if the data provided aren't matching an existing card, 
        /// it will still return `true` since that function only checks the basic template of a credit card data.
        /// </summary>
        /// <returns>True if the card data match the basic card information. False otherwise</returns>
        public bool VerifyCreditCardInfo()
        {
            if (CardNumber == ""
                || Month == ""
                || Year == ""
                || Cvc == ""
                || HolderName == "")
                return false;
            try
            {
                int month = 0;
                int year = 0;
                int cvc = 0;

                if (!Int32.TryParse(Month, out month)
                    || !Int32.TryParse(Year, out year)
                    || !Int32.TryParse(Year, out cvc))
                    return false;

                if (month < 1 || month > 12)
                    return false;
                else if (year < 1990 || year > new DateTime().Year)
                    return false;
                else if (Cvc.Length != 3)
                    return false;
            }
            catch (Exception) { return false; }

            return true;
        }
    }
}