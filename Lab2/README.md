![gablogo][gablogo]

# Lab 2 - Azure Resource Manager (ARM) template 

## Goal

In this lab you will create an Azure Resource Manager template to provision your Azure resources and configure them automatically.

## What is Azure Resource Manager (ARM) template ?

Azure Resource Manager allows you to provision your applications using a declarative template. In a single template, you can deploy multiple services along with their dependencies. You use the same template to repeatedly deploy your application during every stage of the application lifecycle.

## The benefits of using Azure Resource Manager (ARM) template

Resource Manager provides several benefits:

* You can deploy, manage, and monitor all the resources for your solution as a group, rather than handling these resources individually.
* You can **repeatedly** deploy your solution throughout the development lifecycle and **have confidence** your resources are deployed in a consistent state.
* You can manage your infrastructure through declarative templates rather than scripts.
* You can define the dependencies between resources so they're deployed in the correct order.
* You can see your template as a documentation of your infrastructure.  ( [Infrastructure as a code](https://docs.microsoft.com/en-us/azure/devops/learn/what-is-infrastructure-as-code) )

# Template format 

A Resource manager template is simply a JSON file using this structure

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

| Element name | Required | Description |
|:--- |:--- |:--- |
| $schema |Yes |Location of the JSON schema file that describes the version of the template language.<br><br> For resource group deployments, use `https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#`.<br><br>For subscription deployments, use `https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#` |
| contentVersion |Yes |Version of the template (such as 1.0.0.0). You can provide any value for this element. Use this value to document significant changes in your template. When deploying resources using the template, this value can be used to make sure that the right template is being used. |
| parameters |No |Values that are provided when deployment is executed to customize resource deployment. |
| variables |No |Values that are used as JSON fragments in the template to simplify template language expressions. |
| functions |No |User-defined functions that are available within the template. |
| resources |Yes |Resource types that are deployed or updated in a resource group. |
| outputs |No |Values that are returned after deployment. |

[reference](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates)

## Let's code!

To host our solution, we will need to provision multiple Azure resources: 

* A new [App Service Plan](https://docs.microsoft.com/en-us/azure/app-service/overview-hosting-plans) 
* A new [Azure Web App](https://azure.microsoft.com/en-us/services/app-service/web/)
* A new [Azure Storage](https://docs.microsoft.com/en-us/azure/storage/common/storage-introduction)

Let's start by creating our template file. Open a terminal, and navigate to your "dev" folder (ex: C:\Dev).

    cd C:\dev

Now, Create an empty folder 

    MD Deployment

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
### Adding a Web App 

> We need to add a new parameter "appName"

```json
  "appName": {
      "type": "string",
      "metadata": {
        "description": "The name of your App Service instance."
      }
    }
```
> We need to add a new resource type "Microsoft.Web/sites"

```json
{
  "apiVersion": "2018-02-01",
  "type": "Microsoft.Web/sites",
  "dependsOn": [
    "[resourceId('Microsoft.Web/serverfarms', parameters('appSvcPlanName'))]"
  ],
  "kind": "app",
  "location": "[parameters('location')]",
  "name": "[parameters('appName')]",
  "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', parameters('appSvcPlanName'))]",
}
```
### Now lets configure our Web App Automatically



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
