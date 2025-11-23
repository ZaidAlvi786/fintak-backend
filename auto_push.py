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
        # Silently skip if webhook is not configured (it's optional)
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
    """Return list of tracked files with changes (ignores .gitignore and sensitive files)."""
    # In GitHub Actions, pick from all tracked files since there are no local changes
    if os.environ.get('GITHUB_ACTIONS'):
        print("🔧 GitHub Actions detected - selecting from all tracked files...")
        try:
            # Get all tracked files
            all_tracked_files = run_cmd("git ls-files").splitlines()
            all_changed_files = [f for f in all_tracked_files if f.strip()]
        except:
            all_changed_files = []
    else:
        # Use git diff to get only modified tracked files - much simpler!
        try:
            # Get modified files that are tracked
            modified_files = run_cmd("git diff --name-only HEAD").splitlines()
            # Get staged files
            staged_files = run_cmd("git diff --cached --name-only").splitlines()
            
            # Combine and deduplicate
            all_changed_files = list(set(modified_files + staged_files))
            
            # Filter out empty strings
            all_changed_files = [f for f in all_changed_files if f.strip()]
            
        except:
            # Fallback: just get any modified files
            all_changed_files = run_cmd("git ls-files -m").splitlines()
    
    # Simple filter for sensitive files
    sensitive_files = {
        'secrets_map.json',
        '.env',
        'auto_push_state.json',
        'auto_push.log',
        'auto_push_test.log'
    }
    
    filtered_files = []
    for file in all_changed_files:
        if file and not any(sensitive in file.lower() for sensitive in sensitive_files):
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
    """Check if current time is within the allowed schedule (10 AM - 10 PM)"""
    # In GitHub Actions, always allow (schedule is handled by cron)
    if os.environ.get('GITHUB_ACTIONS'):
        return True
    
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
        # Set up git remote with token for authentication (do this before fetch)
        github_token = os.environ.get('GITHUB_TOKEN')
        if github_token:
            remote_url = run_cmd("git remote get-url origin", exit_on_error=False)
            if remote_url and remote_url.startswith("https://github.com/"):
                # Replace https://github.com/ with https://token@github.com/
                auth_url = remote_url.replace("https://github.com/", f"https://{github_token}@github.com/")
                run_cmd(f"git remote set-url origin {auth_url}")
        
        run_cmd("git fetch origin")
        
        # Check if branch exists remotely
        branch_exists_remote = False
        try:
            remote_branches = run_cmd(f"git ls-remote --heads origin {branch_name}", exit_on_error=False)
            if remote_branches and remote_branches.strip():
                branch_exists_remote = True
        except:
            pass
        
        # Check if branch exists locally and switch to it, otherwise create new branch
        branch_checked_out = False
        if branch_exists_remote:
            # Branch exists remotely, checkout and track it
            try:
                run_cmd(f"git checkout -b {branch_name} origin/{branch_name}", exit_on_error=False)
                print(f"🌿 Checked out existing remote branch: {branch_name}")
                branch_checked_out = True
            except:
                pass
        
        if not branch_checked_out:
            # Try to checkout existing local branch
            try:
                run_cmd(f"git checkout {branch_name}", exit_on_error=False)
                print(f"🌿 Checked out existing local branch: {branch_name}")
                branch_checked_out = True
            except:
                pass
        
        if not branch_checked_out:
            # Branch doesn't exist, create new one
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
        
        # Verify the file actually exists
        if not os.path.exists(selected_file):
            print(f"⚠️  Selected file doesn't exist: {selected_file}")
            # Mark as processed to avoid infinite retry
            state["pushed_files"].append(selected_file)
            save_state(state)
            return

        # In GitHub Actions, make a small change to the selected file
        if os.environ.get('GITHUB_ACTIONS'):
            print(f"🔧 Making small change to {selected_file} for GitHub Actions...")
            try:
                # Read the file content
                with open(selected_file, 'r', encoding='utf-8') as f:
                    content = f.read()
                
                # Add a small comment at the end (if it's a text file)
                # Include .disco files (XML-based discovery files) and other common text formats
                text_extensions = ('.py', '.js', '.ts', '.cs', '.html', '.css', '.txt', '.md', '.yml', '.yaml', '.json', '.xml', '.sh', '.disco', '.wsdl', '.xsd', '.config', '.csproj', '.sln', '.vb', '.fs', '.rb', '.java', '.cpp', '.c', '.h', '.hpp')
                if selected_file.endswith(text_extensions):
                    timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                    # Use XML comment format for XML-based files, HTML comment for others
                    if selected_file.endswith(('.xml', '.disco', '.wsdl', '.xsd', '.config', '.csproj')):
                        comment = f"\n<!-- Auto-push timestamp: {timestamp} -->"
                    else:
                        comment = f"\n\n<!-- Auto-push timestamp: {timestamp} -->"
                    new_content = content.rstrip() + comment
                    with open(selected_file, 'w', encoding='utf-8') as f:
                        f.write(new_content)
                    print(f"✅ Added timestamp comment to {selected_file}")
                else:
                    # For binary files, add a newline at the end if possible, or skip
                    try:
                        # Try to append a newline (safe for most binary formats)
                        with open(selected_file, 'ab') as f:
                            f.write(b'\n')
                        print(f"✅ Added newline to {selected_file}")
                    except Exception:
                        # If we can't modify it, skip this file
                        print(f"⚠️  Cannot safely modify binary file: {selected_file}")
                        state["pushed_files"].append(selected_file)
                        save_state(state)
                        return
            except Exception as e:
                print(f"⚠️  Could not modify {selected_file}: {e}")
                # Just touch the file
                os.utime(selected_file, None)
        
        # Add and commit the single file
        run_cmd(f"git add '{selected_file}'")
        
        # Check if there are actually changes to commit
        status = run_cmd("git status --porcelain")
        if status:
            commit_message = f"Automated commit: {selected_file} on {today}"
            # Use exit_on_error=False for commit to handle "nothing to commit" gracefully
            try:
                run_cmd(f'git commit -m "{commit_message}"', exit_on_error=False)
            except Exception as commit_error:
                # If commit fails (e.g., nothing to commit), check status again
                status_after = run_cmd("git status --porcelain")
                if not status_after:
                    print("⚡ No changes detected after modification - file may be identical or ignored by git")
                    state["pushed_files"].append(selected_file)
                    save_state(state)
                    return
                else:
                    # Re-raise if there's a real error
                    raise
            
            # Try to push with better error handling
            try:
                # Remote URL should already be set up with token from earlier
                # But ensure it's still set (in case of any issues)
                if github_token:
                    remote_url = run_cmd("git remote get-url origin", exit_on_error=False)
                    if remote_url and not remote_url.startswith(f"https://{github_token}@"):
                        if remote_url.startswith("https://github.com/"):
                            auth_url = remote_url.replace("https://github.com/", f"https://{github_token}@github.com/")
                            run_cmd(f"git remote set-url origin {auth_url}")
                
                push_result = run_cmd(f"git push origin {branch_name}", exit_on_error=False)
                
                # Update state only on successful push
                state["pushes_today"] += 1
                state["pushed_files"].append(selected_file)
                save_state(state)
                
                success_msg = f"✅ Auto-push succeeded: {selected_file} pushed to {branch_name} (Push #{state['pushes_today']}/{PUSHES_PER_DAY})"
                print(success_msg)
                send_webhook(success_msg)
                
            except Exception as push_error:
                error_msg = f"❌ Push failed: {str(push_error)}"
                print(error_msg)
                
                # Check if it's a GitHub push protection error
                if "GH013" in str(push_error) or "repository rule violations" in str(push_error).lower():
                    print("🔒 GitHub push protection detected - skipping this file")
                    print("💡 This file may contain secrets and was blocked by GitHub")
                    # Don't update state since push failed
                else:
                    # For other errors, still update state to avoid retrying the same file
                    state["pushes_today"] += 1
                    state["pushed_files"].append(selected_file)
                    save_state(state)
                
                send_webhook(f"🚨 Auto Push Failed: {error_msg}")
        else:
            print("⚡ No changes to commit for selected file.")
            # Still mark as processed to avoid infinite retry
            state["pushed_files"].append(selected_file)
            save_state(state)
            
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
