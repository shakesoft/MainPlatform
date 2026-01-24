#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 2237bbee-336c-49a2-9c20-baf3e9af8c0f -t
    fi
    cd ../
fi

docker-compose up -d
