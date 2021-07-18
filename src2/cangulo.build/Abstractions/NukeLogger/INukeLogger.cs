using Logger = Nuke.Common.Logger;

namespace cangulo.build.Abstractions.NukeLogger
{
    public interface INukeLogger
    {
        public void Error(string text);

        public void Info(string text);

        public void Normal(string text);

        public void Success(string text);

        public void Trace(string text);

        public void Warn(string text);
    }

    public class NukeLogger : INukeLogger
    {
        public void Error(string text) => Logger.Error(text);

        public void Info(string text) => Logger.Info(text);

        public void Normal(string text) => Logger.Normal(text);

        public void Success(string text) => Logger.Success(text);

        public void Trace(string text) => Logger.Trace(text);

        public void Warn(string text) => Logger.Warn(text);
    }
}