using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataModel;
using DataModel.DTO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataModel.Service;

namespace GymSystem.ControllerApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersApi : ControllerBase
    {
        private readonly MemberDetailService _memberService;


        public MembersApi(MemberDetailService memberDetailService)
        {
            _memberService = memberDetailService;
        }

        // 取得會員（角色A）
        [HttpGet("members")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MemberSimpleDTO>>> GetMembers()
        {
            var members = await _memberService.GetMembersByRoleAsync("A");

            return Ok(members);
        }

        // 取得教練（角色B）
        [HttpGet("trainers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MemberSimpleDTO>>> GetTrainers()
        {
            var trainers = await _memberService.GetMembersByRoleAsync("B");

            return Ok(trainers);
        }

        [HttpGet("membername")]
        [Authorize(Policy ="Staff")]
        public async Task<ActionResult<string>> GetMemberName(string memberId)
        {
            var member = await _memberService.GetMemberNameByIdAsync(memberId); 
            return Ok(member);
        }

        [Authorize(Roles = "C")]
        [HttpGet("allmembers")]
        public async Task<ActionResult<List<MemberDTO>>> GetAllMembers(int pageIndex = 1, int pageSize = 10,
            string role = "", int isActive = -1, string sortBy = "MemberID", int order = 0)
        {
            var (members , totalCount) = await _memberService.GetMembersListAsync(pageIndex, pageSize, role, isActive, sortBy, order);

            if(members == null || members.Count == 0)
                return Ok(new List<MemberDTO>());

            return Ok(new {members , totalCount});
        }
    }
}