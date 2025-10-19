import subprocess
import datetime
import random
import os
import json
import requests
import schedule
import time
import sys
from pathlib import Path

WEBHOOK_URL = ""  # Set your webhook URL here or leave empty to disable notifications

# Configuration
PUSHES_PER_DAY = 7
START_HOUR = 10  # 10 AM
END_HOUR = 22    # 10 PM
STATE_FILE = "auto_push_state.json"

def send_webhook(message):
    """Send a message to Slack/Discord via webhook"""
    if not WEBHOOK_URL:
        print("⚠️ WEBHOOK_URL not set")
        return
    # Discord uses "content", Slack uses "text"
    data = {"text": message}  # change to {"content": message} if using Discord
    try:
        response = requests.post(WEBHOOK_URL, data=json.dumps(data), headers={"Content-Type": "application/json"})
        if response.status_code in [200, 204]:
            print("📢 Notification sent successfully!")
        else:
            print(f"❌ Failed to send notification: {response.text}")
    except Exception as e:
        print(f"❌ Exception sending webhook: {e}")

def run_cmd(cmd, exit_on_error=True):
    """Run a shell command safely"""
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        error_msg = f"Error running command: {cmd}\n{result.stderr}"
        print(error_msg)
        if exit_on_error:
            send_webhook(f"🚨 Auto Push Failed:\n{error_msg}")
            exit(1)
        else:
            raise Exception(error_msg)
    return result.stdout.strip()

def get_candidate_files():
    """Return list of tracked + untracked files (ignores .gitignore)."""
    tracked = run_cmd("git ls-files").splitlines()
    untracked = run_cmd("git ls-files --others --exclude-standard").splitlines()
    return tracked + untracked

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
    """Check if current time is within the allowed schedule (10 AM - 10 PM)"""
    current_hour = datetime.datetime.now().hour
    return START_HOUR <= current_hour < END_HOUR

def push_single_file():
    """Push one file at a time to the daily branch"""
    state = load_state()
    today = datetime.datetime.now().strftime("%Y-%m-%d")
    branch_name = f"bug-fixes-{today}"
    
    # Check if we need to create a new branch for today
    if state["last_push_date"] != today:
        state["pushes_today"] = 0
        state["pushed_files"] = []
        state["last_push_date"] = today
        state["current_branch"] = branch_name
        save_state(state)
    
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
        except:
            run_cmd(f"git checkout -b {branch_name}")
            print(f"🌿 Created new branch: {branch_name}")

        candidates = get_candidate_files()
        if not candidates:
            print("⚡ No files to commit.")
            return

        # Filter out already pushed files today
        available_files = [f for f in candidates if f not in state["pushed_files"]]
        
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
            commit_message = f"Automated commit: {selected_file} on {today}"
            run_cmd(f'git commit -m "{commit_message}"')
            run_cmd(f"git push origin {branch_name}")
            
            # Update state
            state["pushes_today"] += 1
            state["pushed_files"].append(selected_file)
            save_state(state)
            
            success_msg = f"✅ Auto-push succeeded: {selected_file} pushed to {branch_name} (Push #{state['pushes_today']}/{PUSHES_PER_DAY})"
            print(success_msg)
            send_webhook(success_msg)
        else:
            print("⚡ No changes to commit for selected file.")
            
    except Exception as e:
        error_msg = f"🚨 Auto Push Script Failed:\n{str(e)}"
        print(error_msg)
        send_webhook(error_msg)
        raise

def schedule_pushes():
    """Schedule pushes throughout the day"""
    # Calculate interval between pushes
    total_hours = END_HOUR - START_HOUR
    interval_minutes = (total_hours * 60) // PUSHES_PER_DAY
    
    print(f"📅 Scheduling {PUSHES_PER_DAY} pushes between {START_HOUR}:00 and {END_HOUR}:00")
    print(f"⏱️  Interval: approximately {interval_minutes} minutes between pushes")
    
    # Schedule pushes at regular intervals
    for i in range(PUSHES_PER_DAY):
        minutes_offset = i * interval_minutes
        hour = START_HOUR + (minutes_offset // 60)
        minute = minutes_offset % 60
        
        if hour < END_HOUR:
            schedule.every().day.at(f"{hour:02d}:{minute:02d}").do(push_single_file)
            print(f"⏰ Scheduled push #{i+1} at {hour:02d}:{minute:02d}")

def run_scheduler():
    """Run the scheduler continuously"""
    print("🚀 Starting auto-push scheduler...")
    print(f"📊 Configuration: {PUSHES_PER_DAY} pushes per day between {START_HOUR}:00-{END_HOUR}:00")
    
    schedule_pushes()
    
    while True:
        schedule.run_pending()
        time.sleep(60)  # Check every minute

def main():
    """Main function - can be called directly or via cron"""
    if len(sys.argv) > 1 and sys.argv[1] == "--scheduler":
        # Run as continuous scheduler
        run_scheduler()
    else:
        # Run single push (for cron jobs)
        push_single_file()

if __name__ == "__main__":
    main()
