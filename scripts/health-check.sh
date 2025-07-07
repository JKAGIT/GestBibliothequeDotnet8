#!/bin/bash
set -e

echo "🏥 Vérification de l'état des services..."

# Vérifier le conteneur de l'application
if docker ps | grep -q "gestbiblio-app"; then
    echo "✅ Application container: Running"
else
    echo "❌ Application container: Not running"
    exit 1
fi

# Vérifier le conteneur SQL Server
if docker ps | grep -q "gestbiblio-sqlserver"; then
    echo "✅ SQL Server container: Running"
else
    echo "❌ SQL Server container: Not running"
    exit 1
fi

# Vérifier l'endpoint HTTP
echo "🌐 Test de l'endpoint HTTP..."
max_attempts=10
attempt=0

while [ $attempt -lt $max_attempts ]; do
    if curl -f http://localhost:8080/health > /dev/null 2>&1; then
        echo "✅ Application HTTP: OK"
        break
    else
        echo "⏳ Tentative $((attempt + 1))/$max_attempts..."
        sleep 10
        attempt=$((attempt + 1))
    fi
done

if [ $attempt -eq $max_attempts ]; then
    echo "❌ Application HTTP: Échec après $max_attempts tentatives"
    exit 1
fi

echo "🎉 Tous les services sont opérationnels!"