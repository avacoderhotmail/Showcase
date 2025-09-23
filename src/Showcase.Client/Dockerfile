# Log in to Azure and ACR
az login
az acr login --name <your-acr-name>

# Build and tag API image
docker build -f Showcase.API/Dockerfile -t <your-acr-name>.azurecr.io/showcase-api:latest .

# Build and tag Client image (if containerizing UI)
docker build -f Showcase.Client/Dockerfile -t <your-acr-name>.azurecr.io/showcase-client:latest .

# Push images
docker push <your-acr-name>.azurecr.io/showcase-api:latest
docker push <your-acr-name>.azurecr.io/showcase-client:latest
