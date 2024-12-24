using api.Models;

namespace api.Dtos.Transaction {
    public class CreateTransactionRequestDto {
        public string PartnerKey { get; set; }

        public string PartnerRefNo { get; set; }
        
        public string PartnerPassword { get; set; }
        public long TotalAmount { get; set; }
        public List<TransactionItem>? Items { get; set; } = new List<TransactionItem>();
        public string TimeStamp { get; set; }
        public string Sig { get; set; }
    }
}