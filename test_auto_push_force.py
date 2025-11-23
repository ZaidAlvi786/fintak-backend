#!/usr/bin/env python3
"""
Test script to run auto_push.py with schedule bypass for testing
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
    
    print("🧪 Testing Auto Push System (Force Mode)...")
    print(f"📁 Script directory: {script_dir}")
    print(f"🐍 Python script: {auto_push_script}")
    
    # Change to script directory
    os.chdir(script_dir)
    
    # Temporarily modify the schedule to allow testing
    print("🔧 Temporarily modifying schedule for testing...")
    
    # Read the current auto_push.py
    with open("auto_push.py", "r") as f:
        content = f.read()
    
    # Create a backup
    with open("auto_push.py.backup", "w") as f:
        f.write(content)
    
    # Modify the schedule to allow testing (24/7)
    modified_content = content.replace(
        "START_HOUR = 10  # 10 AM",
        "START_HOUR = 0  # 12 AM (for testing)"
    ).replace(
        "END_HOUR = 22    # 10 PM",
        "END_HOUR = 24    # 12 AM (for testing)"
    )
    
    # Write the modified version
    with open("auto_push.py", "w") as f:
        f.write(modified_content)
    
    try:
        # Run the auto push script
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
    finally:
        # Restore the original file
        print("🔄 Restoring original auto_push.py...")
        with open("auto_push.py.backup", "r") as f:
            original_content = f.read()
        with open("auto_push.py", "w") as f:
            f.write(original_content)
        os.remove("auto_push.py.backup")
    
    return 0

if __name__ == "__main__":
    sys.exit(main())

