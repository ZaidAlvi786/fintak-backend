#!/bin/bash

# GitHub Actions Setup Script for Auto Push System
# This script helps you set up GitHub Actions to run your auto-push system 24/7

echo "🚀 Setting up GitHub Actions for Auto Push System..."

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo "❌ Error: Not in a git repository. Please run this from your project root."
    exit 1
fi

# Check if GitHub Actions workflow file exists
if [ ! -f ".github/workflows/auto-push.yml" ]; then
    echo "❌ Error: GitHub Actions workflow file not found."
    echo "Please make sure .github/workflows/auto-push.yml exists."
    exit 1
fi

echo "✅ GitHub Actions workflow file found!"

# Check if we have a remote origin
REMOTE_URL=$(git remote get-url origin 2>/dev/null)
if [ $? -ne 0 ]; then
    echo "❌ Error: No remote origin found."
    echo "Please add a GitHub remote: git remote add origin https://github.com/username/repo.git"
    exit 1
fi

echo "✅ Remote origin found: $REMOTE_URL"

# Check if it's a GitHub repository
if [[ $REMOTE_URL == *"github.com"* ]]; then
    echo "✅ GitHub repository detected!"
else
    echo "⚠️  Warning: This doesn't appear to be a GitHub repository."
    echo "GitHub Actions only works with GitHub repositories."
fi

echo ""
echo "📋 Next Steps:"
echo "1. Commit and push the GitHub Actions workflow:"
echo "   git add .github/workflows/auto-push.yml"
echo "   git commit -m 'Add GitHub Actions auto-push workflow'"
echo "   git push origin main"
echo ""
echo "2. Go to your GitHub repository in a web browser"
echo "3. Click on the 'Actions' tab"
echo "4. You should see the 'Auto Push System' workflow"
echo "5. The workflow will run automatically every 2 hours between 10 AM - 10 PM UTC"
echo ""
echo "🔧 Manual Testing:"
echo "- You can manually trigger the workflow from the Actions tab"
echo "- Check the logs to see if it's working correctly"
echo ""
echo "📊 Schedule:"
echo "- Runs 7 times per day: 10 AM, 12 PM, 2 PM, 4 PM, 6 PM, 8 PM, 10 PM UTC"
echo "- Each run pushes one file to a daily branch (bug-fixes-YYYY-MM-DD)"
echo "- Works 24/7 even when your computer is off!"
echo ""
echo "✅ Setup complete! Your auto-push system will now run on GitHub's servers."
