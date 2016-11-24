using System;
using System.Collections.Generic;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    class DebugStatusProvider : IBuildStatusProvider
    {
        public List<BuildStatus> GetStatusList(List<Uri> uriList)
        {
            var statusList = new List<BuildStatus>();

            foreach (var uri in uriList)
            {
                statusList.Add(new BuildStatus
                {
                    BuildId = 111 + DateTime.Now.Millisecond,
                    BuildName = "Test build",
                    Key = uri,
                    Link = uri,
                    RequestedBy = "ErikEJ",
                    Status = Status.Error
                }
                    );
            }
            return statusList;
        }
    }
}
