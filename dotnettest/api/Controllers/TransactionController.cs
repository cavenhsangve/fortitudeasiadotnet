using api.Dtos.Transaction;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly TransactionService _transactionService;

        public TransactionController (AuthenticationService authenticationService, TransactionService transactionService)
        {
            _authenticationService = authenticationService;
            _transactionService = transactionService;
        }

        [HttpPost]
        public IActionResult SubmitTransaction([FromBody]CreateTransactionRequestDto request) 
        {
            try {
                string errorMessage = string.Empty;
                long discount, finalAmount;
                if (!_transactionService.ValidateTransaction(request, out errorMessage)) {
                    return BadRequest(CreateTransactionResponseDto.FailureResponse(errorMessage));
                }
                if (!_authenticationService.IsPartnerAllowed(request.PartnerKey, request.PartnerPassword, out errorMessage)) {
                    return Unauthorized(CreateTransactionResponseDto.FailureResponse(errorMessage));
                }
                if (request.Items != null && request.Items.Count > 0) {
                    if (!_transactionService.IsValidTotalAmount(request.Items, request.TotalAmount)) {
                        return BadRequest(CreateTransactionResponseDto.FailureResponse("Invalid Total Amount."));
                    }
                }
                if (!_transactionService.IsValidTimestamp(request.TimeStamp, out errorMessage)) {
                    return BadRequest(CreateTransactionResponseDto.FailureResponse(errorMessage));
                }
                (discount, finalAmount) = _transactionService.CalculateDiscount(request.TotalAmount);
                return Ok(CreateTransactionResponseDto.SuccessResponse(request.TotalAmount, discount, finalAmount));
                }
            catch (Exception ex) 
            {
                return BadRequest(CreateTransactionResponseDto.FailureResponse(ex.Message));
            }
        }
    }
}