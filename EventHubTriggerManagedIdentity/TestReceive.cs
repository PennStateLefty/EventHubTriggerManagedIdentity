using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventHubTriggerManagedIdentity
{
    public static class TestReceive
    {
        //Note the annotation for EventHubTrigger. The 'Connection' reference will actually pull a application configuration in the Function App Service named "eventHubConnection__fullyQualifiedNamespace"
        //This is down to the newer version of the Microsoft.Azure.WebJobs SDK coupled with the v5.x.x-beta of Microsoft.Azure.WebJobs.Extensions.EventHubs being enabled for Managed Identities which no longer
        //requires the connection string style used when leveraging SAS tokens. 
        //Effectively, the new library is creating an EventHubConsumerClient from the configuration supplied in this annotation signature looking like:
        //Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient(string consumerGroupName, string fullyQualifiedNamespace, string eventHubName, Credential azureCredential)
        [FunctionName("HubReceiver")]
        public static async Task Run([EventHubTrigger("testhub", Connection = "eventHubConnection")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();
            
            foreach (EventData eventData in events)
            {
                try
                {
                    //Type Azure.Messaging.EventHubs.EventData offers convenience accessors to the EventBody to do away with needing to use Encoding classes in System.Text directly
                    string messageBody = eventData.EventBody.ToString();

                    log.LogInformation($"ATTENTION: INCOMING MESSAGE - C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
