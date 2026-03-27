# Setup GitHub Actions for Auto-Build

## Step-by-Step Instructions

### 1. Create GitHub Repository

1. Go to https://github.com
2. Click the **+** icon (top right) → **New repository**
3. Repository name: `RJ_VC_Bypass_Windows`
4. Choose **Private** (recommended) or Public
5. Click **Create repository**

### 2. Upload Your Code

**Option A: Using Git (Recommended)**

```bash
cd /storage/emulated/0/Documents/window

# Initialize git repo
git init

# Add all files
git add .

# Create first commit
git commit -m "Initial commit - RJ VC Bypass Windows Edition"

# Add your GitHub repo as remote (replace YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/RJ_VC_Bypass_Windows.git

# Push to GitHub
git branch -M main
git push -u origin main
```

**Option B: Using GitHub Web Upload**

1. On your new repository page, click **uploading an existing file**
2. Drag and drop all files from `/storage/emulated/0/Documents/window/`
3. Click **Commit changes**

### 3. GitHub Actions Will Auto-Run

Once you push code:

1. Go to your repository on GitHub
2. Click the **Actions** tab
3. You'll see "Build Windows EXE" workflow running
4. Wait for it to complete (green checkmark)

### 4. Download the Built EXE

**From Workflow Run:**
1. Click on the workflow run in the Actions tab
2. Scroll down to **Artifacts**
3. Click `RJ_VC_Bypass-Windows` to download
4. Extract the ZIP - contains the EXE file

**From Release (if you create a tag):**
```bash
# Create a release tag
git tag v1.0
git push origin v1.0
```

Then go to **Releases** on GitHub to download.

### 5. Run the Application

1. Extract the downloaded artifact
2. Run `RJ_VC_Bypass.exe`
3. Windows may show a warning - click "More info" → "Run anyway"

---

## Troubleshooting

### Build Fails
- Check the **Actions** tab for error details
- Ensure all files were uploaded correctly

### Artifact Expired
- Artifacts are kept for 30 days
- Re-run the workflow or create a new release

### Need Faster Builds
- The workflow runs on GitHub's servers (usually 2-3 minutes)
- Free tier: 2000 minutes/month

---

## File Locations

- **Source code:** `/storage/emulated/0/Documents/window/`
- **GitHub repo:** `https://github.com/YOUR_USERNAME/RJ_VC_Bypass_Windows`
- **Actions tab:** `https://github.com/YOUR_USERNAME/RJ_VC_Bypass_Windows/actions`
- **Releases:** `https://github.com/YOUR_USERNAME/RJ_VC_Bypass_Windows/releases`
