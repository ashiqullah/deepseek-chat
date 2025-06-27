# DeepSeek Chat Application

A modern web-based chat application built with ASP.NET Core and Angular that integrates with the DeepSeek AI API using Microsoft Semantic Kernel.

## Features

- **Real-time Chat Interface**: Modern, responsive chat UI with typing indicators
- **DeepSeek AI Integration**: Powered by DeepSeek's advanced language model
- **Conversation Management**: Persistent chat history with conversation tracking
- **Docker Support**: Containerized deployment with Docker Compose
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Connection Status**: Real-time API connection monitoring

## Architecture

- **Backend**: ASP.NET Core 8.0 Web API with Microsoft Semantic Kernel
- **Frontend**: Angular 17 with TypeScript
- **Containerization**: Docker with multi-stage builds
- **AI Integration**: DeepSeek API via OpenAI-compatible interface

## Prerequisites

- Docker and Docker Compose
- DeepSeek API Key (get one from [DeepSeek Platform](https://platform.deepseek.com/))

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Docker
   ```

2. **Configure API Key**
   
   Update the API key in `backend/DeepSeekChatApi/appsettings.json`:
   ```json
   {
     "DeepSeek": {
       "ApiKey": "your-deepseek-api-key-here"
     }
   }
   ```

3. **Run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

4. **Access the Application**
   - Frontend: http://localhost:4200
   - Backend API: http://localhost:5000
   - API Documentation: http://localhost:5000/swagger

## Manual Setup

### Backend Setup

1. **Navigate to backend directory**
   ```bash
   cd backend/DeepSeekChatApi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the API**
   ```bash
   dotnet run
   ```

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm start
   ```

## API Endpoints

### Chat Controller

- `POST /api/chat/send` - Send a message to DeepSeek
- `GET /api/chat/history/{conversationId}` - Get chat history
- `DELETE /api/chat/history/{conversationId}` - Clear chat history
- `GET /api/chat/health` - Health check endpoint

### Request/Response Examples

**Send Message:**
```json
POST /api/chat/send
{
  "message": "Hello, how are you?",
  "conversationId": "conv_123456789"
}
```

**Response:**
```json
{
  "response": "Hello! I'm doing well, thank you for asking. How can I help you today?",
  "conversationId": "conv_123456789",
  "success": true
}
```

## Configuration

### Backend Configuration (appsettings.json)

```json
{
  "DeepSeek": {
    "ModelId": "deepseek-chat",
    "ApiKey": "your-api-key-here",
    "Endpoint": "https://api.deepseek.com/v1/",
    "ServiceId": "deepseek"
  }
}
```

### Frontend Configuration (environment.ts)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

## Docker Configuration

### Backend Dockerfile
- Multi-stage build with .NET 8.0
- Optimized for production deployment
- Exposes ports 80 and 443

### Frontend Dockerfile
- Node.js build stage with Angular CLI
- Nginx production serving
- Optimized static asset delivery

### Docker Compose
- Orchestrates both services
- Includes networking and volume configuration
- Environment variable management

## Project Structure

```
Docker/
├── backend/
│   ├── DeepSeekChatApi/
│   │   ├── Controllers/
│   │   │   └── ChatController.cs
│   │   ├── Models/
│   │   │   ├── ChatModels.cs
│   │   │   └── DeepSeekSettings.cs
│   │   ├── Services/
│   │   │   ├── IDeepSeekChatService.cs
│   │   │   └── DeepSeekChatService.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── DeepSeekChatApi.csproj
│   └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── models/
│   │   │   │   └── chat.models.ts
│   │   │   ├── app.component.ts
│   │   │   ├── app.component.html
│   │   │   ├── app.component.css
│   │   │   ├── app.module.ts
│   │   │   └── chat.service.ts
│   │   ├── environments/
│   │   │   └── environment.ts
│   │   ├── index.html
│   │   ├── main.ts
│   │   └── styles.css
│   ├── angular.json
│   ├── package.json
│   ├── tsconfig.json
│   ├── nginx.conf
│   └── Dockerfile
├── docker-compose.yml
└── README.md
```

## Features in Detail

### Chat Interface
- Clean, modern design with gradient styling
- Real-time typing indicators
- Message timestamps
- Responsive layout for mobile devices
- Auto-scrolling to latest messages

### Error Handling
- Comprehensive error handling on both frontend and backend
- User-friendly error messages
- Connection status monitoring
- Graceful fallback for network issues

### Performance Optimizations
- Efficient state management
- Optimized Docker images
- Nginx caching for static assets
- Gzip compression

## Development

### Adding New Features

1. **Backend**: Add new endpoints in `ChatController.cs`
2. **Frontend**: Extend `ChatService` and update components
3. **Models**: Update TypeScript interfaces and C# models

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: Backend binding URLs
- DeepSeek API configuration in appsettings.json

## Troubleshooting

### Common Issues

1. **API Key Issues**
   - Ensure your DeepSeek API key is valid
   - Check the API key format in appsettings.json

2. **Connection Issues**
   - Verify Docker containers are running
   - Check network connectivity between services

3. **Build Issues**
   - Ensure Docker has sufficient resources
   - Clear Docker cache if needed: `docker system prune`

### Logs

- Backend logs: `docker-compose logs deepseek-api`
- Frontend logs: `docker-compose logs deepseek-frontend`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository or contact the development team. 