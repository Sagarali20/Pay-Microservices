namespace MailService.Constants
{
    /**
     * Static Constants for Common Constants
     * 
     * @author: Masud Ahmed
     * @since: Aug 2024
     **/
    public static class MailConstants
    {
        static MailConstants() { }
        // Constants
        // DB Schema
        public const string dbSchema = "dbo.";
        // Stored Procedure names

        // Actions Names

        // TXN STATUS

        // Response Code and Messages
        

        public static string GetMimeType(String extension) { 
            Dictionary<String,String> mimeTypes = new()
            {
                ["jpg"] = "image/jpeg",
                ["png"] = "image/png",
                ["jpeg"] = "image/jpeg",
                ["pdf"] = "application/pdf"
            };

            return mimeTypes[extension];
        }
        
    }
}
