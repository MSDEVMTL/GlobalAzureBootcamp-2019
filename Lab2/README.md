![gablogo][gablogo]

# Lab 2 - Azure Resource Manager (ARM) template

## Goal

In this lab you will create an Azure Resource Manager template to provision your Azure resources and configure them automatically.

## What is Azure Resource Manager (ARM) template

Azure Resource Manager allows you to provision your applications using a declarative template. In a single template, you can deploy multiple services along with their dependencies. You use the same template to repeatedly deploy your application during every stage of the application life cycle.

## The benefits of using Azure Resource Manager (ARM) template

Resource Manager provides several benefits:

* You can deploy, manage, and monitor all the resources for your solution as a group, rather than handling these resources individually.
* You can **repeatedly** deploy your solution throughout the development life cycle and **have confidence** your resources are deployed in a consistent state.
* You can manage your infrastructure through declarative templates rather than scripts.
* You can define the dependencies between resources so they're deployed in the correct order.
* You can see your template as a documentation of your infrastructure.  ( [Infrastructure as a code](https://docs.microsoft.com/en-us/azure/devops/learn/what-is-infrastructure-as-code) )

ARM template are pretty simple.  They are just json files that describe the infrastructure of your project. You define your structure in a template and then use it with a parameter file.

```txt
myproject.json
myproject.parameters.json
```

The two together can be deploy on different environment.  You will change your parameter file depending on which environment you are deploying.

We could want to deploy a VM or a Web application.  Let's say that your development environment could required a smaller resource than your production environment. With ARM, it will be the same structure, the same code, except your parameter file will change the size of your VM or Web application.

Also, when you deploying resources on Azure, ARM will parallelize you deployment.  It's really the fastest way to deploy.

# Deployment using ARM templates

You can deploy them using 3 ways.

1) Using Visual Studio

![VisualStudio_ARM](http://techgenix.com/tgwordpress/wp-content/uploads/2018/06/1050-05-04-1-1024x399.png)

2) Using the Azure Portal

![Portal_ARM](http://techgenix.com/tgwordpress/wp-content/uploads/2018/06/1050-05-07.png)

3) Using powershell

![Powershell_arm](http://techgenix.com/tgwordpress/wp-content/uploads/2018/06/1050-05-08-1024x390.png)

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

# Let's code!

Now that we know a bit more on these ARM templates, let see how we can use them in our project.

In the previous lab, we deployed our web app on an Azure Web App.  This Azure Web App itself require an App Service Plan to run.

| Resource | Link
|-----|-----|
| App Service Plan | [Reference Link](https://docs.microsoft.com/en-us/azure/app-service/overview-hosting-plans)|
| Azure Web App | [Reference Link](https://azure.microsoft.com/en-us/services/app-service/web/)|

## Part 1 - Create our first ARM template

1) Now, create your deployment folder.

Under your solution folder, add a new folder `Deployment`.  This is where all your deployment scripts will be located.

    C:\dev\gab2019\deployment

2) Under this new folder, create two new empty files. A ARM template and its parameter file.

   C:\dev\gab2019\deployment\gab2019.json
   C:\dev\gab2019\deployment\gab2019.parameters.json

3) Open Visual Studio code, if its not already open, and locate your two newly added files.

4) Open your empty template file gab2019.json

5) Insert an ARM Template Skeleton

If you have install the extension, you can do it easily if you type `arm!` at the beginning of the file or copy this snippet directly.

![Insert_ARM_Template_Skeleton](https://raw.githubusercontent.com/sam-cogan/arm-snippets-vscode/master/Extension/images/skeleton.gif)

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {},
    "variables": {},
    "resources": [],
    "outputs": {}
}

```

6) Insert an Azure Service Plan

Move your cursor between the bracket `"resources": []` and copy this code snippet

```json
 {
    "type": "Microsoft.Web/serverfarms",
    "apiVersion": "2015-08-01",
    "name": "APP_SERVICE_PLAN_NAME",
    "location": "[resourceGroup().location]",
    "sku": {
      "name": "F1",
      "tier": "Free",
      "capacity": 1
    }
  }
```

7) Right after, insert an Azure Web App.  Remember to add a comma between element since its a json array.

```json
{
  "type": "Microsoft.Web/sites",
  "apiVersion": "2018-02-01",
  "name": "WEB_APP_NAME",
  "location": "[resourceGroup().location]",
  "dependsOn": [
    "[resourceId('Microsoft.Web/serverfarms/', 'APP_SERVICE_PLAN_NAME')]"  
  ],
  "properties": {
      "name": "WEB_APP_NAME",
      "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', 'APP_SERVICE_PLAN_NAME')]"
  }
}

```

That should look like this

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {},
    "variables": {},
    "resources": [
      {
          "type": "Microsoft.Web/sites",
          "apiVersion": "2018-02-01",
          "name": "WEB_APP_NAME",
          "location": "[resourceGroup().location]",
          "dependsOn": [
            "[resourceId('Microsoft.Web/serverfarms/', 'APP_SERVICE_PLAN_NAME')]"  
          ],
          "properties": {
              "name": "WEB_APP_NAME",
              "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', 'APP_SERVICE_PLAN_NAME')]"
          }
      },
      {
        "type": "Microsoft.Web/serverfarms",
        "apiVersion": "2015-08-01",
        "name": "APP_SERVICE_PLAN_NAME",
        "location": "[resourceGroup().location]",
        "sku": {
          "name": "F1",
          "tier": "Free",
          "capacity": 1
        }
      }
    ],
    "outputs": {}
}
```

8) Use parameters and variables

We now need to replace the place holder with parameters. This way, we will be able to customize our template.

* APP_SERVICE_PLAN_NAME
* WEB_APP_NAME

```json

 "parameters": {
      "appSvcPlanName": {
        "type": "string",
        "metadata": {
          "description": "The name of the App Service Plan that will host your Web App."
        }
      },
      "webAppName": {
        "type": "string",
        "metadata": {
          "description": "The name of your Web App."
        }
      }
    }

```

And use the parameter this syntax

```json
...[parameters('webAppName')]... 

```

That should now look like this

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
      "appSvcPlanName": {
        "type": "string",
        "metadata": {
          "description": "The name of the App Service Plan that will host your Web App."
        }
      },
      "webAppName": {
        "type": "string",
        "metadata": {
          "description": "The name of your Web App."
        }
      }
    },
    "variables": {},
    "resources": [
      {
         "type": "Microsoft.Web/sites",
          "apiVersion": "2018-02-01",
          "name": "[parameters('webAppName')]",
          "location": "[resourceGroup().location]",
         "dependsOn": [
            "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]"  
          ],
          "properties": {
              "name": "[parameters('webAppName')]",
              "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]"
          }
      },
      {
        "type": "Microsoft.Web/serverfarms",
        "apiVersion": "2015-08-01",
        "name": "[parameters('appSvcPlanName')]",
        "location": "[resourceGroup().location]",
        "sku": {
          "name": "F1",
          "tier": "Free",
          "capacity": 1
        }
      }
    ],
    "outputs": {}
}
```

## Part 4 - Add an Azure Storage to the mix

```json

```

## Part 5 - Configure our Web App Automatically

```json

```

## Reference

If you want to know more on the syntax or the advantage of using Azure Resource Manager please visit the following:

* [What is Azure Resource Manager](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-overview)
* [Understanding the structure and syntax of Azure Resource Manager templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates)
* [Best practices using Azure Resource Manager (ARM) Templates](https://www.youtube.com/watch?v=myYTGsONrn0)
* [Azure Resource Manager](https://azure.microsoft.com/en-us/resources/templates/)
* [A library of examples](https://github.com/Azure/azure-quickstart-templates)
* [Exploring three way to deploy your ARM templates](http://techgenix.com/deploy-arm-templates/)

## End

[Previous Lab](../Lab1/README.md)
[Next Lab](../Lab3/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
