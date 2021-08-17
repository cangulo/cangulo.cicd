using cangulo.cicd.abstractons.Models.Enums;

namespace cangulo.cicd.domain.Extensions
{
    public static class CommitTypeExtension
    {
        public static ReleaseType ToReleaseType(this CommitType commitType)
        {
            switch (commitType)
            {
                case CommitType.Fix:
                case CommitType.Patch:
                    return ReleaseType.Patch;
                case CommitType.Feat:
                    return ReleaseType.Minor;
                case CommitType.Major:
                    return ReleaseType.Major;
                default:
                    return ReleaseType.Undefined;
            }
        }
    }
}
