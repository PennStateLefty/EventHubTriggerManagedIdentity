using System;
using Microsoft.Azure.WebJobs;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Logging;
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
                //Create a new producer client - leverage DefaultAzureCredentials to allowed Managed Identity to handle authentication and authorization
                await using (var producer = new EventHubProducerClient(fullyQualifiedNamespace: Environment.GetEnvironmentVariable("eventHubConnection__fullyQualifiedNamespace"), eventHubName: Environment.GetEnvironmentVariable("eventHubName"), new DefaultAzureCredential()))
                {
                    //Create and send an event to the Event Hub
                    
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
