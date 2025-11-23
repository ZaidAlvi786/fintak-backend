#!/usr/bin/env python3

import subprocess
import datetime
import random
import os
import json
import requests
import sys
from pathlib import Path

# Import the functions from auto_push.py
sys.path.append('/Users/apple/Downloads/demo/fintak-backend')

# Configuration for midnight testing
PUSHES_PER_DAY = 7
START_HOUR = 0   # Allow testing at midnight
END_HOUR = 24    # Allow testing anytime
STATE_FILE = "auto_push_state.json"

def run_cmd(cmd, exit_on_error=True):
    """Run a shell command safely"""
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        error_msg = f"Error running command: {cmd}\n{result.stderr}"
        print(error_msg)
        if exit_on_error:
            exit(1)
        else:
            raise Exception(error_msg)
    return result.stdout.strip()

def get_candidate_files():
    """Return list of tracked files with changes (ignores .gitignore and sensitive files)."""
    # Get only tracked files that have changes (modified, added, etc.)
    status_output = run_cmd("git status --porcelain")
    changed_files = []
    
    for line in status_output.splitlines():
        if line.strip():
            # Parse git status output (format: "XY filename")
            status = line[:2]
            filename = line[3:].strip()
            
            # Handle filenames with spaces (they might be quoted)
            if filename.startswith('"') and filename.endswith('"'):
                filename = filename[1:-1]
            
            # Only include files that are tracked and have changes
            if status[0] in ['M', 'A', 'R', 'C']:  # Modified, Added, Renamed, Copied
                changed_files.append(filename)
    
    # Filter out sensitive files that should never be committed
    sensitive_files = {
        'secrets_map.json',
        '.env',
        'auto_push_state.json',
        'auto_push.log',
        'auto_push_test.log',
        '*.key',
        '*.pem',
        '*.p12',
        '*.pfx',
        'config.json',
        'credentials.json'
    }
    
    filtered_files = []
    
    for file in changed_files:
        # Skip sensitive files
        if any(sensitive in file.lower() for sensitive in sensitive_files):
            print(f"🔒 Skipping sensitive file: {file}")
            continue
        # Skip files with common secret patterns
        if any(pattern in file.lower() for pattern in ['secret', 'password', 'token', 'key', 'credential']):
            print(f"🔒 Skipping potentially sensitive file: {file}")
            continue
        filtered_files.append(file)
    
    return filtered_files

def load_state():
    """Load the current state from file"""
    if os.path.exists(STATE_FILE):
        try:
            with open(STATE_FILE, 'r') as f:
                return json.load(f)
        except:
            pass
    return {
        "current_branch": None,
        "last_push_date": None,
        "pushes_today": 0,
        "pushed_files": []
    }

def save_state(state):
    """Save the current state to file"""
    with open(STATE_FILE, 'w') as f:
        json.dump(state, f, indent=2)

def is_within_schedule():
    """Check if current time is within the allowed schedule"""
    current_hour = datetime.datetime.now().hour
    return START_HOUR <= current_hour < END_HOUR

def midnight_test_push():
    """Test pushing one file at midnight"""
    state = load_state()
    today = datetime.datetime.now().strftime("%Y-%m-%d")
    branch_name = f"bug-fixes-{today}"
    
    print(f"🌙 MIDNIGHT TEST - AUTO PUSH SYSTEM")
    print(f"📅 Today: {today}")
    print(f"🌿 Branch: {branch_name}")
    print(f"⏰ Current time: {datetime.datetime.now().strftime('%H:%M')}")
    print(f"📊 Schedule: {START_HOUR}:00 - {END_HOUR}:00 (ALLOWED FOR TESTING)")
    print(f"✅ Within schedule: {is_within_schedule()}")
    print("")
    
    # Check if we need to create a new branch for today
    if state["last_push_date"] != today:
        state["pushes_today"] = 0
        state["pushed_files"] = []
        state["last_push_date"] = today
        state["current_branch"] = branch_name
        save_state(state)
        print(f"🆕 New day detected - resetting state")
    
    # Check if we've reached the daily limit
    if state["pushes_today"] >= PUSHES_PER_DAY:
        print(f"✅ Daily push limit reached ({PUSHES_PER_DAY} pushes)")
        return
    
    # Check if we're within schedule
    if not is_within_schedule():
        print(f"⏰ Outside schedule hours ({START_HOUR}:00 - {END_HOUR}:00)")
        return

    try:
        run_cmd("git fetch origin")
        
        # Check if branch exists and switch to it, otherwise create new branch
        try:
            run_cmd(f"git checkout {branch_name}", exit_on_error=False)
            print(f"✅ Switched to existing branch: {branch_name}")
        except:
            run_cmd(f"git checkout -b {branch_name}")
            print(f"🌿 Created new branch: {branch_name}")

        candidates = get_candidate_files()
        print(f"📂 Found {len(candidates)} candidate files")
        
        if not candidates:
            print("⚡ No files to commit.")
            return

        # Filter out already pushed files today
        available_files = [f for f in candidates if f not in state["pushed_files"]]
        print(f"📂 Available files (not pushed today): {len(available_files)}")
        
        if not available_files:
            print("⚡ All available files have been pushed today.")
            return

        # Select one random file
        selected_file = random.choice(available_files)
        print(f"📂 Selected file for commit: {selected_file}")

        # Add and commit the single file
        run_cmd(f"git add '{selected_file}'")
        
        status = run_cmd("git status --porcelain")
        if status:
            commit_message = f"🌙 Midnight test commit: {selected_file} on {today}"
            run_cmd(f'git commit -m "{commit_message}"')
            print(f"✅ Committed: {commit_message}")
            
            # Try to push with better error handling
            try:
                push_result = run_cmd(f"git push origin {branch_name}", exit_on_error=False)
                print(f"📤 Push successful!")
                
                # Update state only on successful push
                state["pushes_today"] += 1
                state["pushed_files"].append(selected_file)
                save_state(state)
                
                success_msg = f"✅ MIDNIGHT TEST SUCCESS: {selected_file} pushed to {branch_name} (Push #{state['pushes_today']}/{PUSHES_PER_DAY})"
                print(success_msg)
                
            except Exception as push_error:
                error_msg = f"❌ Push failed: {str(push_error)}"
                print(error_msg)
                
                # Check if it's a GitHub push protection error
                if "GH013" in str(push_error) or "repository rule violations" in str(push_error).lower():
                    print("🔒 GitHub push protection detected - skipping this file")
                    print("💡 This file may contain secrets and was blocked by GitHub")
                else:
                    # For other errors, still update state to avoid retrying the same file
                    state["pushes_today"] += 1
                    state["pushed_files"].append(selected_file)
                    save_state(state)
        else:
            print("⚡ No changes to commit for selected file.")
            
    except Exception as e:
        error_msg = f"🚨 Midnight Test Failed:\n{str(e)}"
        print(error_msg)
        raise

if __name__ == "__main__":
    midnight_test_push()
