using System;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContractEditTypeRequiredValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var contractEdit = value as ContractEdit;
            var db = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (contractEdit == null)
                return ValidationResult.Success;

            switch (contractEdit.EditType)
            {
                case EditType.延展結束日期:
                    if (!contractEdit.NewEndDate.HasValue)
                        return new ValidationResult("展延結束時間必填。", new[] { nameof(contractEdit.NewEndDate) });
                    if (db != null)
                    {
                        var endDate = db.Contracts
                            .Where(c => c.ContractID == contractEdit.ContractID)
                            .Select(c => c.EndDate)
                            .FirstOrDefault();
                        if (contractEdit.NewEndDate <= endDate)
                            return new ValidationResult("展延結束時間必須大於目前合約結束時間。", new[] { nameof(contractEdit.NewEndDate) });
                    }
                    break;
                case EditType.增加課堂數:
                    if (!contractEdit.AddClassCount.HasValue || contractEdit.AddClassCount.Value <= 0)
                        return new ValidationResult("增加課堂數必填且必須大於 0。", new[] { nameof(contractEdit.AddClassCount) });
                    break;
                case EditType.轉讓課堂:
                    if (string.IsNullOrWhiteSpace(contractEdit.TransferToMemberID))
                        return new ValidationResult("轉讓課堂對象必填。", new[] { nameof(contractEdit.TransferToMemberID) });
                    if (!contractEdit.TransferClassCount.HasValue || contractEdit.TransferClassCount.Value <= 0)
                        return new ValidationResult("轉讓課堂數必填且必須大於 0。", new[] { nameof(contractEdit.TransferClassCount) });
                    
                    if (db != null)
                    {
                        var contractMemberId = db.Contracts
                            .Where(c => c.ContractID == contractEdit.ContractID)
                            .Select(c => c.MemberID)
                            .FirstOrDefault();

                        if (contractEdit.TransferToMemberID == contractMemberId)
                            return new ValidationResult("轉讓課堂對象不能是自己。", new[] { nameof(contractEdit.TransferToMemberID) });
                    }
                    break;
            }
            return ValidationResult.Success;
        }
    }
}
