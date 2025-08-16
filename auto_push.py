import subprocess
import datetime
import random

def run_cmd(cmd):
    """Run a shell command and return output or raise error."""
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"Error running command: {cmd}\n{result.stderr}")
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

    # Fetch latest branches
    run_cmd("git fetch origin")

    # Create new branch from main
    # run_cmd("git checkout main")
    # run_cmd("git pull origin main")
    run_cmd(f"git checkout -b {branch_name}")

    # Pick 7 random files
    candidates = get_candidate_files()
    if not candidates:
        print("⚡ No files to commit.")
        return

    selected = random.sample(candidates, min(7, len(candidates)))
    print(f"📂 Selected files for commit: {selected}")

    # Stage only selected files
    for f in selected:
        run_cmd(f"git add '{f}'")

    # Commit & push if changes exist
    status = run_cmd("git status --porcelain")
    if status:
        commit_message = f"Automated commit ({len(selected)} files) on {today}"
        run_cmd(f'git commit -m "{commit_message}"')
        run_cmd(f"git push origin {branch_name}")
        print(f"✅ Pushed {len(selected)} files to branch {branch_name}")
    else:
        print("⚡ No changes to commit after selecting files.")

if __name__ == "__main__":
    main()
