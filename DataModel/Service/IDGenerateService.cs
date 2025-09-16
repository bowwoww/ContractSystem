using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Service
{
    public class IDGenerateService
    {
        private readonly AppDbContext _context;

        public IDGenerateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNewContractID()
        {
            // 假設分店代號為 "A"，可依需求調整
            string branchCode = "A";
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");

            // 查詢本年月分店的最大流水號
            string prefix = $"{branchCode}{year}{month}";
            var lastContract = await _context.Contracts
                .Where(c => c.ContractID.StartsWith(prefix))
                .OrderByDescending(c => c.ContractID)
                .FirstOrDefaultAsync();

            int serial = 1;
            if (lastContract != null)
            {
                // 取出最後兩碼流水號並+1
                string lastSerialStr = lastContract.ContractID.Substring(7, 2);
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    serial = lastSerial + 1;
                }
            }

            string newContractID = $"{branchCode}{year}{month}{serial.ToString("D2")}";

            return newContractID;
        }

        public async Task<string> GenerateNewTrainingDateID(DateTime trainingDate)
        {
            // 產生 TrainingDateID: 4碼年 + 4碼月日 + 4碼流水號
            string year = trainingDate.Year.ToString("D4");
            string monthDay = trainingDate.ToString("MMdd");
            string prefix = $"{year}{monthDay}";

            // 查詢該日期的最大流水號
            var lastDate = await _context.TrainingDates
                .Where(td => td.TrainingDateID.ToString().StartsWith(prefix))
                .OrderByDescending(td => td.TrainingDateID)
                .FirstOrDefaultAsync();

            int serial = 1;
            if (lastDate != null)
            {
                string lastSerialStr = lastDate.TrainingDateID.ToString().Substring(8, 4);
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    serial = lastSerial + 1;
                }
            }
            string trainingDateId = $"{prefix}{serial.ToString("D4")}";

            return trainingDateId;
        }
    }
}
