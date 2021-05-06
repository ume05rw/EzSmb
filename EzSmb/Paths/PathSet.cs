using EzSmb.Shareds;
using System;
using System.Linq;
using System.Net;

namespace EzSmb.Paths
{
    /// <summary>
    /// Path strings and IPAddress
    /// </summary>
    public class PathSet
    {
        internal static PathSet Parse(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("Required fullPath.");

            var resolved = Utils.Resolve(fullPath);

            if (string.IsNullOrEmpty(resolved))
                throw new ArgumentException($"Invalid Path: {fullPath}");

            var result = new PathSet();

            var elems = resolved.Split(Utils.Delimiter);

            try
            {
                result.IpAddress = IPAddress.Parse(elems[0]);
            }
            catch (Exception)
            {
                throw new ArgumentException($"First Section Requires IP-Address Format: {elems[0]}");
            }

            result.IpAddressString = elems[0];

            if (2 <= elems.Length)
                result.Share = elems[1];

            if (3 <= elems.Length)
            {
                result.Elements = elems
                    .Skip(2)
                    .Take(elems.Length - 2)
                    .ToArray();

                result.ElementsPath = string.Join(@"\", result.Elements);
            }

            result.FullPath = resolved;

            return result;
        }

        /// <summary>
        /// IP-Address string
        /// </summary>
        public string IpAddressString { get; private set; }

        /// <summary>
        /// IP-Address object
        /// </summary>
        public IPAddress IpAddress { get; private set; }

        /// <summary>
        /// Share Name
        /// </summary>
        public string Share { get; private set; } = null;

        /// <summary>
        /// Sub-Path elements on Share
        /// </summary>
        public string[] Elements { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// Sub-Path string on Share
        /// </summary>
        public string ElementsPath { get; private set; } = null;

        /// <summary>
        /// FullPath
        /// </summary>
        public string FullPath { get; private set; }

        private PathSet()
        {
        }
    }
}
