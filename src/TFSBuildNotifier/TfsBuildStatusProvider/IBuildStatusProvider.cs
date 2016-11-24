using System;
using System.Collections.Generic;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    public interface IBuildStatusProvider
    {
        List<BuildStatus> GetStatusList(List<Uri> uriList);
    }
}
