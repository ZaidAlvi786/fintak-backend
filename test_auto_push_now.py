#!/usr/bin/env python3
"""
Test script to run auto_push.py immediately for testing
"""

import subprocess
import sys
import os
from pathlib import Path

def main():
    script_dir = Path(__file__).parent
    auto_push_script = script_dir / "auto_push.py"
    
    if not auto_push_script.exists():
        print("❌ auto_push.py not found!")
        return 1
    
    print("🧪 Testing Auto Push System...")
    print(f"📁 Script directory: {script_dir}")
    print(f"🐍 Python script: {auto_push_script}")
    
    # Change to script directory
    os.chdir(script_dir)
    
    # Run the auto push script
    try:
        result = subprocess.run([sys.executable, "auto_push.py"], 
                              capture_output=True, text=True, timeout=60)
        
        print("📤 STDOUT:")
        print(result.stdout)
        
        if result.stderr:
            print("❌ STDERR:")
            print(result.stderr)
        
        print(f"🔢 Exit code: {result.returncode}")
        
        if result.returncode == 0:
            print("✅ Test completed successfully!")
        else:
            print("❌ Test failed!")
            
    except subprocess.TimeoutExpired:
        print("⏰ Test timed out after 60 seconds")
    except Exception as e:
        print(f"❌ Test error: {e}")
    
    return 0

if __name__ == "__main__":
    sys.exit(main())