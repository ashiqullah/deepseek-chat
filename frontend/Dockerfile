# Build stage
FROM --platform=$BUILDPLATFORM node:18-alpine AS build

WORKDIR /app

# Copy package files (context is frontend directory)
COPY package*.json ./

# Install ALL dependencies (including devDependencies for build)
RUN npm ci

# Install Angular CLI globally
RUN npm install -g @angular/cli@17

# Copy source code
COPY . .

# Build the Angular app
RUN ng build --configuration production

# Production stage
FROM --platform=$TARGETPLATFORM nginx:alpine

# Copy built app to nginx
COPY --from=build /app/dist/deepseek-chat-frontend /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"] 