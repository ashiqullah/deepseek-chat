# Raspberry Pi 4 Deployment Guide

This guide will help you deploy the DeepSeek Chat application on your Raspberry Pi 4 with Portainer.

## Prerequisites

- Raspberry Pi 4 with Raspberry Pi OS installed
- At least 4GB RAM (8GB recommended)
- 32GB+ microSD card (Class 10 or better)
- Internet connection
- SSH access enabled

## Method 1: Using Portainer (Recommended)

### Step 1: Access Portainer
1. Open your web browser and go to `http://[PI_IP]:9000`
2. Log in to your Portainer instance

### Step 2: Create a New Stack
1. Navigate to **Stacks** in the left sidebar
2. Click **Add stack**
3. Give it a name: `deepseek-chat`

### Step 3: Deploy the Stack
Choose one of these options:

#### Option A: Upload Compose File
1. Select **Upload** tab
2. Upload the `portainer-stack.yml` file from this project
3. **Important**: Update the DeepSeek API key in the environment variables
4. Click **Deploy the stack**

#### Option B: Git Repository (if your code is in Git)
1. Select **Repository** tab
2. Repository URL: `https://github.com/your-username/your-repo`
3. Compose path: `portainer-stack.yml`
4. Click **Deploy the stack**

#### Option C: Web Editor
1. Select **Web editor** tab
2. Copy and paste the contents of `portainer-stack.yml`
3. **Important**: Update the DeepSeek API key in the environment variables:
   ```yaml
   - DeepSeek__ApiKey=your-actual-deepseek-api-key-here
   ```
4. Click **Deploy the stack**

### Step 4: Monitor Deployment
1. Go to **Containers** to see the deployment progress
2. Wait for both containers to show "running" status
3. Check logs if any container fails to start

### Step 5: Access Your Application
- **Frontend**: `http://[PI_IP]:4200`
- **Backend API**: `http://[PI_IP]:5000`
- **API Documentation**: `http://[PI_IP]:5000/swagger`

## Method 2: SSH Deployment Script

### Step 1: Prepare Your Local Machine
```bash
# Make the script executable
chmod +x deploy-to-pi.sh

# Update the script with your Pi's IP and username
nano deploy-to-pi.sh
```

### Step 2: Deploy to Pi
```bash
# Replace with your Pi's actual IP and username
./deploy-to-pi.sh 192.168.1.100 pi
```

### Step 3: Monitor Deployment
```bash
# SSH into your Pi and check container status
ssh pi@192.168.1.100
cd ~/deepseek-chat
docker-compose -f docker-compose.pi.yml ps
docker-compose -f docker-compose.pi.yml logs -f
```

## Method 3: Manual Docker Compose

### Step 1: Copy Files to Pi
```bash
# Copy project files
scp -r . pi@192.168.1.100:~/deepseek-chat/
```

### Step 2: SSH into Pi and Deploy
```bash
ssh pi@192.168.1.100
cd ~/deepseek-chat

# Build and start services
docker-compose -f docker-compose.pi.yml up --build -d

# Check status
docker-compose -f docker-compose.pi.yml ps
```

## Troubleshooting

### Common Issues

1. **Out of Memory**
   - The Pi 4 needs at least 4GB RAM
   - Close other applications
   - Increase swap space:
     ```bash
     sudo dphys-swapfile swapoff
     sudo nano /etc/dphys-swapfile
     # Set CONF_SWAPSIZE=2048
     sudo dphys-swapfile setup
     sudo dphys-swapfile swapon
     ```

2. **Slow Build Times**
   - Building on Pi can take 15-30 minutes
   - Consider building on a more powerful machine and pushing to Docker registry

3. **ARM64 Compatibility Issues**
   - All base images are ARM64 compatible
   - If you encounter issues, check Docker logs:
     ```bash
     docker logs deepseek-api
     docker logs deepseek-frontend
     ```

4. **Network Issues**
   - Ensure ports 4200 and 5000 are not blocked by firewall
   - Check if other services are using these ports:
     ```bash
     sudo netstat -tulpn | grep :4200
     sudo netstat -tulpn | grep :5000
     ```

### Performance Optimization

1. **Enable GPU Memory Split** (if using desktop version):
   ```bash
   sudo raspi-config
   # Advanced Options > Memory Split > 16
   ```

2. **Overclock Pi 4** (optional, ensure good cooling):
   ```bash
   sudo nano /boot/config.txt
   # Add:
   # arm_freq=2000
   # gpu_freq=750
   ```

3. **Use SSD instead of SD card** for better I/O performance

### Monitoring

You can monitor your application through Portainer:

1. **Container Stats**: Real-time CPU, memory, network usage
2. **Logs**: View application logs in real-time
3. **Console**: Access container shells for debugging

### Backup and Updates

1. **Backup Configuration**:
   ```bash
   # Backup your custom appsettings.json
   cp ~/deepseek-chat/backend/appsettings.json ~/deepseek-chat-backup.json
   ```

2. **Update Application**:
   ```bash
   cd ~/deepseek-chat
   git pull  # if using Git
   docker-compose -f docker-compose.pi.yml down
   docker-compose -f docker-compose.pi.yml build --no-cache
   docker-compose -f docker-compose.pi.yml up -d
   ```

## Security Considerations

1. **Change Default Ports** (optional):
   - Map to different external ports in docker-compose file
   - Example: `"8080:80"` instead of `"4200:80"`

2. **Enable HTTPS** (production):
   - Configure reverse proxy (nginx, traefik)
   - Use Let's Encrypt certificates

3. **Firewall Configuration**:
   ```bash
   sudo ufw enable
   sudo ufw allow 22    # SSH
   sudo ufw allow 4200  # Frontend
   sudo ufw allow 5000  # Backend API
   sudo ufw allow 9000  # Portainer (if needed)
   ```

4. **Regular Updates**:
   ```bash
   sudo apt update && sudo apt upgrade -y
   docker system prune -f  # Clean up unused images
   ```

## Support

If you encounter issues:

1. Check container logs in Portainer
2. Verify network connectivity between containers
3. Ensure sufficient resources (RAM/disk space)
4. Check Raspberry Pi system logs: `sudo journalctl -f` 