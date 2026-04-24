#!/bin/bash
set -e

CONFIG_DIR="/OPC Foundation/GDS/config"
CONFIG_FILE="$CONFIG_DIR/Opc.Ua.GlobalDiscoveryServer.Config.xml"
HTTPS_DIR="/OPC Foundation/HTTPSCert"
HTTPS_CERT="$HTTPS_DIR/aspnetapp.pfx"

mkdir -p "$CONFIG_DIR"
mkdir -p "$HTTPS_DIR"
mkdir -p "/OPC Foundation/GDS/PKI/own/certs"
mkdir -p "/OPC Foundation/GDS/PKI/trusted/certs"
mkdir -p "/OPC Foundation/GDS/PKI/rejected/certs"

if [ ! -f "$CONFIG_FILE" ]; then
    echo "Copying OPC UA config into volume..."
    cp /app/GdsConfig/Opc.Ua.GlobalDiscoveryServer.Config.xml "$CONFIG_FILE"
fi

if [ ! -f "$HTTPS_CERT" ]; then
    echo "Copying HTTPS cert into volume..."
    cp /app/HTTPSCert/aspnetapp.pfx "$HTTPS_CERT"
fi

exec dotnet GDSwithREST.dll "$@"