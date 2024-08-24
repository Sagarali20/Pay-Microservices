namespace AuthenticationService.Helpers
{
    public static class Extentions
    {

        public static string GeneratenWalletAccount(this int InvoiceId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("WAL-{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GeneratenMarcentAccount(this int InvoiceId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("MAR-{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GeneratenEscAccount(this int InvoiceId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("ESC-{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }

    }
}
