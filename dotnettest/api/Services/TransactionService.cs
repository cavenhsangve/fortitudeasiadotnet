using System.Globalization;
using api.Dtos.Transaction;
using api.Models;
using api.Utils;
using Microsoft.AspNetCore.Http.Features;

namespace api.Services 
{
    public class TransactionService
    {
        public bool ValidateTransaction(CreateTransactionRequestDto request)
        {
            if (string.IsNullOrEmpty(request.PartnerKey) 
            || string.IsNullOrEmpty(request.PartnerPassword)
            || string.IsNullOrEmpty(request.PartnerRefNo)
            || string.IsNullOrEmpty(request.TimeStamp)
            || string.IsNullOrEmpty(request.Sig)) 
            {
                return false;
            }

            if (request.TotalAmount <= 0)
                return false;

            if (request.Items != null)
            {
                foreach (var item in request.Items)
                {
                    if (string.IsNullOrEmpty(item.PartnerItemRef) || string.IsNullOrEmpty(item.Name))
                        return false;

                    if (item.Qty < 1 || item.Qty > 5 || item.UnitPrice <= 0)
                        return false;
                }
            }

            if (!IsValidTimestamp(request.TimeStamp))
                return false;

            return true;
        }

        public bool IsValidTotalAmount(List<TransactionItem> transactionItems, long totalAmount) {
            long itemAmount = 0;
            foreach (var item in transactionItems)
            {
                if (item.Qty.HasValue && item.UnitPrice.HasValue)
                {
                    itemAmount += item.Qty.Value * item.UnitPrice.Value;
                }
            }
            
            return itemAmount == totalAmount;
        }

        private bool IsValidTimestamp(string timestamp)
        {
            if (!timestamp.EndsWith("Z"))
                return false;

            return DateTimeOffset.TryParseExact(
                timestamp,
                "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out _);
        }
    }
}