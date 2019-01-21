![gablogo][gablogo]

# Lab 2 - Azure Resource Manager (ARM) template 

## Goal

In this lab you will create an Azure Resource Manager template to provision your Azure resources and configure them automatically.

## Azure Resource Manager (ARM) template ?

Azure Resource Manager allows you to provision your applications using a declarative template. In a single template, you can deploy multiple services along with their dependencies. You use the same template to repeatedly deploy your application during every stage of the application lifecycle.

## The benefits of using Azure Resource Manager (ARM) template

Resource Manager provides several benefits:

* You can deploy, manage, and monitor all the resources for your solution as a group, rather than handling these resources individually.
* You can **repeatedly** deploy your solution throughout the development lifecycle and **have confidence** your resources are deployed in a consistent state.
* You can manage your infrastructure through declarative templates rather than scripts.
* You can define the dependencies between resources so they're deployed in the correct order.
* You can apply access control to all services in your resource group because Role-Based Access Control (RBAC) is natively integrated into the management platform.
* You can apply tags to resources to logically organize all the resources in your subscription.
* You can clarify your organization's billing by viewing costs for a group of resources sharing the same tag.
* You can see your template as a documentation of your infrastructure.  ( [Infrastructure as a code](https://docs.microsoft.com/en-us/azure/devops/learn/what-is-infrastructure-as-code) )

# Template format 

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "",
    "parameters": {  },
    "variables": {  },
    "functions": [  ],
    "resources": [  ],
    "outputs": {  }
}
```
## Let's code!

### Adding a service plan

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "appSvcPlanName": {
      "type": "string",
      "metadata": {
        "description": "The name of the App Service Plan that will host the Web App."
      }
    },
    "svcPlanSize": {
      "type": "string",
      "allowedValues": [
        "F1",
        "D1",
        "B1",
        "B2",
        "B3",
        "S1",
        "S2",
        "S3",
        "P1",
        "P2",
        "P3",
        "P4"
      ],
      "defaultValue": "F1",
      "metadata": {
        "description": "The instance size of the App Service Plan."
      }
    },
    "svcPlanSku": {
      "type": "string",
      "allowedValues": [
        "Free",
        "Shared",
        "Basic",
        "Standard",
        "Premium"
      ],
      "defaultValue": "Free",
      "metadata": {
        "description": "The pricing tier of the App Service plan."
      }
    },
    "location": {
      "type": "string",
      "defaultValue": "[resourceGroup().location]",
      "metadata": {
        "description": "Location for all resources."
      }
    }
  },
  "variables": {},
  "resources": [
    {
      "type": "Microsoft.Web/serverfarms",
      "apiVersion": "2015-08-01",
      "name": "[parameters('appSvcPlanName')]",
      "location": "[parameters('location')]",
      "sku": {
        "name": "[parameters('svcPlanSize')]",
        "tier": "[parameters('svcPlanSku')]",
        "capacity": 1
      }
    }
  ]
}
```
### Adding a website 

```json
  "appName": {
      "type": "string",
      "metadata": {
        "description": "The name of your App Service instance."
      }
    }
```

```json
{
  "apiVersion": "2018-02-01",
  "dependsOn": [
    "[resourceId('Microsoft.Web/serverfarms', parameters('appSvcPlanName'))]"
  ],
  "kind": "app",
  "location": "[parameters('location')]",
  "name": "[parameters('appName')]",
  "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', parameters('appSvcPlanName'))]",
},
```


## Reference

If you want to know more on the syntax or the advanatge of using Azure Resource Manager please visit the following:

* [What is Azure Resource Manager](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-overview)
* [Best practices using Azure Resource Manager (ARM) Templates](https://www.youtube.com/watch?v=myYTGsONrn0)
* [Azure Resource Manager](https://azure.microsoft.com/en-us/resources/templates/)
* [A library of example](https://github.com/Azure/azure-quickstart-templates)
* [Exploring three way to deploy your ARM templates](http://techgenix.com/deploy-arm-templates/)

## End
[Previous Lab](../Lab1/README.md)
[Next Lab](../Lab3/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
