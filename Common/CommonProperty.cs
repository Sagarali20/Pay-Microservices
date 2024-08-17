
namespace Common
{
    public class CommonProperty
    {
        public int IdEnvKey { get; set; }
        public int IdUserModKey { get; set; }
        public int IdFsmActionKey { get; set; }
        public int IdFsmStateKey { get; set; }
        public DateTime DttMod { get; set; }
        public string TxDescription { get; set; } = string.Empty;
        public int IsActive { get; set; }
    }
}
