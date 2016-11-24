using System;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    public class BuildStatus
    {
        public Uri Key { get; set; }
        public Uri Link { get; set; }
        public string BuildName { get; set; }
        public int BuildId { get; set; }
        public string RequestedBy { get; set; }
        public Status Status { get; set; }
    }
}
