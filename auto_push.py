import subprocess
import datetime
import random
import smtplib
from email.mime.text import MIMEText

# ========== CONFIG ==========
SMTP_SERVER = "smtp.gmail.com"   # e.g., Gmail SMTP
SMTP_PORT = 587
EMAIL_USER = "itsabout786@gmail.com"    # sender email
EMAIL_PASS = "alvig786"       # Gmail app password (not your main password!)
EMAIL_TO   = "zaidalviza786@gmail.com"     # receiver email
# ============================

def send_email(subject, body):
    """Send email notification"""
    msg = MIMEText(body)
    msg["Subject"] = subject
    msg["From"] = EMAIL_USER
    msg["To"] = EMAIL_TO

    try:
        with smtplib.SMTP(SMTP_SERVER, SMTP_PORT) as server:
            server.starttls()
            server.login(EMAIL_USER, EMAIL_PASS)
            server.sendmail(EMAIL_USER, EMAIL_TO, msg.as_string())
        print("📧 Error notification sent to", EMAIL_TO)
    except Exception as e:
        print("❌ Failed to send email:", e)

def run_cmd(cmd):
    """Run a shell command and return output or raise error."""
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        error_msg = f"Error running command: {cmd}\n{result.stderr}"
        print(error_msg)
        send_email("🚨 Auto Push Failed", error_msg)
        exit(1)
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
        send_email("🚨 Auto Push Script Failed", str(e))
        raise

if __name__ == "__main__":
    main()
