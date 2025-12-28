# Deployment Guide - Azure App Service

This guide walks you through deploying your Shopping API to Azure App Service using GitHub Actions.

## Prerequisites

- Azure account with an active subscription
- Azure CLI installed (optional, but recommended)
- GitHub repository with your code

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

### Step 3: Add GitHub Secret

**⚠️ IMPORTANT: You need to be on the REPOSITORY settings, not your personal account settings!**

**Detailed Steps:**

1. **First, navigate to your repository:**
   - Go to `https://github.com/YOUR_USERNAME/Shopping` (replace YOUR_USERNAME)
   - Or search for your repository in GitHub

2. **On the repository page**, look at the **top menu bar** (you should see tabs like: Code, Issues, Pull requests, Actions, Projects, Wiki, Security, Insights, Settings)
   - Click on **"Settings"** tab (it's usually the last tab on the right)

3. **In the repository Settings page**, look at the **left sidebar** - you should see different options like:
   - General
   - Access
   - Secrets and variables ← **This is what you need!**
   - Actions
   - Environments
   - etc.

4. Click on **"Secrets and variables"** in the left sidebar

5. Click on **"Actions"** (under "Secrets and variables")

6. You should see a button **"New repository secret"** - click it

7. Fill in:
   - **Name**: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - **Secret**: Paste the entire contents of the `.PublishSettings` file

8. Click **"Add secret"**

**Visual Guide:**
```
1. Go to: github.com/YOUR_USERNAME/Shopping
   ↓
2. Click "Settings" tab (top menu of repository)
   ↓
3. Left sidebar shows repository settings
   ↓
4. Click "Secrets and variables"
   ↓
5. Click "Actions"
   ↓
6. Click "New repository secret"
```

**Direct URL Method (Easiest):**

Replace `YOUR_USERNAME` with your GitHub username:
```
https://github.com/YOUR_USERNAME/Shopping/settings/secrets/actions
```

**Common Mistake:**
- ❌ **Wrong**: Going to your profile → Settings (personal account settings)
- ✅ **Correct**: Going to your repository → Settings tab (repository settings)

**If you still can't find it:**
- Make sure you're on the **repository page**, not your profile page
- You need **admin/owner** permissions on the repository
- If it's not your repo, ask the owner to add you as a collaborator with admin rights

### Step 4: Update Workflow File (if needed)

Check the workflow file (`.github/workflows/main.yml`) and ensure:
- `AZURE_WEBAPP_NAME` matches your App Service name (currently set to `shopping-api`)

If your App Service name is different, update line 11 in the workflow file.

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

### Step 6: Test the Deployment

#### Option A: Manual Trigger

1. Go to your GitHub repository
2. Click **Actions** tab
3. Select **"Build and Deploy to Azure App Service"** workflow
4. Click **"Run workflow"** button (right side)
5. Select branch (usually `main` or `master`)
6. Click **"Run workflow"**

#### Option B: Push to Main Branch

Simply push your code to the `main` or `master` branch:

```powershell
git add .
git commit -m "Setup deployment workflow"
git push origin main
```

The workflow will automatically trigger.

### Step 7: Monitor Deployment

1. Go to **Actions** tab in GitHub
2. Click on the running workflow
3. Watch the build and deployment progress
4. Green checkmark = Success ✅
5. Red X = Failure ❌ (click to see error details)

### Step 8: Verify Your App

1. Once deployment succeeds, go to Azure Portal
2. Open your App Service
3. Click **"Browse"** or go to: `https://your-app-name.azurewebsites.net`
4. Try accessing: `https://your-app-name.azurewebsites.net/swagger` (if Swagger is enabled)

## Troubleshooting

### Deployment Fails

1. **Check App Service name**: Ensure it matches in workflow file
2. **Check publish profile secret**: Verify it's correctly added in GitHub
3. **Check build errors**: Look at the "Build" step logs in GitHub Actions
4. **Check .NET version**: Ensure your project uses .NET 8.0

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


