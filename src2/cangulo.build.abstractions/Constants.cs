using cangulo.build.abstractions.Models.Enums;
using System.Collections.Generic;

namespace cangulo.build.abstractions
{
    public static class Constants
    {
        public static class RegexConstants
        {
            public const string REGEX_NUGET_VERSION = @"([\d]{1,2})\.([\d]{1,2})\.([\d]{1,2})\.([\d]{1,2})";
        }

        public static class CSProjProperties
        {
            public const string VERSION_PREFIX = "VersionPrefix";
            public const string IS_PACKABLE = "IsPackable";
        }

        public static IDictionary<CommitAction, string> CommitActionsVsMsg =
            new Dictionary<CommitAction, string>
            {
                {CommitAction.CreatePatch, @"\[ci\][\s]+create[\s]+patch" },
                {CommitAction.CreateMinor, @"\[ci\][\s]+create[\s]+minor" },
                {CommitAction.CreateMajor, @"\[ci\][\s]+create[\s]+major" },
            };
    }
}