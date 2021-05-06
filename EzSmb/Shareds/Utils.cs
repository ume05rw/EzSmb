using System.Collections.Generic;
using System.Linq;

namespace EzSmb.Shareds
{
    internal static class Utils
    {
        public const char Delimiter = '\\';
        private const char ReplaceDelimiter = '/';

        private static readonly char[] TrimChars
            = new char[] { Utils.Delimiter, Utils.ReplaceDelimiter, ' ' };

        public static string CleanUp(string path)
        {
            return (path ?? string.Empty)
                .Trim(Utils.TrimChars)
                .Replace(Utils.ReplaceDelimiter, Utils.Delimiter);
        }

        public static string Combine(params string[] pathElems)
        {
            var elems = pathElems
                .Select(e => Utils.CleanUp(e))
                .Where(e => !string.IsNullOrEmpty(e) && e != ".")
                .ToArray();

            return Utils.Resolve(string.Join(Utils.Delimiter.ToString(), elems));
        }

        public static string Resolve(string path)
        {
            var elems = Utils.CleanUp(path)
                .Split(Utils.Delimiter)
                .Where(e => !string.IsNullOrEmpty(e) && e != ".")
                .ToArray();

            if (elems.Any(e => e == ".."))
            {
                var list = new List<string>();
                foreach (var elem in elems)
                {
                    if (elem == "..")
                    {
                        if (0 < list.Count)
                            list.RemoveAt(list.Count - 1);
                    }
                    else
                    {
                        list.Add(elem);
                    }
                }

                elems = list.ToArray();
            }

            return string.Join(Utils.Delimiter.ToString(), elems);
        }
    }
}
