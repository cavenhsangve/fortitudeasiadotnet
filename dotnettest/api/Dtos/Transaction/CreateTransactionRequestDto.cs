using api.Models;

namespace api.Dtos.Transaction {
    public class CreateTransactionRequestDto {
        public string PartnerKey { get; set; } = string.Empty;

        public string PartnerRefNo { get; set; } = string.Empty;
        
        public string PartnerPassword { get; set; } = string.Empty;
        public long TotalAmount { get; set; }
        public List<TransactionItem>? Items { get; set; } = new List<TransactionItem>();
        public string TimeStamp { get; set; } = string.Empty;
        public string Sig { get; set; } = string.Empty;
    }
}