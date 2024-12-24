using api.Dtos.Transaction;
using api.Services;
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
            if (!_authenticationService.IsPartnerAllowed(request.PartnerKey, request.PartnerPassword)) {
                return Unauthorized(CreateTransactionResponseDto.FailureResponse("Access Denied!"));
            }

            if (request.Items != null && request.Items.Count > 0) {
                if (!_transactionService.IsValidTotalAmount(request.Items, request.TotalAmount)) {
                    return BadRequest(CreateTransactionResponseDto.FailureResponse("Invalid Total Amount."));
                }
            }

            if (!_transactionService.ValidateTransaction(request)) {
                return BadRequest(CreateTransactionResponseDto.FailureResponse("Bad Request!"));
            }

            return Ok(CreateTransactionResponseDto.SuccessResponse(request.TotalAmount, 0, request.TotalAmount));
        }
    }
}