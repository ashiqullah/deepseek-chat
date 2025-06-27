@echo off
echo 🚀 Setting up GitHub Repository for DeepSeek Chat
echo =============================================
echo.

REM Check if Git is installed
git --version >nul 2>&1
if %errorLevel% neq 0 (
    echo ❌ Git is not installed. Please install Git first from https://git-scm.com/
    pause
    exit /b 1
)

echo ✅ Git is installed
echo.

REM Get user input
set /p GITHUB_USERNAME="Enter your GitHub username: "
set /p REPO_NAME="Enter repository name (default: deepseek-chat): "
if "%REPO_NAME%"=="" set REPO_NAME=deepseek-chat

echo.
echo 📋 Configuration:
echo    GitHub Username: %GITHUB_USERNAME%
echo    Repository Name: %REPO_NAME%
echo.

REM Check if already a git repository
if exist ".git" (
    echo ⚠️  This directory is already a Git repository
    echo.
) else (
    echo 📁 Initializing Git repository...
    git init
    if %errorLevel% neq 0 (
        echo ❌ Failed to initialize Git repository
        pause
        exit /b 1
    )
)

REM Add files to Git
echo 📦 Adding files to Git...
git add .
if %errorLevel% neq 0 (
    echo ❌ Failed to add files to Git
    pause
    exit /b 1
)

REM Create initial commit
echo 💾 Creating initial commit...
git commit -m "Initial commit: DeepSeek Chat application with ASP.NET Core API and Angular frontend"
if %errorLevel% neq 0 (
    echo ❌ Failed to create commit. Files may already be committed.
)

REM Rename branch to main if needed
echo 🔄 Setting main branch...
git branch -M main

REM Add remote origin
echo 🔗 Adding GitHub remote...
git remote add origin https://github.com/%GITHUB_USERNAME%/%REPO_NAME%.git 2>nul
if %errorLevel% neq 0 (
    echo ⚠️  Remote origin already exists, updating...
    git remote set-url origin https://github.com/%GITHUB_USERNAME%/%REPO_NAME%.git
)

echo.
echo 🌐 IMPORTANT: Create GitHub Repository
echo =====================================
echo 1. Go to https://github.com/new
echo 2. Repository name: %REPO_NAME%
echo 3. Description: Chat application using DeepSeek AI with ASP.NET Core and Angular
echo 4. Choose Public or Private
echo 5. DO NOT check "Add a README file"
echo 6. Click "Create repository"
echo.

set /p CONTINUE="Press Enter after creating the GitHub repository..."

echo 🚀 Pushing to GitHub...
git push -u origin main
if %errorLevel% neq 0 (
    echo.
    echo ❌ Failed to push to GitHub. This might be due to:
    echo    1. Repository doesn't exist on GitHub
    echo    2. Authentication issues
    echo    3. Incorrect repository name or username
    echo.
    echo 🔧 Manual steps:
    echo    1. Check repository exists: https://github.com/%GITHUB_USERNAME%/%REPO_NAME%
    echo    2. Try: git push -u origin main
    echo    3. Use Personal Access Token if prompted for password
    echo.
    pause
    exit /b 1
)

echo.
echo ✅ Success! Your repository is now on GitHub
echo 🌐 Repository URL: https://github.com/%GITHUB_USERNAME%/%REPO_NAME%
echo.
echo 📋 Next steps:
echo 1. Visit your repository: https://github.com/%GITHUB_USERNAME%/%REPO_NAME%
echo 2. Update README.md if needed
echo 3. Consider making repository private if it contains sensitive information
echo.
echo 🔄 Future updates:
echo    git add .
echo    git commit -m "Your commit message"
echo    git push
echo.
pause 