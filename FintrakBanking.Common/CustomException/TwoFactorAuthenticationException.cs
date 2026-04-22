using System;

namespace FintrakBanking.Common.CustomException
{
    [Serializable]
    public class TwoFactorAuthenticationException : SecureException
    {
        public TwoFactorAuthenticationException(string literal) : base(String.Format(literal)) { }
    }
}

<!-- Auto-push timestamp: 2026-04-22 15:02:40 -->