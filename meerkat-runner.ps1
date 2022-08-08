[CmdletBinding()]
param (
    [string]$Token = "00000000-0000-0000-0000-000000000000"
)

$rabbitPodName = (kubectl get pod -l app=rabbit)[1].Split(" ")[0]
$rabbitIp = ((kubectl get pod $rabbitPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]

$configProdTemplate = (Get-Content "./Backend/VigilantMeerkat.Micro.Meerkat/config.temp.json" -Raw).Replace("{rabbit}", $rabbitIp).Replace("{token}", $Token)

Set-Content -Path "./Backend/VigilantMeerkat.Micro.Meerkat/config.json" -Value $configProdTemplate

Set-Location .\Backend

docker build -f Meerkat-Dockerfile -t theberserkerzx/web_app_meerkat:latest .
docker push theberserkerzx/web_app_meerkat

Set-Location ..
Set-Location .\kubernetes

kubectl apply -f meerkat-deployment.yaml

Set-Location ..