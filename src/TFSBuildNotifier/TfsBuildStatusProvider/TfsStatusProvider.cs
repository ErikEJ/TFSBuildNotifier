using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace TFSBuildNotifier.TfsBuildStatusProvider
{
    class TfsStatusProvider : IBuildStatusProvider
    {
        public List<BuildStatus> GetStatusList(List<Uri> uriList)
        {
            var result = new List<BuildStatus>();
            foreach (var uri in uriList)
            {
                var buildStatus = new BuildStatus();
                buildStatus.Key = uri;
                buildStatus.BuildId = 0;
                buildStatus.Status = Status.Undetermined;
                buildStatus.BuildName = "N/A";
                buildStatus.RequestedBy = "Unknown";
                try
                {
                    var body = GetJsonPayloadAsync(uri).GetAwaiter().GetResult();
                    var serializer = new DataContractJsonSerializer(typeof(Rootobject));
                    using (var stream = GenerateStreamFromString(body))
                    {
                        var response = (Rootobject)serializer.ReadObject(stream);
                        buildStatus.Link = new Uri(response.value[0]._links.web.href);
                        buildStatus.BuildName = response.value[0].definition.name;
                        buildStatus.RequestedBy = response.value[0].requestedBy.displayName;
                        var status = response.value[0].result;
                        buildStatus.Status = Status.Success;
                        if (status != "succeeded")
                        {
                            buildStatus.Status = Status.Error;
                        }
                        //TODO Do more granular status?
                        //https://www.visualstudio.com/en-us/docs/integrate/api/build/builds
                    }
                }
                catch
                {
                    //Ignored                    
                }
                result.Add(buildStatus);
            }
            return result;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async Task<string> GetJsonPayloadAsync(Uri url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
