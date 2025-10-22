#!/usr/bin/env python3
"""
Test script to verify GitHub Actions environment
"""

import os
import sys

def main():
    print("🧪 Testing GitHub Actions Environment...")
    
    # Check if running in GitHub Actions
    if os.environ.get('GITHUB_ACTIONS'):
        print("✅ Running in GitHub Actions")
        print(f"📊 GitHub Actions info:")
        print(f"   - Repository: {os.environ.get('GITHUB_REPOSITORY', 'Unknown')}")
        print(f"   - Workflow: {os.environ.get('GITHUB_WORKFLOW', 'Unknown')}")
        print(f"   - Run ID: {os.environ.get('GITHUB_RUN_ID', 'Unknown')}")
    else:
        print("⚠️  Not running in GitHub Actions")
        print("💡 This script is designed to test GitHub Actions environment")
    
    # Test git commands
    import subprocess
    try:
        result = subprocess.run(['git', 'status'], capture_output=True, text=True)
        if result.returncode == 0:
            print("✅ Git is working")
        else:
            print(f"❌ Git error: {result.stderr}")
    except Exception as e:
        print(f"❌ Git test failed: {e}")
    
    # Test Python imports
    try:
        import requests
        import schedule
        print("✅ Required Python packages are available")
    except ImportError as e:
        print(f"❌ Missing package: {e}")
    
    print("🎯 GitHub Actions test completed!")

if __name__ == "__main__":
    main()
