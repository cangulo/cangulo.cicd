using cangulo.Build;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using System.Collections.Generic;

namespace cangulo.build.Abstractions.Models
{
    public class BuildContext
    {
        public AbsolutePath RootDirectory { get; set; }
        public IEnumerable<Solution> Solutions { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public Configuration Configuration { get; set; }
    }
}