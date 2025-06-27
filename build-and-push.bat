@echo off
REM Build and Push Docker Images Script for Windows
REM Usage: build-and-push.bat [your-dockerhub-username]

set DOCKER_USERNAME=%1
if "%DOCKER_USERNAME%"=="" set DOCKER_USERNAME=yourusername

echo 🔨 Building Docker images for multi-platform (AMD64 + ARM64)
echo ==================================================
echo Docker Hub Username: %DOCKER_USERNAME%
echo.

REM Enable Docker BuildKit for multi-platform builds
set DOCKER_BUILDKIT=1

REM Create and use a new builder instance for multi-platform builds
echo 📋 Setting up multi-platform builder...
docker buildx create --name multiplatform-builder --use --bootstrap
docker buildx inspect --bootstrap

REM Build and push backend image
echo 🔧 Building backend image...
cd backend
docker buildx build --platform linux/amd64,linux/arm64 --tag %DOCKER_USERNAME%/deepseek-api:latest --tag %DOCKER_USERNAME%/deepseek-api:v1.0 --push .

REM Build and push frontend image
echo 🎨 Building frontend image...
cd ..\frontend
docker buildx build --platform linux/amd64,linux/arm64 --tag %DOCKER_USERNAME%/deepseek-frontend:latest --tag %DOCKER_USERNAME%/deepseek-frontend:v1.0 --push .

cd ..

echo.
echo ✅ Images built and pushed successfully!
echo 🐳 Backend: %DOCKER_USERNAME%/deepseek-api:latest
echo 🐳 Frontend: %DOCKER_USERNAME%/deepseek-frontend:latest
echo.
echo 📋 Next steps:
echo 1. Update portainer-stack-prebuilt.yml with your username
echo 2. Upload the stack file to Portainer
echo 3. Deploy the stack
echo.
pause 