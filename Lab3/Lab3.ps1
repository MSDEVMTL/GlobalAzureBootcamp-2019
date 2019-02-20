# You must run this script from your application directory (same level as your Dockerfile).
# If you run it, it will do the whole lab3 for you (almost); you might want to reconsider if you want to learn something out of it tho.
#
# A better use of this script would be to help debug yourself!
# 
# Run this using the following synthax:
# .\lab3.ps1 -containerRegistryName <containerRegistryName> -containerDnsName <containerDnsName>
#
# Example:
# .\lab3.ps1 -containerRegistryName GAB2019ContainerRegistry -containerDnsName gab-2019-container-demo
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)][string]$containerRegistryName,
    [Parameter(Mandatory = $true)][string]$containerDnsName,
    [string]$resourceGroupName = "GAB2019Group",
    [string]$location = "canadacentral",
    [string]$imageName = "gabdemo",
    [string]$containerName = "gab2019container"
)

# Create the local Docker image
$localImageName = "${imageName}:latest"
Write-Debug "localImageName: $localImageName"
docker build --rm -f "Dockerfile" -t $localImageName .

# Create Azure resources
az group create --name $resourceGroupName --location $location
az acr create --resource-group $resourceGroupName --name $containerRegistryName --sku Basic --admin-enabled true
az acr login --name $containerRegistryName

# Prepare and push the Docker image into Azure
$azureImageName = "${imageName}:v1"
$acrLoginServer = "$containerRegistryName.azurecr.io".ToLower()
Write-Debug "azureImageName: $azureImageName"
Write-Debug "acrLoginServer: $acrLoginServer"
docker tag $imageName $acrLoginServer/$azureImageName
docker push $acrLoginServer/$azureImageName

# Start a Container based on our Docker image
az container create --resource-group $resourceGroupName --name $containerName --image $acrLoginServer/$azureImageName --dns-name-label $containerDnsName --ports 80

# Open the URI in a browser
$fullUri = "http://$containerDnsName.$location.azurecontainer.io"
Start-Process $fullUri