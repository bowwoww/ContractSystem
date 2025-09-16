using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DataModel.Service
{
    public class LogService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _dbContext;

        public LogService(IHttpContextAccessor contextAccessor,AppDbContext appDbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = appDbContext;
        }



        public void Log(string? target = null)
        {
            var context = _contextAccessor.HttpContext;
            if (context == null) return;
            var memberId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (memberId == null)
            {
                memberId = target;
                target = null;
            }
            var action = $"{context.Request.Method} {context.Request.Path}";
            var device = context.Request.Headers["User-Agent"].ToString();

            // 取得 X-Forwarded-For 或 RemoteIpAddress
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            // 如果有多個 IP，取第一個
            var log = new OperationLog
            {
                MemberId = memberId,
                Action = action,
                Device = device,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                Target = target
            };
            _dbContext.OperationLogs.Add(log);
            _dbContext.SaveChanges();
        }
    }
}
