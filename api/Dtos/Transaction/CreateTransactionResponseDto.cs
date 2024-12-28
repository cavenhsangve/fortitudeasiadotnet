using api.Dtos.Transaction;

namespace api.Dtos.Transaction {
    public class CreateTransactionResponseDto {
        public int Result { get; set; }
        public long? TotalAmount { get; set; }
        public long? TotalDiscount { get; set; }
        public long? FinalAmount { get; set; }
        public string? ResultMessage { get; set; }

        public static CreateTransactionResponseDto FailureResponse(string message) 
        {
            return new CreateTransactionResponseDto 
            { 
                Result = 0,
                ResultMessage = message 
            };
        }

        public static CreateTransactionResponseDto SuccessResponse(long totalAmount, 
        long totalDiscount, long finalAmount)
        {
            return new CreateTransactionResponseDto
            {
                Result = 1,
                TotalAmount = totalAmount,
                TotalDiscount = totalDiscount,
                FinalAmount = finalAmount
            };
        }
    }
}


