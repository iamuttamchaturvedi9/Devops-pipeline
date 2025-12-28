# Deployment Guide - Azure App Service

This guide walks you through deploying your Shopping API to Azure App Service using **Azure DevOps Pipelines**.

## Prerequisites

- Azure account with an active subscription
- Azure DevOps organization and project
- Code stored in Azure Repos (Azure DevOps Git)
- Azure CLI installed (optional, but recommended)

## Step-by-Step Instructions

### Step 1: Create Azure App Service

You can create the App Service using Azure Portal or Azure CLI.

#### Option A: Using Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"**
3. Search for **"Web App"** and select it
4. Click **"Create"**
5. Fill in the details:
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or select existing (e.g., `rg-shopping`)
   - **Name**: `shopping-api` (must be globally unique, add numbers if needed)
   - **Publish**: Code
   - **Runtime stack**: .NET 8
   - **Operating System**: Linux (recommended) or Windows
   - **Region**: Choose closest to you
   - **App Service Plan**: Create new or select existing
     - **Plan name**: `plan-shopping`
     - **Sku and size**: Free F1 (for testing) or Basic B1 (for production)
6. Click **"Review + create"** then **"Create"**
7. Wait for deployment to complete (2-3 minutes)

#### Option B: Using Azure CLI

```powershell
# Login to Azure
az login

# Set variables
$resourceGroup = "rg-shopping"
$location = "eastus"
$appServicePlan = "plan-shopping"
$appName = "shopping-api"  # Must be globally unique

# Create resource group
az group create --name $resourceGroup --location $location

# Create App Service Plan
az appservice plan create --name $appServicePlan --resource-group $resourceGroup --sku FREE --is-linux

# Create Web App
az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $appName --runtime "DOTNET|8.0"
```

### Step 2: Get Publish Profile

1. Go to your App Service in Azure Portal
2. Click on **"Get publish profile"** button (top menu)
3. This downloads a `.PublishSettings` file
4. **Open the file** in a text editor (like Notepad)
5. **Copy the entire contents** - you'll need this for the next step

### Step 3: Create Azure Service Connection in Azure DevOps

**This connects Azure DevOps to your Azure subscription for deployment.**

**Detailed Steps:**

1. **Go to Azure DevOps:**
   - Navigate to: `https://dev.azure.com/YOUR_ORG/YOUR_PROJECT`
   - Replace `YOUR_ORG` with your organization name
   - Replace `YOUR_PROJECT` with your project name

2. **Open Project Settings:**
   - Click the **gear icon** ⚙️ (Settings) in the bottom left corner
   - Or click **"Project settings"** from the left sidebar

3. **Navigate to Service Connections:**
   - In the left sidebar, under **"Pipelines"**, click **"Service connections"**

4. **Create New Service Connection:**
   - Click **"New service connection"** button (top right)
   - Select **"Azure Resource Manager"**
   - Click **"Next"**

5. **Choose Authentication Method:**
   - Select **"Workload Identity federation (recommended)"** or **"Service principal (automatic)"**
   - Click **"Next"**

6. **Configure Connection:**
   - **Scope level**: Select **"Subscription"**
   - **Subscription**: Select your Azure subscription
   - **Resource group**: Leave empty or select a specific resource group
   - **Service connection name**: Enter a name (e.g., `Azure-Shopping-Connection`)
   - **Security**: Check **"Grant access permission to all pipelines"** (or configure specific pipelines)
   - Click **"Save"**

7. **Note the Service Connection Name:**
   - Remember the name you gave it (e.g., `Azure-Shopping-Connection`)
   - You'll need to update this in the pipeline file

**Alternative: Using Publish Profile (Simpler but less secure)**

If you prefer using publish profile:
1. Download publish profile from Azure Portal (Step 2)
2. In Azure DevOps → Project Settings → Service Connections
3. Create new → **"Azure App Service deployment"**
4. Select **"Publish Profile"**
5. Upload the `.PublishSettings` file
6. Name it (e.g., `Shopping-AppService-Profile`)

### Step 4: Update Pipeline File

1. **Open the pipeline file** (`azure-pipelines.yml`) in your repository

2. **Update these variables:**
   - `azureSubscription`: Change `'YOUR_SERVICE_CONNECTION_NAME'` to the service connection name you created (e.g., `'Azure-Shopping-Connection'`)
   - `appName`: Change `'shopping-api'` to match your App Service name if different

3. **Commit and push the changes:**
   ```powershell
   git add azure-pipelines.yml
   git commit -m "Configure Azure Pipeline"
   git push
   ```

**Example after update:**
```yaml
variables:
  azureSubscription: 'Azure-Shopping-Connection'  # Your service connection name
  appName: 'shopping-api'  # Your App Service name
```

### Step 5: Configure App Settings in Azure

Since your app uses MongoDB, you need to configure the connection string:

1. Go to your App Service in Azure Portal
2. Click **Configuration** (left sidebar)
3. Click **"New application setting"**
4. Add these settings:

   **Name**: `DatabaseSettings:ConnectionString`  
   **Value**: Your MongoDB connection string (e.g., `mongodb://your-mongodb-server:27017`)

   **Name**: `DatabaseSettings:DatabaseName`  
   **Value**: `ProductDb`

   **Name**: `DatabaseSettings:CollectionName`  
   **Value**: `Products`

5. Click **"Save"** (top menu)
6. Click **"Continue"** when prompted

> **Note**: If you're using MongoDB Atlas (cloud MongoDB), use the connection string from Atlas.  
> If you're using Azure Cosmos DB with MongoDB API, use the connection string from Cosmos DB.

### Step 6: Create and Run the Pipeline

#### Option A: Create Pipeline from YAML File

1. **Go to Azure DevOps:**
   - Navigate to your project: `https://dev.azure.com/YOUR_ORG/YOUR_PROJECT`

2. **Open Pipelines:**
   - Click **"Pipelines"** in the left sidebar
   - Click **"Pipelines"** (under Pipelines section)

3. **Create New Pipeline:**
   - Click **"New pipeline"** button
   - Select **"Azure Repos Git"** (or your repository source)
   - Select your repository
   - Select **"Existing Azure Pipelines YAML file"**
   - Choose branch: `main` or `master`
   - Choose path: `/azure-pipelines.yml`
   - Click **"Continue"**

4. **Review and Run:**
   - Review the pipeline configuration
   - Click **"Run"** to start the first build

#### Option B: Push to Main Branch (Auto-trigger)

Once the pipeline is created, it will automatically trigger on pushes to `main` or `master`:

```powershell
git add .
git commit -m "Setup deployment pipeline"
git push origin main
```

### Step 7: Monitor Deployment

1. **Go to Pipelines in Azure DevOps:**
   - Click **"Pipelines"** → **"Pipelines"** in left sidebar

2. **View Pipeline Runs:**
   - Click on your pipeline name
   - You'll see all pipeline runs

3. **Monitor Progress:**
   - Click on a running pipeline to see real-time logs
   - Green checkmark = Success ✅
   - Red X = Failure ❌ (click to see error details)

4. **View Logs:**
   - Click on any stage (Build or Deploy) to see detailed logs
   - Expand individual tasks to see step-by-step output

### Step 8: Verify Your App

1. Once deployment succeeds, go to Azure Portal
2. Open your App Service
3. Click **"Browse"** or go to: `https://your-app-name.azurewebsites.net`
4. Try accessing: `https://your-app-name.azurewebsites.net/swagger` (if Swagger is enabled)

## Troubleshooting

### Deployment Fails

1. **Check App Service name**: Ensure it matches in `azure-pipelines.yml` (variable `appName`)
2. **Check service connection**: Verify the service connection name matches in pipeline file
3. **Check service connection permissions**: Ensure it has access to your App Service
4. **Check build errors**: Look at the "Build" stage logs in Azure Pipelines
5. **Check .NET version**: Ensure your project uses .NET 8.0
6. **Check Azure subscription**: Verify the service connection is linked to the correct subscription

### App Doesn't Work After Deployment

1. **Check Application Logs**:
   - Azure Portal → App Service → **Log stream** (real-time logs)
   - Or **Logs** section for detailed logs

2. **Check Configuration**:
   - Verify MongoDB connection string is set correctly
   - Check all app settings are configured

3. **Check Runtime**:
   - App Service → Configuration → General settings
   - Ensure Stack is set to `.NET 8`

### Common Issues

- **404 Error**: App might not be starting. Check logs.
- **500 Error**: Usually configuration issue (MongoDB connection, etc.)
- **Build fails**: Check if all NuGet packages are available

## Next Steps

- Set up custom domain (optional)
- Configure SSL/TLS certificates
- Set up staging slots for blue-green deployments
- Configure monitoring and alerts
- Set up CI/CD for multiple environments

## Useful Commands

```powershell
# View app logs
az webapp log tail --name shopping-api --resource-group rg-shopping

# Restart app
az webapp restart --name shopping-api --resource-group rg-shopping

# View app settings
az webapp config appsettings list --name shopping-api --resource-group rg-shopping

# Update app setting
az webapp config appsettings set --name shopping-api --resource-group rg-shopping --settings "DatabaseSettings:ConnectionString=mongodb://new-connection-string"
```


