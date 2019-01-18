![gablogo][gablogo]

# Lab 1 - Continouus Integration and Continuous Deployment (CI/CD) 

## Goal

In this lab you will create an application in .Net Core, push it to a remote repository and create a Continouus Integration and Continuous Deployment (CI/CD) with the Azure DevOps Pipeline to deploy the website to Azure. 

## Let's code!

### Build The Application

Let's create a new web application using the .Net Core. Open a terminal, and navigate to a "dev" folder (ex: C:\Dev).

    cd C:\dev

Let's scafold an application named GABCDemo using the following command:

    dotnet new mvc -o GABCDemo

Go into the new folder where your application as just been created.

    cd GABCDemo

Open the solution into Visual Studio Code with:

    code .

### Initialize Local Repository

To create a Git repository you can from Visual Studio Code open a terminal (Ctrl + \`) or return to the terminal windows already open. You should be in the folder `C:\Dev\GABCDemo`. Type

    git Init

This command will initialize a local repository. Now let's add alll the code files to the repo.

    git add -A

Now Git will track the evolution of our code. Git is a decentralized code repository system, therefore there is usually many repository where you can push and pull. However, before we can push our code to a remote repository we have other task to do. Will come back to it later.


### Creat The Azure WebApp

The next step is to create a placeholder for our website.  We will create an empty shell of a web application in Azure with these three Azure CLI commands. You can execute them locally or from the Cloud Shell. (Don't forget to validate that you are in the good subscription)

    az group create --name gabcdemogroup --location eastus

    az appservice plan create --name gabcdemoplan --resource-group gabcdemogroup --sku FREE

    az webapp create --name gabcdemo --resource-group gabcdemogroup --plan gabcdemoplan

The first command will create a Resource group. Then inside of this group we create a service plan, and finally we create a webapp to the mix. 


### Create an Azure DevOps project 

Navigate to [Dev.Azure.com](http://Dev.Azure.com) and if you don't already have an account create one it's free! Once you are logged-in, create a new project by clicking the New project blue button in the top right corner.

![createNewProject][createNewProject]

You will need to provide a unique name and a few simple information. 

### Get the Remote Repository Url

Here you have two options you can use the repository provided in the Azure DevOps project you just created, or create a another one in GitHub.

#### Option 1: Using Azure DevOps Repos

On Azure DevOps portal, from the left menu select *Repos*. Copy the command under the `or push an existing repository from command line`.

[ADD AN IMAGE]

#### Option 2: Using GitHub

[SOON]

### Add a Remote Repository

Return to the Terminal/ Console and paste the command.

    git remote add origin https:..... 
    git push origin --all

THe first line is to add the remote repository and name it "origin". The second line is to push (upload) the content of the local repository to origin (the remote one). You will need to enter your credential.

> Note: You may need to add some creds from the Setting in Azure DevOps [NEED TO TEST AND DOCUMENT THIS]

## Reference

## End
[Previous Lab](../Lab0/README.md)
[Next Lab](../Lab2/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[createNewProject]: medias/createNewProject.png