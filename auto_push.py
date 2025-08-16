import subprocess
import datetime
import random
import os
import json
import requests  # install via pip

WEBHOOK_URL = "https://hooks.slack.com/services/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"

def send_webhook(message):
    """Send a message to Slack/Discord via webhook"""
    if not WEBHOOK_URL:
        print("⚠️ WEBHOOK_URL not set")
        return
    data = {"content": message}  # Discord
    # For Slack, use: data = {"text": message}
    try:
        response = requests.post(WEBHOOK_URL, data=json.dumps(data), headers={"Content-Type": "application/json"})
        if response.status_code == 200 or response.status_code == 204:
            print("📢 Notification sent successfully!")
        else:
            print(f"❌ Failed to send notification: {response.text}")
    except Exception as e:
        print(f"❌ Exception sending webhook: {e}")

def run_cmd(cmd):
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        error_msg = f"Error running command: {cmd}\n{result.stderr}"
        print(error_msg)
        send_webhook(f"🚨 Auto Push Failed:\n{error_msg}")
        exit(1)
    return result.stdout.strip()

# ----------------- rest of your auto_push.py -----------------

def main():
    today = datetime.datetime.now().strftime("%Y-%m-%d")
    branch_name = f"bug-fixes-{today}"

    try:
        run_cmd("git fetch origin")
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
            print(f"✅ Pushed {len(selected)} files to branch {branch_name}")
        else:
            print("⚡ No changes to commit after selecting files.")
    except Exception as e:
        send_webhook(f"🚨 Auto Push Script Failed:\n{str(e)}")
        raise

if __name__ == "__main__":
    main()
