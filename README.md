# Shopping - DevOps Pipeline (Build and Deployment)

This project uses a .NET 8 build pipeline and supports deployment to Azure App Service today, with AKS deployment flow documented for container-based rollout.

## CI/CD Overview

- **Azure DevOps**: `azure-pipelines.yml` (triggered on `main`/`master`)
- **GitHub Actions**: `.github/workflows/main.yml` (push + manual trigger)
- **Primary app project**: `Shopping.API/Shopping.API.csproj`

## Build Pipeline

Both pipelines follow the same core build path:

1. Setup .NET SDK 8
2. Restore NuGet packages
3. Build in Release mode
4. Publish API output
5. Package artifact for deployment

## Deployment - Azure App Service

### Current deployment path

- Azure DevOps pipeline deploys a zipped publish artifact to App Service.
- Post-deploy steps configure runtime and restart the app.
- GitHub Actions also supports direct App Service deployment using publish profile secret.

### Required values

- Azure subscription/service connection
- App Service name
- Publish profile secret (for GitHub Actions path)

## Deployment - AKS (Recommended Container Path)

Use AKS when you need container orchestration, scaling control, and multi-service release management.

1. Build Docker image for `Shopping.API`
2. Push image to container registry (for example, ACR)
3. Create/update Kubernetes manifests (or Helm chart):
   - Deployment
   - Service
   - Ingress (optional)
   - ConfigMap/Secret for settings
4. Deploy to AKS using `kubectl apply` or `helm upgrade --install`
5. Verify rollout with `kubectl get pods` and `kubectl rollout status`

## Quick Notes

- Keep environment-specific configuration in App Settings / Kubernetes Secrets.
- Use separate environments (Dev/UAT/Prod) with independent pipeline variables.
- Add health checks/readiness probes before production AKS rollout.

## C# References (Client and Server)

- C# Client (`HttpClient`): [Microsoft Learn - Make HTTP requests with HttpClient in .NET](https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient)
- C# Server (ASP.NET Core Web API): [Microsoft Learn - Create web APIs with ASP.NET Core](https://learn.microsoft.com/aspnet/core/web-api/)

