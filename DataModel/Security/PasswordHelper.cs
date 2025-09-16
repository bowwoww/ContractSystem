using Microsoft.AspNetCore.Identity;


namespace DataModel.Security
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> hasher = new();

        public static string Hash(string plainPassword,Member member)
        {
            return hasher.HashPassword(member, plainPassword);
        }

        public static bool Verify(string plainPassword,Member member)
        {
            var result = hasher.VerifyHashedPassword(member, member.MemberPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}