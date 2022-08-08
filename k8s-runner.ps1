Set-Location .\kubernetes

kubectl create -f appdb-config-map.yaml 
kubectl create -f userdb-config-map.yaml 
kubectl create -f appdb-volume.yaml 
kubectl create -f userdb-volume.yaml 
kubectl apply -f appdb-deployment.yaml
kubectl apply -f userdb-deployment.yaml
kubectl apply -f rabbit-deployment.yaml
kubectl apply -f redis-deployment.yaml
kubectl apply -f mongo-deployment.yaml

Set-Location ..

Set-Location .\front

docker build -t theberserkerzx/web_app_front:latest .
docker push theberserkerzx/web_app_front

Set-Location ..
Set-Location .\kubernetes

Start-Sleep -s 20

kubectl apply -f frontend-deployment.yaml
kubectl expose deployment frontend-deployment --type=NodePort --port=3000
kubectl expose deployment rabbit-deployment --type=NodePort --port=15672

$url = minikube service frontend-deployment --url

kubectl apply -f frontend-ingress.yaml

Set-Location ..

Start-Sleep -s 20

$userDbPodName = (kubectl get pod -l app=user-db)[1].Split(" ")[0]
$appDbPodName = (kubectl get pod -l app=app-db)[1].Split(" ")[0]
$rabbitPodName = (kubectl get pod -l app=rabbit)[1].Split(" ")[0]
$mongoPodName = (kubectl get pod -l app=mongo)[1].Split(" ")[0]
$redisPodName = (kubectl get pod -l app=redis)[1].Split(" ")[0]
$userDbIp = ((kubectl get pod $userDbPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]
$appDbIp = ((kubectl get pod $appDbPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]
$rabbitIp = "<none>"
while ($rabbitIp -eq "<none>") {
    $rabbitIp = ((kubectl get pod $rabbitPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]
    echo $rabbitIp
}
$mongoIp = ((kubectl get pod $mongoPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]
$redisIp = ((kubectl get pod $redisPodName -o wide)[1].Split(" ") | Select-Object -Unique)[6]

$userDbInitScript = Get-Content "./db/user/init.sql" -Raw
$appDbInitScript = Get-Content "./db/app/init.sql" -Raw

echo $rabbitIp

kubectl exec $userDbPodName -- psql --username postgres --dbname postgres -c $userDbInitScript
kubectl exec $appDbPodName -- psql --username postgres --dbname postgres -c $appDbInitScriptW

$configProdTemplate = (Get-Content "./Backend/Config/config.prod.temp.json" -Raw).Replace("{userdb}", $userDbIp).Replace("{appdb}", $appDbIp).Replace("{rabbit}", $rabbitIp).Replace("{mongo}", $mongoIp).Replace("{redis}", $redisIp)
$appSettingsTemplate = (Get-Content "./Backend/Gateway/appsettings.temp.json" -Raw).Replace("{userdb}", $userDbIp).Replace("{appdb}", $appDbIp).Replace("{rabbit}", $rabbitIp).Replace("{mongo}", $mongoIp).Replace("{redis}", $redisIp)

Set-Content -Path "./Backend/Config/config.prod.json" -Value $configProdTemplate
Set-Content -Path "./Backend/Config/config.json" -Value $configProdTemplate
Set-Content -Path "./Backend/Gateway/appsettings.json" -Value $appSettingsTemplate

Set-Location .\Backend

docker build -f API-Gateway-Dockerfile -t theberserkerzx/web_app_gateway:latest .
docker push theberserkerzx/web_app_gateway

docker build -f Presenter-Dockerfile -t theberserkerzx/web_app_presenter:latest .
docker push theberserkerzx/web_app_presenter

docker build -f Auth-Dockerfile -t theberserkerzx/web_app_auth:latest .
docker push theberserkerzx/web_app_auth

docker build -f Central-Dockerfile -t theberserkerzx/web_app_central:latest .
docker push theberserkerzx/web_app_central

docker build -f Client-Creator-Dockerfile -t theberserkerzx/web_app_client_creator:latest .
docker push theberserkerzx/web_app_client_creator

docker build -f Notifier-Dockerfile -t theberserkerzx/web_app_notifier:latest .
docker push theberserkerzx/web_app_notifier

docker build -f EmailNotifier-Dockerfile -t theberserkerzx/web_app_email_notifier:latest .
docker push theberserkerzx/web_app_email_notifier

Set-Location ..
Set-Location .\kubernetes

kubectl apply -f gateway-deployment.yaml
kubectl apply -f presenter-deployment.yaml
kubectl apply -f auth-deployment.yaml
kubectl apply -f central-deployment.yaml
kubectl apply -f client-creator-deployment.yaml
kubectl apply -f notifier-deployment.yaml
kubectl apply -f email-notifier-deployment.yaml
kubectl expose deployment gateway-deployment --type=NodePort --port=80
