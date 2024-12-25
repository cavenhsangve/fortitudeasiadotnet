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

            // if (!IsValidTimestamp(request.TimeStamp))
            //     return false;

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

        public bool IsValidTimestamp(string timestamp, out string errorMessage) 
        {
            errorMessage = string.Empty;

            if (!DateTime.TryParse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime providedTime))
            {
                errorMessage = "Invalid timestamp format. Must be ISO 8601.";
                return false;
            }

            DateTime serverTime = DateTime.UtcNow;
            Console.WriteLine(serverTime);

            TimeSpan timeDifference = serverTime - providedTime;

            if (Math.Abs(timeDifference.TotalMinutes) > 5) {
                errorMessage = "Expired.";
                return false;
            }
            return true;
        }

        // private bool IsValidTimestamp(string timestamp)
        // {
        //     return DateTimeOffset.TryParseExact(
        //         timestamp,
        //         "yyyy-MM-ddTHH:mm:ss.fffffffZ",
        //         CultureInfo.InvariantCulture,
        //         DateTimeStyles.AssumeUniversal,
        //         out _);
        // }
    }
}