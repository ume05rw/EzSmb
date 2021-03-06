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

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        internal string Password { get; private set; }

        /// <summary>
        /// Windows-Domain name
        /// </summary>
        public string DomainName { get; private set; }

        /// <summary>
        /// Smb2 or Smb1
        /// </summary>
        public SmbType SmbType { get; private set; }

        private FixedParamSet()
        {
        }

        /// <summary>
        /// Create same value instance
        /// </summary>
        /// <returns></returns>
        public FixedParamSet Clone()
        {
            return new FixedParamSet()
            {
                UserName = (this.UserName == null)
                    ? null
                    : string.Copy(this.UserName),
                Password = (this.Password == null)
                    ? null
                    : string.Copy(this.Password),
                DomainName = (this.DomainName == null)
                    ? null
                    : string.Copy(this.DomainName),
                SmbType = this.SmbType
            };
        }
    }
}
