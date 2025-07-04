#!/bin/bash

# DeepSeek Chat - Model Provider Setup Script
# This script helps set up both DeepSeek API and local model providers

set -e

echo "🚀 DeepSeek Chat - Model Provider Setup"
echo "======================================"

# Function to check if Docker is running
check_docker() {
    if ! docker --version >/dev/null 2>&1; then
        echo "❌ Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! docker info >/dev/null 2>&1; then
        echo "❌ Docker is not running. Please start Docker first."
        exit 1
    fi
    
    echo "✅ Docker is running"
}

# Function to check if Docker Compose is available
check_docker_compose() {
    if docker compose version >/dev/null 2>&1; then
        DOCKER_COMPOSE="docker compose"
    elif docker-compose --version >/dev/null 2>&1; then
        DOCKER_COMPOSE="docker-compose"
    else
        echo "❌ Docker Compose is not installed. Please install Docker Compose first."
        exit 1
    fi
    
    echo "✅ Docker Compose is available"
}

# Function to prompt for API key
setup_api_key() {
    echo ""
    echo "🔑 DeepSeek API Configuration"
    echo "You need a DeepSeek API key to use the cloud service."
    echo "Get one at: https://platform.deepseek.com/"
    echo ""
    
    read -p "Enter your DeepSeek API key (or press Enter to skip): " api_key
    
    if [ -n "$api_key" ]; then
        # Update the appsettings.json file
        if [ -f "backend/appsettings.json" ]; then
            # Create a backup
            cp backend/appsettings.json backend/appsettings.json.backup
            
            # Update the API key using sed
            sed -i.tmp "s/\"ApiKey\": \".*\"/\"ApiKey\": \"$api_key\"/" backend/appsettings.json
            rm backend/appsettings.json.tmp 2>/dev/null || true
            
            echo "✅ API key updated in backend/appsettings.json"
        else
            echo "⚠️  backend/appsettings.json not found. Please update it manually."
        fi
    else
        echo "⚠️  Skipping API key setup. You can update it later in backend/appsettings.json"
    fi
}

# Function to set up local model
setup_local_model() {
    echo ""
    echo "🤖 Local Model Setup"
    echo "Do you want to set up a local model using Ollama?"
    echo "This requires at least 8GB of RAM and will download a large model."
    echo ""
    
    read -p "Set up local model? (y/N): " setup_local
    
    if [[ $setup_local =~ ^[Yy]$ ]]; then
        echo "✅ Local model will be set up"
        return 0
    else
        echo "⚠️  Skipping local model setup"
        
        # Comment out Ollama service in docker-compose.yml
        if [ -f "docker-compose.yml" ]; then
            echo "🔧 Commenting out Ollama service in docker-compose.yml..."
            sed -i.tmp '/# Optional: Ollama service/,/^$/s/^/# /' docker-compose.yml
            sed -i.tmp 's/depends_on:/# depends_on:/' docker-compose.yml
            sed -i.tmp 's/- ollama/# - ollama/' docker-compose.yml
            rm docker-compose.yml.tmp 2>/dev/null || true
            echo "✅ Ollama service commented out"
        fi
        
        return 1
    fi
}

# Function to start services
start_services() {
    echo ""
    echo "🐳 Starting Docker services..."
    
    $DOCKER_COMPOSE up -d
    
    echo "✅ Services started"
    echo ""
    echo "🌐 Application URLs:"
    echo "   Frontend: http://localhost:4200"
    echo "   Backend:  http://localhost:5000"
    if [ "$1" = "with_ollama" ]; then
        echo "   Ollama:   http://localhost:11434"
    fi
}

# Function to setup Ollama model
setup_ollama_model() {
    echo ""
    echo "📦 Setting up Ollama model..."
    echo "This may take several minutes depending on your internet connection."
    
    # Wait for Ollama to be ready
    echo "⏳ Waiting for Ollama to start..."
    sleep 10
    
    # Check if Ollama is running
    for i in {1..30}; do
        if docker exec deepseek-ollama ollama list >/dev/null 2>&1; then
            break
        fi
        echo "   Still waiting for Ollama... ($i/30)"
        sleep 2
    done
    
    # Show available models and let user choose
    echo ""
    echo "Available DeepSeek models:"
    echo "1. deepseek-r1:1.5b  (Smaller, faster, ~2GB)"
    echo "2. deepseek-r1:7b    (Balanced, ~4GB)"
    echo "3. deepseek-r1:latest (Best quality, ~8GB)"
    echo ""
    
    read -p "Choose model (1-3, default: 1): " model_choice
    
    case $model_choice in
        2)
            model="deepseek-r1:7b"
            ;;
        3)
            model="deepseek-r1:latest"
            ;;
        *)
            model="deepseek-r1:1.5b"
            ;;
    esac
    
    echo "📥 Pulling $model..."
    if docker exec deepseek-ollama ollama pull $model; then
        echo "✅ Model $model downloaded successfully"
        
        # Update the configuration if needed
        if [ "$model" != "deepseek-r1:latest" ]; then
            sed -i.tmp "s/\"LocalModelId\": \".*\"/\"LocalModelId\": \"$model\"/" backend/appsettings.json
            rm backend/appsettings.json.tmp 2>/dev/null || true
            echo "✅ Configuration updated for $model"
        fi
    else
        echo "❌ Failed to download model. You can try again later with:"
        echo "   docker exec -it deepseek-ollama ollama pull $model"
    fi
}

# Function to show final instructions
show_instructions() {
    echo ""
    echo "🎉 Setup Complete!"
    echo "=================="
    echo ""
    echo "Next steps:"
    echo "1. Open your browser and go to http://localhost:4200"
    echo "2. Use the dropdown in the header to select your preferred model provider"
    echo "3. Start chatting!"
    echo ""
    echo "Troubleshooting:"
    echo "- Check service status: $DOCKER_COMPOSE ps"
    echo "- View logs: $DOCKER_COMPOSE logs"
    echo "- Restart services: $DOCKER_COMPOSE restart"
    echo ""
    echo "For detailed documentation, see MODEL_PROVIDER_SETUP.md"
}

# Main execution
main() {
    check_docker
    check_docker_compose
    
    setup_api_key
    
    local_model_enabled=false
    if setup_local_model; then
        local_model_enabled=true
        start_services "with_ollama"
        setup_ollama_model
    else
        start_services
    fi
    
    show_instructions
}

# Run main function
main "$@" 