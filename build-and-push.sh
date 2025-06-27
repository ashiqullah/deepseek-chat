#!/bin/bash

# Build and Push Docker Images Script
# Usage: ./build-and-push.sh [your-dockerhub-username]

DOCKER_USERNAME=${1:-"yourusername"}  # Replace with your Docker Hub username

echo "🔨 Building Docker images for multi-platform (AMD64 + ARM64)"
echo "=================================================="
echo "Docker Hub Username: $DOCKER_USERNAME"
echo ""

# Enable Docker BuildKit for multi-platform builds
export DOCKER_BUILDKIT=1

# Create and use a new builder instance for multi-platform builds
echo "📋 Setting up multi-platform builder..."
docker buildx create --name multiplatform-builder --use --bootstrap
docker buildx inspect --bootstrap

# Build and push backend image
echo "🔧 Building backend image..."
cd backend
docker buildx build \
    --platform linux/amd64,linux/arm64 \
    --tag $DOCKER_USERNAME/deepseek-api:latest \
    --tag $DOCKER_USERNAME/deepseek-api:v1.0 \
    --push \
    .

# Build and push frontend image
echo "🎨 Building frontend image..."
cd ../frontend
docker buildx build \
    --platform linux/amd64,linux/arm64 \
    --tag $DOCKER_USERNAME/deepseek-frontend:latest \
    --tag $DOCKER_USERNAME/deepseek-frontend:v1.0 \
    --push \
    .

cd ..

echo ""
echo "✅ Images built and pushed successfully!"
echo "🐳 Backend: $DOCKER_USERNAME/deepseek-api:latest"
echo "🐳 Frontend: $DOCKER_USERNAME/deepseek-frontend:latest"
echo ""
echo "📋 Next steps:"
echo "1. Update portainer-stack-prebuilt.yml with your username"
echo "2. Upload the stack file to Portainer"
echo "3. Deploy the stack" 