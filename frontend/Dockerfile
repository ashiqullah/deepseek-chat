# Build stage
FROM --platform=$BUILDPLATFORM node:18-alpine AS build

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production

# Copy source code
COPY . .

# Build the Angular app
RUN npm run build

# Production stage
FROM --platform=$TARGETPLATFORM nginx:alpine

# Copy built app to nginx
COPY --from=build /app/dist/deepseek-chat-frontend /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"] 