using System.Globalization;
using System.Runtime.ConstrainedExecution;
using api.Dtos.Transaction;
using api.Models;
using api.Utils;
using Microsoft.AspNetCore.Http.Features;

namespace api.Services 
{
    public class TransactionService
    {
        public bool ValidateTransaction(CreateTransactionRequestDto request, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(request.PartnerKey)) {
                errorMessage = "partnerkey is required.";
                return false;
            }
            if (string.IsNullOrEmpty(request.PartnerPassword)) {
                errorMessage = "partnerpassword is required.";
                return false;
            }
            if (string.IsNullOrEmpty(request.PartnerRefNo)) {
                errorMessage = "partnerrefno is required.";
                return false;
            }
            if (string.IsNullOrEmpty(request.TimeStamp)) {
                errorMessage = "timestamp is required.";
                return false;
            }
            if (string.IsNullOrEmpty(request.Sig)) {
                errorMessage = "sig is required.";
                return false;
            }

            if (request.TotalAmount <= 0) {
                errorMessage = "Must be positive value.";
                return false;
            }

            if (request.Items != null)
            {
                foreach (var item in request.Items)
                {
                    if (string.IsNullOrEmpty(item.PartnerItemRef)) {
                        errorMessage = "partneritemref is required.";
                        return false;
                    }

                    if (string.IsNullOrEmpty(item.Name)){
                        errorMessage = "Item name is required.";
                        return false;
                    }

                    if (item.Qty < 1 || item.Qty > 5 || item.UnitPrice <= 0) {
                        errorMessage = "Item Quantity must be more than 1 or less than 6.";
                        return false;
                    }
                }
            }
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

        public (long,long) CalculateDiscount(long totalAmount) 
        {
            long discount = 0, finalAmount=totalAmount;
            // Calculate base
            if (totalAmount < 20000) {
                discount = 0;
            }
            else if (totalAmount >= 20000 && totalAmount <= 50099)
            {
                discount = 5;
            } 
            else if (totalAmount > 50099 && totalAmount <= 80099) 
            {
                discount = 7;
            } 
            else if (totalAmount > 80099 && totalAmount <= 120099)
            {
                discount = 10;
            }
            else
            {
                discount = 15;
            }

            // Calculate conditional
            if (totalAmount > 50000 && PrimeUtil.IsPrime((int)totalAmount)) {
                discount += 8;
            }
            if (totalAmount > 90000 && GetEffectiveLastDigit(totalAmount) == 5) {
                discount += 10;
            }

            // Calculate discount exceeds 20%
            if (discount > 20)
                discount = 20;

            if (discount > 0) {
                finalAmount = (long)(totalAmount * ((double)(100-discount)/100));
            }
            return (discount, finalAmount);
        }

        private long GetEffectiveLastDigit(long number) {
            while (number > 0) 
            {
                long lastDigit = number % 10;
                if (lastDigit != 0) 
                    return lastDigit;
                number /= 10;
            }
            return 0;
        }
    }
}