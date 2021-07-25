using cangulo.build.abstractions;
using cangulo.build.abstractions.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cangulo.build.domain
{
    public interface ICommitMessageService
    {
        CommitAction GetAction(string msg);

        IEnumerable<CommitAction> GetActions(IEnumerable<string> msgs);
    }

    public class CommitMessageService : ICommitMessageService
    {
        public CommitAction GetAction(string msg)
        {
            if (Constants.CommitActionsVsMsg.Any(x => MatchActionRegex(msg, x)))
                return Constants.CommitActionsVsMsg.FirstOrDefault(x => MatchActionRegex(msg, x)).Key;

            return CommitAction.Undefined;
        }

        private static bool MatchActionRegex(string msg, KeyValuePair<CommitAction, string> x)
            => Regex.IsMatch(msg, x.Value, RegexOptions.IgnoreCase);

        public IEnumerable<CommitAction> GetActions(IEnumerable<string> msgs)
            => msgs.Select(x => GetAction(x));
    }
}