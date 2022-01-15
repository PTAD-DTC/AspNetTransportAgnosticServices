using System;
using JetBrains.Annotations;

namespace Service.Interface.Base
{
    [PublicAPI]
    public abstract class ModelBase
    {
#if DEBUG
        private static readonly System.Text.RegularExpressions.Regex VersionRegex = new System.Text.RegularExpressions.Regex(@"\.[vV]\d+(_\d+)*(\w+|-\w+)", System.Text.RegularExpressions.RegexOptions.Compiled);

        protected ModelBase()
        {
            DebugType = GetType().FullName;
            if (DebugType is null)
            {
                return;
            }
            var match = VersionRegex.Match(DebugType);
            if (match.Success)
            {
                ApiVersion = match.Value.Replace(".", string.Empty).Replace('_', '.');
            }
        }

        public string? ApiVersion { get; set; }
        public string? DebugType { get; set; }
#endif
    }
}
