import subprocess
import datetime
import random
import os
import json
import requests  # installed via requirements.txt

WEBHOOK_URL = ""  # Set your webhook URL here or leave empty to disable notifications

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

def main():
    today = datetime.datetime.now().strftime("%Y-%m-%d")
    branch_name = f"bug-fixes-{today}"

    try:
        run_cmd("git fetch origin")
        # Check if branch exists and switch to it, otherwise create new branch
        try:
            run_cmd(f"git checkout {branch_name}", exit_on_error=False)
        except:
            run_cmd(f"git checkout -b {branch_name}")

        candidates = get_candidate_files()
        if not candidates:
            print("⚡ No files to commit.")
            return

        selected = random.sample(candidates, min(7, len(candidates)))
        print(f"📂 Selected files for commit: {selected}")

        for f in selected:
            run_cmd(f"git add '{f}'")

        status = run_cmd("git status --porcelain")
        if status:
            commit_message = f"Automated commit ({len(selected)} files) on {today}"
            run_cmd(f'git commit -m "{commit_message}"')
            run_cmd(f"git push origin {branch_name}")
            success_msg = f"✅ Auto-push succeeded: {len(selected)} files pushed to {branch_name}"
            print(success_msg)
            send_webhook(success_msg)
        else:
            print("⚡ No changes to commit after selecting files.")
    except Exception as e:
        send_webhook(f"🚨 Auto Push Script Failed:\n{str(e)}")
        raise

if __name__ == "__main__":
    main()
