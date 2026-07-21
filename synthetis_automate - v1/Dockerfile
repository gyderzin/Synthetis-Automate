<<<<<<< HEAD
# ========= ETAPA 1 =========
# Build da aplicação

FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

# ========= ETAPA 2 =========
# Servidor web

FROM nginx:stable-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

=======
# ========= ETAPA 1 =========
# Build da aplicação

FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

# ========= ETAPA 2 =========
# Servidor web

FROM nginx:stable-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
CMD ["nginx", "-g", "daemon off;"]