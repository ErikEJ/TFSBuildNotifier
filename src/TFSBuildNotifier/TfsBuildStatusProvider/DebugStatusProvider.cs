using System;
using System.Collections.Generic;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    class DebugStatusProvider : IBuildStatusProvider
    {
        public List<BuildStatus> GetStatusList(List<Uri> uriList)
        {
            var statusList = new List<BuildStatus>();

            statusList.Add(new BuildStatus
                {
                   BuildId = 111,
                   BuildName = "Test build",
                   Key = null,
                   Link = null,
                   RequestedBy = "ErikEJ",
                   Status = Status.Undetermined
                }
                );
            return statusList;
        }
    }
}
