## create resource group

`az group create -l polandcentral -n lct-rs`

## create aks cluster
`az aks create -g lct-rs -n lct-cluster --ssh-key-value .\aks-ssh.pub --node-count 1`

