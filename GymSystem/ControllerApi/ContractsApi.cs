using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataModel.Service;
using DataModel.DTO;

namespace GymSystem.ControllerApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsApi : ControllerBase
    {
        private readonly ContractDetailService _contractDetailService;


        public ContractsApi(ContractDetailService contractDetailService)
        {
            _contractDetailService = contractDetailService;
        }

        // 取得指定會員的第一筆合約ID
        [HttpGet("firstid")]
        [Authorize]
        public async Task<ActionResult<string>> GetFirstContractId(string memberId)
        {
            var contractId = await _contractDetailService.GetFirstContractIdByMemberId(memberId);
            if (string.IsNullOrEmpty(contractId))
                return NotFound();
            return Ok(contractId);
        }

        [HttpGet("contracts")]
        [Authorize]

        public async Task<ActionResult<List<ContractDTO>>> GetContracts(string memberId, int pageIndex = 1,
                    int pageSize = 10,string trainerId = "",int graduate=1)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            string tempMemberId = memberId;

            if (currentUserId != memberId && currentUserRole != "B" && currentUserRole != "C")
            {
                return Ok(new List<ContractDTO>());
            }
            if(currentUserRole == "B" || currentUserRole == "C")
            {
                tempMemberId = "0";
            }

            var (contracts, totalCount) = await _contractDetailService.GetContractsPagedAsync(pageIndex, pageSize, tempMemberId,trainerId,graduate);
            if (contracts == null || contracts.Count == 0)
                return Ok(new List<ContractDTO>());
            return Ok(new { contracts, totalCount });

        }

        [HttpGet("contracts/{memberId}")]
        [Authorize]
        public async Task<List<ContractSimpleDTO>> GetAllActiveContractIds(string memberId)
        {
            return await _contractDetailService.GetActiveContractInfosByMemberId(memberId);
        }
    }
}