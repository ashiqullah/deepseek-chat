@echo off
echo 🔧 Fixing Git and Docker Issues
echo ==============================
echo.

echo 📋 Step 1: Fixing Git Repository
echo --------------------------------

REM Check Git status
echo Checking Git status...
git status

echo.
echo Adding all files to Git...
git add .

echo.
echo Creating initial commit...
git commit -m "Initial commit: DeepSeek Chat application with ASP.NET Core API and Angular frontend"

echo.
echo Setting main branch...
git branch -M main

echo.
echo 📋 Step 2: Testing Docker Build
echo ------------------------------

echo Testing frontend Docker build...
cd frontend
docker build -t deepseek-frontend-test .
if %errorLevel% neq 0 (
    echo ❌ Frontend Docker build failed
    echo.
    echo 🔧 Common solutions:
    echo 1. Make sure Docker is running
    echo 2. Check internet connection for npm packages
    echo 3. Try building locally first: cd frontend && npm install && npm run build
    echo.
    cd ..
    pause
    exit /b 1
) else (
    echo ✅ Frontend Docker build successful
)

cd ..

echo.
echo Testing backend Docker build...
cd backend
docker build -t deepseek-backend-test .
if %errorLevel% neq 0 (
    echo ❌ Backend Docker build failed
    echo.
    echo 🔧 Common solutions:
    echo 1. Make sure Docker is running
    echo 2. Check internet connection for NuGet packages
    echo 3. Try building locally first: cd backend && dotnet build
    echo.
    cd ..
    pause
    exit /b 1
) else (
    echo ✅ Backend Docker build successful
)

cd ..

echo.
echo 📋 Step 3: Push to GitHub
echo ------------------------

echo Pushing to GitHub...
git push -u origin main
if %errorLevel% neq 0 (
    echo ❌ Failed to push to GitHub
    echo.
    echo 🔧 Manual steps:
    echo 1. Make sure you created the repository on GitHub
    echo 2. Check your GitHub username in the remote URL
    echo 3. Use Personal Access Token when prompted for password
    echo 4. Repository URL: https://github.com/ashiqullah/deepseek-chat
    echo.
    pause
    exit /b 1
) else (
    echo ✅ Successfully pushed to GitHub
)

echo.
echo 🎉 All issues fixed!
echo ===================
echo.
echo ✅ Git repository initialized and pushed
echo ✅ Docker builds working
echo ✅ Ready for deployment
echo.
echo 🌐 Your repository: https://github.com/ashiqullah/deepseek-chat
echo.
pause 