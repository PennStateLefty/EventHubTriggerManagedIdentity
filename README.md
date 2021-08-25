# EventHubTriggerManagedIdentity

A simple Azure Function app demonstrating usage of Azure Managed Identities in EventHubTrigger bindings. 

## Azure Resources required to deploy

1. A Resource Group
2. An Event Hub Namespace
    * A Event Hub Entity within
3. A Function App (Linux or Windows, v3 Function target)

## Enabling Managed Identity (Azure Function access to Event Hub Entity

1. From the Function App created access the Identity blade under the Settings section
2. Enable a System assigned Managed Identity (The Identity name will be the same as the Function App name)
3. Navigate to the Event Hub Entity created in step 2a.
4. Select Access control (IAM) from the sidebar
6. Click the Add role assignment button on the right side of the layout
7. From the roles available select Azure Event Hub Receiver or Azure Event Hub Sender then click the Next button at the bottom
8. In the Members step select the Managed identity radio button related to Assign access to then click + Select Members
9. Navigate to the System assigned Identity created in step 2 above
    * Select the Resource type (Function App)
    * Ensure the correct subscription is selected
    * Pick the identity created by the Function App
10. Review and Create
11. Repeat steps 6 - 10 for the other role (Sender or Receiver)

At this point the Function App will have permissions to Send and Receive messages from the Event Hub entity configured. In order to test on your local machine you will want to explicitly assign the same Azure Event Hub Sender/Receiver roles to the account you use in Visual Studio/VSCode/Azure CLI as this code uses DefaultAzureCredential, a reference to environmentally cached credentials. Basically, when local it will use entitlements flowing from the developer account, when running in Azure it will use the System assigned identity. So long as the role assignments are the same the local testing will be valid for the deployed state.
