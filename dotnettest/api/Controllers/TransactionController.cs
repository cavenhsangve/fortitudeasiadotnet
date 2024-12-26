using System.Text.Json;
using api.Dtos.Transaction;
using api.Services;
using log4net;
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
        private static readonly ILog logger = LogManager.GetLogger(typeof(TransactionController));

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
                logger.Info($"(SubmitTransaction Post API called) Request: {JsonSerializer.Serialize(request)}");
                if (!_transactionService.ValidateTransaction(request, out errorMessage)) {
                    var errorResponse = CreateTransactionResponseDto.FailureResponse(errorMessage);
                    logger.Error($"Response: {JsonSerializer.Serialize(errorResponse)}");
                    return BadRequest(errorResponse);
                }
                if (!_authenticationService.IsPartnerAllowed(request.PartnerKey, request.PartnerPassword, out errorMessage)) {
                    var errorResponse = CreateTransactionResponseDto.FailureResponse(errorMessage);
                    logger.Error($"Response: {JsonSerializer.Serialize(errorResponse)}");
                    return Unauthorized(errorResponse);
                }
                if(!_authenticationService.IsValidSignature(request)) {
                    var errorResponse = CreateTransactionResponseDto.FailureResponse("Access Denied!");
                    logger.Error($"Response: {JsonSerializer.Serialize(errorResponse)}");
                    return Unauthorized(errorResponse);
                }
                if (request.Items != null && request.Items.Count > 0) {
                    if (!_transactionService.IsValidTotalAmount(request.Items, request.TotalAmount)) {
                        var errorResponse = CreateTransactionResponseDto.FailureResponse("Invalid Total Amount.");
                        logger.Error($"Response: {JsonSerializer.Serialize(errorResponse)}");
                        return BadRequest(errorResponse);
                    }
                }
                if (!_transactionService.IsValidTimestamp(request.TimeStamp, out errorMessage)) {
                    var errorResponse = CreateTransactionResponseDto.FailureResponse(errorMessage);
                    logger.Error($"Response: {JsonSerializer.Serialize(errorResponse)}");
                    return BadRequest(errorResponse);
                }
                (discount, finalAmount) = _transactionService.CalculateDiscount(request.TotalAmount);
                var response = CreateTransactionResponseDto.SuccessResponse(request.TotalAmount, discount, finalAmount);
                logger.Info($"Response: {JsonSerializer.Serialize(response)}");
                return Ok(response);
                }
            catch (Exception ex) 
            {
                logger.Error($"Unexpected error has occurred: {ex.Message}");
                return StatusCode(500, CreateTransactionResponseDto.FailureResponse(ex.Message));
            }
        }
    }
}