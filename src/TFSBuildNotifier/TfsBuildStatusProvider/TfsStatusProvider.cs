using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    class TfsStatusProvider : IBuildStatusProvider
    {
        public List<BuildStatus> GetStatusList(List<Uri> uriList)
        {
            var result = new List<BuildStatus>();
            foreach (var uri in uriList)
            {
                var buildStatus = new BuildStatus
                {
                    Key = uri,
                    BuildId = 0,
                    Status = Status.Undetermined,
                    BuildName = "N/A",
                    RequestedBy = "Unknown"
                };
                try
                {
                    var body = GetJsonPayload(uri);
                    var response = JsonConvert.DeserializeObject<Rootobject>(body);

                    buildStatus.BuildId = response.value[0].id;
                    buildStatus.Status = Status.Success;
                    buildStatus.BuildName = response.value[0].definition.name;
                    buildStatus.RequestedBy = response.value[0].requestedBy.displayName;
                    buildStatus.Link = new Uri(response.value[0]._links.web.href);
                    buildStatus.BuildDateTime = response.value[0].finishTime.ToLocalTime();
                    var startTime = response.value[0].startTime.ToLocalTime();
                    buildStatus.BuildTime = (buildStatus.BuildDateTime- startTime).TotalSeconds;
                    var status = response.value[0].result;                    
                    if (status != "succeeded")
                    {
                        buildStatus.Status = Status.Error;
                    }
                    //TODO Do more granular status?
                    //https://www.visualstudio.com/en-us/docs/integrate/api/build/builds
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()), ex.ToString());
                }
                result.Add(buildStatus);
            }
            return result;
        }

        private string GetJsonPayload(Uri url)
        {
            var client = new HttpClient(new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultNetworkCredentials,
                UseDefaultCredentials = true
            });
            client.Timeout = new TimeSpan(0,0, 10);
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
