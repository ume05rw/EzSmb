using EzSmb.Params.Enums;

namespace EzSmb.Params
{
    /// <summary>
    /// Connection Parameters
    /// </summary>
    public class ParamSet
    {
        /// <summary>
        /// Account Name
        /// </summary>
        public string UserName { get; set; } = null;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = null;

        /// <summary>
        /// Windows Domain Name
        /// </summary>
        /// <remarks>
        /// Set null for using Local Account.
        ///
        /// Warning:
        /// SMB1 with Windows Domain (= Active Directory) is NOT Supoorted.
        /// </remarks>
        public string DomainName { get; set; } = null;

        /// <summary>
        /// SMB Protocol Version
        /// </summary>
        /// <remarks>
        /// If null, SMB2 is tried first, and SMB1 is tried on failure.
        /// </remarks>
        public SmbType? SmbType { get; set; } = null;

        internal ParamSet Clone()
        {
            return new ParamSet()
            {
                UserName = this.UserName,
                Password = this.Password,
                DomainName = this.DomainName,
                SmbType = this.SmbType
            };
        }
    }
}
