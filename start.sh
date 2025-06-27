#!/bin/bash

echo "🚀 Starting DeepSeek Chat Application..."
echo "================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Check if docker-compose is available
if ! command -v docker-compose > /dev/null 2>&1; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

echo "📦 Building and starting containers..."
docker-compose up --build -d

echo "⏳ Waiting for services to start..."
sleep 10

echo "🔍 Checking service status..."
docker-compose ps

echo ""
echo "✅ Application is ready!"
echo "🌐 Frontend: http://localhost:4200"
echo "🔧 Backend API: http://localhost:5000"
echo "📚 API Documentation: http://localhost:5000/swagger"
echo ""
echo "📝 To view logs:"
echo "   docker-compose logs -f deepseek-api"
echo "   docker-compose logs -f deepseek-frontend"
echo ""
echo "🛑 To stop the application:"
echo "   docker-compose down" 