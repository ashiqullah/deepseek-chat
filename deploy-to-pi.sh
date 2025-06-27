#!/bin/bash

# DeepSeek Chat Application - Raspberry Pi Deployment Script
# Usage: ./deploy-to-pi.sh [raspberry-pi-ip] [username]

PI_IP=${1:-"192.168.1.100"}  # Default IP, change as needed
PI_USER=${2:-"pi"}           # Default username

echo "🚀 Deploying DeepSeek Chat to Raspberry Pi"
echo "========================================"
echo "Target: $PI_USER@$PI_IP"
echo ""

# Check if SSH connection works
echo "🔍 Testing SSH connection..."
if ! ssh -o ConnectTimeout=5 $PI_USER@$PI_IP "echo 'SSH connection successful'"; then
    echo "❌ SSH connection failed. Please check:"
    echo "   - IP address: $PI_IP"
    echo "   - Username: $PI_USER"
    echo "   - SSH key or password authentication"
    exit 1
fi

# Create project directory on Pi
echo "📁 Creating project directory..."
ssh $PI_USER@$PI_IP "mkdir -p ~/deepseek-chat"

# Copy project files
echo "📦 Copying project files..."
rsync -avz --progress \
    --exclude='node_modules' \
    --exclude='bin' \
    --exclude='obj' \
    --exclude='.git' \
    ./ $PI_USER@$PI_IP:~/deepseek-chat/

# Build and run containers on Pi
echo "🔨 Building and starting containers on Raspberry Pi..."
ssh $PI_USER@$PI_IP << 'EOF'
    cd ~/deepseek-chat
    
    # Update system packages
    sudo apt update && sudo apt upgrade -y
    
    # Install Docker if not already installed
    if ! command -v docker &> /dev/null; then
        echo "Installing Docker..."
        curl -fsSL https://get.docker.com -o get-docker.sh
        sh get-docker.sh
        sudo usermod -aG docker $USER
        echo "Please log out and back in to use Docker without sudo"
    fi
    
    # Install Docker Compose if not already installed
    if ! command -v docker-compose &> /dev/null; then
        echo "Installing Docker Compose..."
        sudo apt install -y docker-compose
    fi
    
    # Build and start services
    docker-compose -f docker-compose.pi.yml down
    docker-compose -f docker-compose.pi.yml build --no-cache
    docker-compose -f docker-compose.pi.yml up -d
    
    # Show status
    docker-compose -f docker-compose.pi.yml ps
EOF

echo ""
echo "✅ Deployment complete!"
echo "🌐 Frontend: http://$PI_IP:4200"
echo "🔧 Backend API: http://$PI_IP:5000"
echo "📊 Portainer: http://$PI_IP:9000 (if installed)"
echo ""
echo "📝 To check logs:"
echo "   ssh $PI_USER@$PI_IP 'cd ~/deepseek-chat && docker-compose -f docker-compose.pi.yml logs -f'" 