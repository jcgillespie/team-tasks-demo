# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Install pnpm
RUN corepack enable pnpm

# Copy workspace files for dependency caching
COPY client/package.json client/pnpm-lock.yaml ./
COPY pnpm-workspace.yaml ../

RUN pnpm install --frozen-lockfile

# Copy source
COPY client/ ./

RUN pnpm build

# Runtime stage — serve static assets with nginx
FROM nginx:1.27-alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Custom nginx config that supports SPA routing
COPY --from=build /app/dist /usr/share/nginx/html

RUN cat > /etc/nginx/conf.d/default.conf << 'EOF'
server {
    listen 8080;
    root /usr/share/nginx/html;
    index index.html;

    # SPA fallback — serve index.html for all unknown routes
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Proxy API requests to the API service
    location /api/ {
        proxy_pass http://teamtasks-api-svc:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    # Health check endpoint
    location /healthz {
        access_log off;
        return 200 "ok\n";
        add_header Content-Type text/plain;
    }
}
EOF

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
