# Direct File Upload Guide (No Git Repository Required)

This guide shows you how to deploy your DeepSeek Chat application to Raspberry Pi without using a Git repository.

## Method 1: Pre-built Images via Docker Hub (Recommended)

### Step 1: Create Docker Hub Account
1. Go to [Docker Hub](https://hub.docker.com/)
2. Create a free account
3. Note your username

### Step 2: Login to Docker Hub
```bash
docker login
# Enter your Docker Hub username and password
```

### Step 3: Build and Push Images

**On Windows:**
```cmd
# Run the batch script
build-and-push.bat [your-dockerhub-username]
```

**On Linux/Mac:**
```bash
# Make script executable and run
chmod +x build-and-push.sh
./build-and-push.sh [your-dockerhub-username]
```

### Step 4: Update Portainer Stack File
1. Open `portainer-stack-prebuilt.yml`
2. Replace `yourusername` with your actual Docker Hub username
3. Update your DeepSeek API key

### Step 5: Deploy in Portainer
1. Access Portainer: `http://[PI_IP]:9000`
2. Go to **Stacks** → **Add stack**
3. Name: `deepseek-chat`
4. Select **Upload** tab
5. Upload `portainer-stack-prebuilt.yml`
6. Click **Deploy the stack**

---

## Method 2: Direct File Copy to Raspberry Pi

### Step 1: Copy Files to Pi
```bash
# Copy entire project to Pi
scp -r . pi@[PI_IP]:~/deepseek-chat/

# Or use WinSCP on Windows to copy files
```

### Step 2: SSH into Pi and Build
```bash
ssh pi@[PI_IP]
cd ~/deepseek-chat

# Build and start services
docker-compose -f docker-compose.pi.yml up --build -d
```

---

## Method 3: Manual Docker Commands

### Step 1: Copy Files to Pi
```bash
scp -r . pi@[PI_IP]:~/deepseek-chat/
```

### Step 2: Build Images on Pi
```bash
ssh pi@[PI_IP]
cd ~/deepseek-chat

# Build backend image
cd backend
docker build -t deepseek-api:local .

# Build frontend image
cd ../frontend
docker build -t deepseek-frontend:local .
```

### Step 3: Create and Run Containers
```bash
# Create network
docker network create deepseek-network

# Run backend
docker run -d \
  --name deepseek-api \
  --network deepseek-network \
  -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:80 \
  deepseek-api:local

# Run frontend
docker run -d \
  --name deepseek-frontend \
  --network deepseek-network \
  -p 4200:80 \
  deepseek-frontend:local
```

---

## Method 4: Using Portainer Build Feature

### Step 1: Create Local Stack without Build Context
1. In Portainer, create a new stack
2. Use this simplified YAML:

```yaml
version: '3.8'

services:
  deepseek-api:
    image: deepseek-api:local
    container_name: deepseek-api
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - DeepSeek__ModelId=deepseek-chat
      - DeepSeek__ApiKey=sk-4de054d548984d8b97b5f5e7d630b6a2
      - DeepSeek__Endpoint=https://api.deepseek.com/v1/
      - DeepSeek__ServiceId=deepseek
    networks:
      - deepseek-network
    restart: unless-stopped

  deepseek-frontend:
    image: deepseek-frontend:local
    container_name: deepseek-frontend
    ports:
      - "4200:80"
    depends_on:
      - deepseek-api
    networks:
      - deepseek-network
    restart: unless-stopped

networks:
  deepseek-network:
    driver: bridge
```

### Step 2: Build Images First
Before deploying the stack, SSH into Pi and build images:
```bash
ssh pi@[PI_IP]
cd ~/deepseek-chat
docker build -t deepseek-api:local backend/
docker build -t deepseek-frontend:local frontend/
```

---

## Comparison of Methods

| Method | Pros | Cons | Best For |
|--------|------|------|----------|
| **Docker Hub** | ✅ Fast deployment<br>✅ Works anywhere<br>✅ Version control | ❌ Images are public<br>❌ Requires Docker Hub account | **Recommended** |
| **Direct Copy** | ✅ No external dependencies<br>✅ Private | ❌ Slower deployment<br>❌ Manual process | Small projects |
| **Manual Docker** | ✅ Full control<br>✅ Educational | ❌ Complex setup<br>❌ No orchestration | Learning/testing |
| **Portainer Build** | ✅ UI-based<br>✅ Integrated | ❌ Still need file transfer<br>❌ Two-step process | UI preference |

---

## Troubleshooting

### Issue: "Image not found"
**Solution**: Make sure you've built or pushed the images correctly
```bash
# Check available images
docker images

# Check if images were pushed to Docker Hub
docker search yourusername/deepseek
```

### Issue: "Build failed on ARM64"
**Solution**: Some packages might not support ARM64. Check Docker logs:
```bash
docker logs deepseek-api
docker logs deepseek-frontend
```

### Issue: "Out of space during build"
**Solution**: Clean up Docker system:
```bash
docker system prune -a --volumes
```

### Issue: "Multi-platform build not working"
**Solution**: Enable Docker BuildKit:
```bash
# On Linux/Mac
export DOCKER_BUILDKIT=1

# On Windows
set DOCKER_BUILDKIT=1

# Install buildx if missing
docker buildx install
```

---

## Security Notes

1. **Docker Hub Images**: Free accounts have public repositories
2. **API Keys**: Never commit API keys to public repositories
3. **Environment Variables**: Always use environment variables for secrets
4. **Network Security**: Consider using HTTPS in production

---

## Quick Start Summary

**Easiest method** (Docker Hub):
1. `docker login`
2. `./build-and-push.sh yourusername`
3. Update `portainer-stack-prebuilt.yml` with your username
4. Upload to Portainer and deploy

**Fastest method** (Direct copy):
1. `scp -r . pi@[PI_IP]:~/deepseek-chat/`
2. `ssh pi@[PI_IP]`
3. `cd ~/deepseek-chat && docker-compose -f docker-compose.pi.yml up --build -d` 