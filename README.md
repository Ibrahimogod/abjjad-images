# Abjjad Images API

[![Build and Test](https://github.com/Ibrahimogod/abjjad-images/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/Ibrahimogod/abjjad-images/actions/workflows/build-and-test.yml)

This is the image processing API service for Abjjad platform.

## Prerequisites

- Docker Desktop
- Docker Compose (optional)

## Getting Started

### Running with Docker

You can run the application using the pre-built Docker image from GitHub Container Registry:

1. Pull the latest image:
```bash
docker pull ghcr.io/ibrahimogod/abjjad-images-api:latest
```

2. Run the container:
```bash
docker run -d \
  -p 7090:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e StorageOptions__BaseImagesPath=images \
  -e StorageOptions__BaseFilesPath=files/data.json \
  -e ImageProcessingOptions__Quality=80 \
  -e ImageProcessingOptions__Method=4 \
  -e ImageProcessingOptions__PhoneWidth=480 \
  -e ImageProcessingOptions__PhoneHeight=800 \
  -e ImageProcessingOptions__TabletWidth=1024 \
  -e ImageProcessingOptions__TabletHeight=768 \
  -e ImageProcessingOptions__DesktopWidth=1920 \
  -e ImageProcessingOptions__DesktopHeight=1080 \
  -v /wwwroot/container:/app/wwwroot \
  ghcr.io/ibrahimogod/abjjad-images-api:latest
```

Alternatively, you can build and run from source:

1. Clone the repository:
```bash
git clone https://github.com/Ibrahimogod/abjjad-images.git
cd abjjad-images
```

2. Build and run the application using Docker Compose:
```bash
docker-compose up --build
```

The API will be available at:
- HTTP: http://localhost:7090
- Swagger UI: http://localhost:7090/swagger

### Development

To run the application in development mode:

1. Make sure you have .NET 8.0 SDK installed
2. Navigate to the API project directory:
```bash
cd Abjjad.Images.API
```

3. Run the application:
```bash
dotnet run
```

## API Documentation

The API documentation is available through Swagger UI when the application is running:
- https://localhost:7091/swagger: 
- http://localhost:7090/swagger

## Health Checks

The API includes built-in health checks to monitor the application's status. The following health check endpoints are available:

- `/healthz` - Overall health status of the application
- `/healthz/ready` - Readiness probe endpoint
- `/healthz/live` - Liveness probe endpoint
- `/healthz-ui` - Health check dashboard UI

The health checks monitor:
- Images directory accessibility
- Data file accessibility

The health check dashboard is available at:
- http://localhost:7090/healthz-ui

## Project Structure

- `Abjjad.Images.API/` - Main API project
- `Abjjad.Images/` - Core library containing business logic and models
- `Abjjad.Images.Core/` - Core library containing core models

## Docker Commands

### Build the Image
```bash
docker-compose build
```

### Run the Container
```bash
docker-compose up
```

### Run in Background
```bash
docker-compose up -d
```

### Stop the Container
```bash
docker-compose down
```

### View Logs
```bash
docker-compose logs -f
```

## Environment Variables

The following environment variables can be configured:

### Storage configuration
- `StorageOptions__BaseImagesPath`= images
- `StorageOptions__BaseFilesPath` = files/data.json

### Image processing configuration
- `ImageProcessingOptions__Quality`= 80
- `ImageProcessingOptions__Method`= 4
- `ImageProcessingOptions__PhoneWidth`= 480
- `ImageProcessingOptions__PhoneHeight`= 800
- `ImageProcessingOptions__TabletWidth`= 1024
- `ImageProcessingOptions__TabletHeight`= 768
- `ImageProcessingOptions__DesktopWidth`= 1920
- `ImageProcessingOptions__DesktopHeight`= 1080