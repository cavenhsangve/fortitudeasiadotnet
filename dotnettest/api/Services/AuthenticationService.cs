using System.Buffers.Text;
using api.Dtos.Transaction;
using api.Utils;

namespace api.Services
{
    public class AuthenticationService
    {
        private readonly Dictionary<string, string> _allowedPartners = new()
        {
            { "FAKEGOOGLE", "FAKEPASSWORD1234" },
            { "FAKEPEOPLE", "FAKEPASSWORD4578" },
        };

        public bool IsPartnerAllowed(string partnerKey, string partnerPassword)
        {
            return _allowedPartners.GetValueOrDefault(partnerKey)?.Equals(Base64Encoder.Decode(partnerPassword)) ?? false;
        }

        // Acutal signature does not match expected signature
        // Step 1: Concatenate request body
        // Step 2: Compute Hash of concatenated string
        // Step 3: Encode Hash
        public bool IsValidSignature(CreateTransactionRequestDto request)
        {
            if (!DateTimeOffset.TryParse(request.TimeStamp, out DateTimeOffset parsedTimeStamp)) {
                throw new FormatException("Invalid ISO 8601 timestamp format");
            }

            string concatenated = $"{parsedTimeStamp:yyyyMMddHHmmss}{request.PartnerKey}{request.PartnerRefNo}{request.TotalAmount}{request.PartnerPassword}";
            string calculatedSig = HashUtil.ComputeSHA256(concatenated);
            Console.WriteLine(calculatedSig);
            Console.WriteLine(concatenated);

            return calculatedSig.Equals(request.Sig);
        }
    }
}