#!/bin/bash

if [[ -z "$1" ]]; then
  echo "Expected no of tenants arg" 1>&2;
  exit 1
fi

NUM_TENANTS=$1
PORT=5000
NAME="tenancyexample"
KIND="$NAME"
IMAGE_TAG="$NAME:0.1"

# On exit
set -e
function cleanup {
  kind delete cluster --name $KIND
}
trap cleanup EXIT


function waitpods() {
  sleep 2
  while kubectl --context kind-$KIND get pods --all-namespaces | grep -Eq "Pending|Waiting|ContainerCreating"
  do
    echo "waiting for pods ..." 
    sleep 2
  done
}


##
## Create the DB init scripts and .Net connection strings
##

mkdir -p /tmp/exmaple/initdb
CONN_STRINGS=""

for ((i=0; i<NUM_TENANTS; i++))
do
  TENANT="tenant$i"
  export TENANT
  
  CN='"'"${TENANT}"'" : "'"server=${TENANT}db;database=example;port=3306;user=example;password=example"'"'
  CONN_STRINGS="$CONN_STRINGS$CN,"
  
  envsubst <<EOF > "/tmp/exmaple/initdb/$TENANT.sql"
CREATE SCHEMA if not exists example;

USE example;

CREATE TABLE hello 
(
    hello_id BIGINT primary key auto_increment,
    say NVARCHAR(256) not null
);

INSERT INTO hello (say) values ('Hello ${TENANT}'), ('Hallo ${TENANT}'), ('Hola ${TENANT}'), ('Bonjour ${TENANT}');
EOF

done

#TODO make a configmap
printf '{"ConnectionStrings": {%s}}' "${CONN_STRINGS::-1}" > "TenancyExample/dbsettings.json"

docker build -t "$IMAGE_TAG" TenancyExample

##
## Create the cluster
##

#mount db init scripts
envsubst <<EOF | kind create cluster --name $KIND --config -
apiVersion: kind.x-k8s.io/v1alpha4
kind: Cluster
nodes:
  - role: control-plane
    extraMounts:
      - hostPath: /tmp/exmaple/initdb
        containerPath: /initdb
EOF

kind load docker-image --name $KIND "$IMAGE_TAG"
kind load docker-image --name $KIND mysql:8.0

#create dbs
for ((i=0; i<NUM_TENANTS; i++))
do
  TENANT="tenant$i"
  export TENANT

  envsubst <<EOF | kubectl --context kind-$KIND create -f - 
apiVersion: apps/v1
kind: Deployment
metadata:
  labels:
    app: ${TENANT}db
  name: ${TENANT}db
spec:
  replicas: 1
  selector:
    matchLabels:
      app: ${TENANT}db
  template:
    metadata:
      labels:
        app: ${TENANT}db
    spec:
      volumes:
      - name: init
        hostPath:
          path: /initdb/${TENANT}.sql 
      containers:
      - image: "mysql:8.0"
        name: mysql
        env:
        - name: MYSQL_ROOT_PASSWORD
          value: example
        - name: MYSQL_DATABASE
          value: example
        - name: MYSQL_USER
          value: example
        - name: MYSQL_PASSWORD
          value: example
        ports:
        - containerPort: 3306
        volumeMounts:
        - name: init
          mountPath: /docker-entrypoint-initdb.d/${TENANT}.sql
EOF

kubectl --context kind-$KIND expose deployment ${TENANT}db
 
done

#create app
kubectl --context kind-$KIND create deployment "$NAME"  --image="$IMAGE_TAG" --port=$PORT --replicas=2
kubectl --context kind-$KIND expose deployment tenancyexample


waitpods


if [[ "Z$(kubectl --context kind-$KIND get pods | grep "CrashLoopBackOff")" != "Z" ]]
then
  echo "There are pods with errors, try running less (< $NUM_TENANTS) db instances" 1>&2;
  exit 1  
fi


#Port forward

kubectl --context kind-$KIND port-forward --address 127.0.0.1 service/tenancyexample $PORT:$PORT &
PFPID=$?

while [[ "Z$(netstat -ln | grep ":$PORT")" = "Z" ]]
do
  echo "waiting for port $PORT ..." 
  sleep 2
done


#
# Run tests
#

for ((i=0; i<NUM_TENANTS; i++))
do
  TENANT="tenant$i"
  curl --retry 3 --retry-delay 3 -X GET --location "http://127.0.0.1:$PORT/api/hello" \
    -H "Accept: application/json" \
    -H "X-TenantId: $TENANT"
done
