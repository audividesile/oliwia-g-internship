[CmdletBinding()]
param (
    [string]$Scale = "1"
)

kubectl scale --replicas=$Scale replicaset gateway-deployment-c9568f567