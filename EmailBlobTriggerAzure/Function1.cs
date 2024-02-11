using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EmailBlobTriggerAzure
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("reenbittaskcontainer/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=reenbittasksa;AccountKey=pcASRy2bc00Y7NRWj4w4w2koAzKzvjbrTGFCcS6qi/DaSpX128Y99RTjyDffmOvmNis9gNMNqoxy+AStVoU8Wg==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
