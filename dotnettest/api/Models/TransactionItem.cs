using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models 
{
    public class TransactionItem
    {
        public string? PartnerItemRef { get; set; }
        public string? Name { get; set; }
        public int? Qty { get; set; }
        public long? UnitPrice { get; set; }
    }
}