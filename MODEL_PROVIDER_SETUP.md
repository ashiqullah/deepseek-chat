# Model Provider Setup Guide

This application now supports two model providers:
1. **DeepSeek API** - Cloud-based API service
2. **Local Model** - Self-hosted model using Ollama

## Quick Start

### Option 1: DeepSeek API Only
If you only want to use the DeepSeek API:

1. Update your API key in `backend/appsettings.json`:
```json
{
  "DeepSeek": {
    "ApiKey": "your-actual-api-key-here"
  }
}
```

2. Comment out the Ollama service in `docker-compose.yml`:
```yaml
# ollama:
#   image: ollama/ollama:latest
#   ... (comment out the entire service)
```

3. Remove the dependency from the API service:
```yaml
deepseek-api:
  # depends_on:
  #   - ollama
```

### Option 2: Both DeepSeek API and Local Model

#### Prerequisites
- Docker and Docker Compose installed
- At least 8GB RAM for local model
- (Optional) NVIDIA GPU with Docker GPU support for better performance

#### Setup Steps

1. **Start the services:**
```bash
docker-compose up -d
```

2. **Pull the DeepSeek model in Ollama:**
```bash
# Wait for Ollama to start, then pull the model
docker exec -it deepseek-ollama ollama pull deepseek-r1:latest
```

Alternative models you can use:
```bash
# Other DeepSeek variants
docker exec -it deepseek-ollama ollama pull deepseek-coder:latest
docker exec -it deepseek-ollama ollama pull deepseek-r1:1.5b
docker exec -it deepseek-ollama ollama pull deepseek-r1:7b
```

3. **Verify the setup:**
- Frontend: http://localhost:4200
- Backend API: http://localhost:5000
- Ollama API: http://localhost:11434

## Configuration

### Backend Configuration (`backend/appsettings.json`)
```json
{
  "DeepSeek": {
    "ModelId": "deepseek-chat",
    "ApiKey": "your-deepseek-api-key",
    "Endpoint": "https://api.deepseek.com/v1/",
    "ServiceId": "deepseek",
    "LocalModelEndpoint": "http://localhost:11434/v1/",
    "LocalModelId": "deepseek-r1:latest",
    "LocalServiceId": "local-deepseek"
  }
}
```

### Docker Environment Variables
You can override settings using environment variables in `docker-compose.yml`:

```yaml
environment:
  # DeepSeek API
  - DeepSeek__ApiKey=your-api-key
  - DeepSeek__ModelId=deepseek-chat
  - DeepSeek__Endpoint=https://api.deepseek.com/v1/
  
  # Local Model
  - DeepSeek__LocalModelEndpoint=http://ollama:11434/v1/
  - DeepSeek__LocalModelId=deepseek-r1:latest
```

## Usage

1. **Open the application** at http://localhost:4200
2. **Select your preferred provider** from the dropdown in the header
3. **Start chatting!** The app will automatically use your selected provider

### Provider Status Indicators
- 🟢 **Available** - Provider is ready to use
- 🟠 **Unavailable** - Provider is not accessible
- The dropdown will show "(Unavailable)" for providers that can't be reached

## Troubleshooting

### Local Model Issues

**Problem**: Local model shows as unavailable
**Solutions**:
1. Check if Ollama container is running:
   ```bash
   docker ps | grep ollama
   ```
2. Verify the model is pulled:
   ```bash
   docker exec -it deepseek-ollama ollama list
   ```
3. Check Ollama logs:
   ```bash
   docker logs deepseek-ollama
   ```

**Problem**: Out of memory errors
**Solutions**:
1. Use a smaller model variant:
   ```bash
   docker exec -it deepseek-ollama ollama pull deepseek-r1:1.5b
   ```
2. Update the model ID in configuration
3. Increase Docker memory limits

### API Issues

**Problem**: DeepSeek API shows as unavailable
**Solutions**:
1. Verify your API key is correct
2. Check your internet connection
3. Verify API quota/credits
4. Check the DeepSeek API status

### General Issues

**Problem**: Application won't start
**Solutions**:
1. Check Docker logs:
   ```bash
   docker-compose logs
   ```
2. Verify all required ports are available (4200, 5000, 11434)
3. Restart the services:
   ```bash
   docker-compose down && docker-compose up -d
   ```

## Performance Tips

### For Local Model
1. **Use GPU acceleration** (if available):
   - Uncomment GPU configuration in `docker-compose.yml`
   - Install NVIDIA Container Toolkit
   
2. **Choose appropriate model size**:
   - `deepseek-r1:1.5b` - Faster, less memory, lower quality
   - `deepseek-r1:7b` - Balanced performance and quality
   - `deepseek-r1:latest` - Best quality, more resources needed

3. **Allocate sufficient resources**:
   ```yaml
   deploy:
     resources:
       limits:
         memory: 8G
       reservations:
         memory: 4G
   ```

### For API Usage
1. **Monitor API usage** to avoid quota limits
2. **Implement rate limiting** if needed
3. **Cache responses** for repeated queries (future enhancement)

## Model Comparison

| Feature | DeepSeek API | Local Model |
|---------|--------------|-------------|
| Setup Complexity | Easy | Moderate |
| Response Speed | Fast | Variable |
| Privacy | Data sent to API | Fully private |
| Cost | Pay per use | One-time setup |
| Reliability | High | Depends on hardware |
| Model Updates | Automatic | Manual |
| Internet Required | Yes | No (after setup) |

## Development

### Adding New Providers
To add support for additional providers:

1. Update the `ModelProvider` enum in both backend and frontend
2. Add configuration settings in `DeepSeekSettings.cs`
3. Implement provider logic in `DeepSeekChatService.cs`
4. Update the UI to include the new provider option

### Local Development
For local development without Docker:

1. **Backend**: Run with `dotnet run` in `/backend`
2. **Frontend**: Run with `ng serve` in `/frontend`  
3. **Local Model**: Install Ollama locally and run `ollama serve`

## Security Considerations

1. **API Keys**: Never commit API keys to version control
2. **Environment Variables**: Use Docker secrets in production
3. **Network Security**: Configure firewalls appropriately
4. **Local Model**: Ensure Ollama is not exposed to public internet

## Support

- Check the logs for detailed error messages
- Verify all services are running and healthy
- Test individual components separately
- Check network connectivity between services

For additional help, please check the main README.md or create an issue in the repository. 