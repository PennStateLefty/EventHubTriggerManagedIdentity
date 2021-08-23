using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubTriggerManagedIdentity
{
    public static class PeriodicSend
    {
        [FunctionName("PeriodicSend")]
        public static async Task Run([TimerTrigger("*/2 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
			{
                //var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = "16dde819-2b05-4fe7-b92b-6dcf0b8a1a7f" });
                
                //Create a new producer client - leverage DefaultAzureCredentials to allowed Managed Identity to handle authentication and authorization
                await using (var producer = new EventHubProducerClient(fullyQualifiedNamespace: Environment.GetEnvironmentVariable("eventHubConnection__fullyQualifiedNamespace"), eventHubName: Environment.GetEnvironmentVariable("eventHubName"), new DefaultAzureCredential()))
                {
                    using EventDataBatch eventBatch = await producer.CreateBatchAsync();
                    eventBatch.TryAdd(new EventData(new BinaryData($"Event being sent at { DateTime.Now }")));

                    await producer.SendAsync(eventBatch);

                }
            }
            catch(Exception ex) 
            {
                log.LogError(ex.Message);
			}

        }
    }
}
