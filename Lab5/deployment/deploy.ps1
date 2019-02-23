[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)][string]$webAppNameSuffix,
    [string]$location = "canadacentral",
    [string]$resourceGroupName = "GAB2019RG",
    [string]$deploymentName = "Gab2019Deployment1",
    [bool]$createResourceGroup = $false
)

$webAppName = "gab2019-mvc-$webAppNameSuffix"

if ($createResourceGroup) {
    az group create --name $resourceGroupName --location $location
}
az group deployment create --name $deploymentName --mode Incremental --resource-group $resourceGroupName --template-file gab2019.json --parameters GAB2019.parameters.json --parameters "webAppName=$webAppName"