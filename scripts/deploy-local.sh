#!/bin/bash
set -e

echo "🚀 Démarrage du déploiement local..."

# Arrêter les conteneurs existants
echo "🛑 Arrêt des conteneurs existants..."
docker-compose down || true

# Nettoyer les anciennes images
echo "🧹 Nettoyage des anciennes images..."
docker system prune -f

# Démarrer les services
echo "🐳 Démarrage des services..."
docker-compose up -d

# Attendre que les services soient prêts
echo "⏳ Attente des services..."
sleep 30

echo "✅ Déploiement terminé!"
echo "🌐 Application accessible sur : http://localhost:8080"