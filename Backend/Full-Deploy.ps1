$parentPath = $(Split-Path -Path $(Get-Location) -Parent)
$VaultHeaders = @{
    "X-Vault-Token" = "00000000-0000-0000-0000-000000000000"
}

function CreateCredential {

    param (
        $Path,
        $KeyValueDict,
        $VaultHeaders
    )

    $Body = @{
        options = @{
            cas = 0
        }
        data    = $KeyValueDict
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "http://localhost:8200/v1/secret/data/$Path" -Method POST -Headers $VaultHeaders -Body $Body -ContentType "application/json"

    Write-Host "Successfully created credential for <$Path>" -ForegroundColor green
}

$userDb = "source=" + $parentPath + "/db/user"
$appDb = "source=" + $parentPath + "/db/app"

docker run -d -p 5000:5000 --restart=always --name registry registry:2

docker network create -d overlay web_app

docker service create --name vault --network web_app -p 8201:8201 -p 8200:8200 --cap-add IPC_LOCK --env-file ./vault.env vault
docker service create --name rabbit --network web_app -p 15672:15672 -p 5672:5672 -p 5671:5671 rabbitmq:3-management
docker service create --name redis --network web_app -p 6379:6379 redis
docker service create --name mongo --network web_app -p 27017:27017 mongo
docker service create --name userdb --network web_app --env POSTGRES_PASSWORD=postgres -p 5432:5432 --mount type=bind,$userDb,destination=/docker-entrypoint-initdb.d/ postgres
docker service create --name appdb --network web_app --env POSTGRES_PASSWORD=postgres -p 5433:5432 --mount type=bind,$appDb,destination=/docker-entrypoint-initdb.d/ postgres

$paths = Get-ChildItem -Path "./creds" -Directory

foreach ($path in $paths) {
    $keys = Get-ChildItem -Path $path.FullName

    $keysToAdd = @{}

    foreach ($key in $keys) {
        $credential = Get-Content $key.FullName -Raw
        $keysToAdd.Add($key.Name, $credential)
    }

    CreateCredential -Path $path.Name -KeyValueDict $keysToAdd -VaultHeaders $VaultHeaders
}

# FRONTEND

cd ..
cd front

docker build -f Dockerfile -t localhost:5000/web_app_front .
docker push localhost:5000/web_app_front
docker service create --name front --network web_app -p 3000:3000 -t localhost:5000/web_app_front

cd ..
cd Backend

# API GATEWAY

docker build -f API-Gateway-Dockerfile -t localhost:5000/web_app_gateway .
docker push localhost:5000/web_app_gateway
docker service create --name gateway --network web_app -p 12345:80 localhost:5000/web_app_gateway

# AUTH

docker build -f Auth-Dockerfile -t localhost:5000/web_app_auth .
docker push localhost:5000/web_app_auth
docker service create --name auth --network web_app localhost:5000/web_app_auth

# CENTRAL

docker build -f Central-Dockerfile -t localhost:5000/web_app_central .
docker push localhost:5000/web_app_central
docker service create --name central --network web_app localhost:5000/web_app_central

# CLIENT CREATOR

docker build -f Client-Creator-Dockerfile -t localhost:5000/web_app_client_creator .
docker push localhost:5000/web_app_client_creator
docker service create --name client_creator --network web_app localhost:5000/web_app_client_creator

# PRESENTER

docker build -f Presenter-Dockerfile -t localhost:5000/web_app_presenter .
docker push localhost:5000/web_app_presenter
docker service create --name presenter --network web_app localhost:5000/web_app_presenter

# NOTIFIER

docker build -f Notifier-Dockerfile -t localhost:5000/web_app_notifier .
docker push localhost:5000/web_app_notifier
docker service create --name notifier --network web_app localhost:5000/web_app_notifier

# EMAILNOTIFIER

docker build -f EmailNotifier-Dockerfile -t localhost:5000/web_app_emailnotifier .
docker push localhost:5000/web_app_emailnotifier
docker service create --name emailnotifier --network web_app localhost:5000/web_app_emailnotifier

# SMSNOTIFIER

docker build -f SMSNotifier-Dockerfile -t localhost:5000/web_app_smsnotifer .
docker push localhost:5000/web_app_smsnotifer
docker service create --name smsnotifer --network web_app localhost:5000/web_app_smsnotifer

docker service create --name logstash --network web_app -p 33311:33311 -p 31311:31311 --mount type=bind,source=$parentPath/ext/logstash,destination=/usr/share/logstash/config/ docker.elastic.co/logstash/logstash:7.10.0
docker service create --name elasticsearch --network web_app -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.9.3-amd64
docker service create --name kibana --network web_app -p 5601:5601 docker.elastic.co/kibana/kibana:7.9.3