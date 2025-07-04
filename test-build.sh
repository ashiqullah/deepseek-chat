#!/bin/bash

echo "🧪 Testing DeepSeek Chat Build"
echo "==============================="

# Test backend build
echo "📦 Testing .NET backend build..."
cd backend

if dotnet build --configuration Release; then
    echo "✅ Backend build successful"
else
    echo "❌ Backend build failed"
    exit 1
fi

cd ..

# Test frontend build
echo "📦 Testing Angular frontend build..."
cd frontend

if npm install --silent && npm run build --silent; then
    echo "✅ Frontend build successful"
else
    echo "❌ Frontend build failed"
    exit 1
fi

cd ..

echo ""
echo "🎉 All builds successful!"
echo "Ready to run: docker-compose up -d" 