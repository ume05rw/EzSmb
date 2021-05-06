using EzSmb.Params.Enums;
using System;

namespace EzSmb.Params
{
    /// <summary>
    /// Readonly Connection Parameters
    /// </summary>
    public class FixedParamSet
    {
        internal static FixedParamSet Parse(
            ParamSet paramSet,
            SmbType smbType
        )
        {
            if (paramSet == null)
                throw new ArgumentException("Required paramSet.");

            return new FixedParamSet()
            {
                UserName = paramSet.UserName ?? string.Empty,
                Password = paramSet.Password ?? string.Empty,
                DomainName = paramSet.DomainName ?? string.Empty,
                SmbType = smbType
            };
        }

        public string UserName { get; private set; }
        internal string Password { get; private set; }
        public string DomainName { get; private set; }
        public SmbType SmbType { get; private set; }

        private FixedParamSet()
        {
        }
    }
}
