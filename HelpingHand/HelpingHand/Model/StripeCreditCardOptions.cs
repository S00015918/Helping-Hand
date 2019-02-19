namespace HelpingHand
{
    internal class StripeCreditCardOptions
    {
        public StripeCreditCardOptions()
        {
        }

        public string Number { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string Cvc { get; set; }
    }
}