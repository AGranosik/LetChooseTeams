## create resource group

`az group create -l polandcentral -n lct-rs`

## create aks cluster
`az aks create -g lct-rs -n lct-cluster --ssh-key-value .\aks-ssh.pub --node-count 1`

## create azure key-vault
`az aks enable-addons --addons azure-keyvault-secrets-provider --name lct-cluster --resource-group lct-rs`

`az keyvault set-policy -n lct-kvv --secret-permissions get --spn ...`