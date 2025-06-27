# GitHub Setup Guide for DeepSeek Chat

This guide will help you commit your DeepSeek Chat application to GitHub.

## Prerequisites

- Git installed on your computer
- GitHub account

## Step 1: Install Git (if not already installed)

### Windows:
1. Download from [git-scm.com](https://git-scm.com/)
2. Install with default settings
3. Open PowerShell or Command Prompt

### Check if Git is installed:
```bash
git --version
```

## Step 2: Configure Git (first time only)

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

## Step 3: Initialize Git Repository

In your project folder (D:\LLM\Docker):

```bash
# Initialize Git repository
git init

# Add all files to staging
git add .

# Create first commit
git commit -m "Initial commit: DeepSeek Chat application with ASP.NET Core API and Angular frontend"
```

## Step 4: Create GitHub Repository

### Option A: Using GitHub Website (Easier)
1. Go to [github.com](https://github.com)
2. Click the "+" icon → "New repository"
3. Repository name: `deepseek-chat` (or your preferred name)
4. Description: `Chat application using DeepSeek AI with ASP.NET Core and Angular`
5. Choose "Public" or "Private"
6. **DO NOT** check "Add a README file" (since you already have files)
7. Click "Create repository"

### Option B: Using GitHub CLI (if installed)
```bash
gh repo create deepseek-chat --public --description "Chat application using DeepSeek AI"
```

## Step 5: Connect Local Repository to GitHub

After creating the GitHub repository, you'll see instructions. Use these commands:

```bash
# Add remote origin (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/deepseek-chat.git

# Verify remote
git remote -v

# Push to GitHub
git push -u origin main
```

If you get an error about branch name, use:
```bash
# Rename branch to main if needed
git branch -M main
git push -u origin main
```

## Step 6: Authentication

### If prompted for credentials:

#### Option A: Personal Access Token (Recommended)
1. Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Generate new token with "repo" scope
3. Copy the token
4. Use token as password when prompted

#### Option B: GitHub Desktop
- Download and install GitHub Desktop
- Sign in and clone/push through the GUI

## Step 7: Verify Upload

1. Go to your GitHub repository URL
2. You should see all your files:
   - `backend/` folder with ASP.NET Core code
   - `frontend/` folder with Angular code
   - `docker-compose.yml`
   - `README.md`
   - All other files

## Step 8: Update README (Optional)

Your repository should now have a proper README. You can edit it on GitHub or locally:

```bash
# Edit README.md file, then:
git add README.md
git commit -m "Update README with project description"
git push
```

## Future Updates

When you make changes to your code:

```bash
# Add changes
git add .

# Commit with descriptive message
git commit -m "Add new feature: improved error handling"

# Push to GitHub
git push
```

## Common Issues & Solutions

### Issue: "Repository not found"
**Solution**: Check the remote URL
```bash
git remote -v
git remote set-url origin https://github.com/YOUR_USERNAME/deepseek-chat.git
```

### Issue: "Authentication failed"
**Solution**: Use personal access token instead of password

### Issue: "Permission denied"
**Solution**: Check repository is public or you have access

### Issue: "Branch 'main' not found"
**Solution**: 
```bash
git branch -M main
git push -u origin main
```

## Useful Git Commands

```bash
# Check status
git status

# View commit history
git log --oneline

# Create new branch
git checkout -b feature-name

# Switch branches
git checkout main

# Merge branch
git merge feature-name

# Pull latest changes
git pull origin main
```

## Repository Structure

After upload, your GitHub repository will look like:

```
deepseek-chat/
├── backend/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── DeepSeekChatApi.csproj
│   └── Dockerfile
├── frontend/
│   ├── src/
│   ├── package.json
│   ├── angular.json
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
├── docker-compose.pi.yml
├── portainer-stack.yml
├── portainer-stack-prebuilt.yml
├── build-and-push.sh
├── build-and-push.bat
├── deploy-to-pi.sh
├── README.md
├── .gitignore
└── raspberry-pi-setup.md
```

## Security Note

The `.gitignore` file ensures sensitive files are not uploaded:
- `node_modules/`
- `bin/` and `obj/` folders
- Environment files with secrets
- IDE-specific files

Your DeepSeek API key in `appsettings.json` will be uploaded. Consider using environment variables for production. 