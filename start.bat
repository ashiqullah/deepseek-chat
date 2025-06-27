@echo off
echo 🚀 Starting DeepSeek Chat Application...
echo =================================

REM Check if Docker is running
docker info >nul 2>&1
if %errorLevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker first.
    pause
    exit /b 1
)

REM Check if docker-compose is available
docker-compose --version >nul 2>&1
if %errorLevel% neq 0 (
    echo ❌ Docker Compose is not installed. Please install Docker Compose first.
    pause
    exit /b 1
)

echo 📦 Building and starting containers...
docker-compose up --build -d

echo ⏳ Waiting for services to start...
timeout /t 10 /nobreak >nul

echo 🔍 Checking service status...
docker-compose ps

echo.
echo ✅ Application is ready!
echo 🌐 Frontend: http://localhost:4200
echo 🔧 Backend API: http://localhost:5000
echo 📚 API Documentation: http://localhost:5000/swagger
echo.
echo 📝 To view logs:
echo    docker-compose logs -f deepseek-api
echo    docker-compose logs -f deepseek-frontend
echo.
echo 🛑 To stop the application:
echo    docker-compose down
echo.
pause 