import os
import subprocess
import json

# Load secret replacements
with open("secrets_map.json") as f:
    secrets_map = json.load(f)

def replace_secrets_in_file(filepath):
    """Replaces all known secrets with dummy values in a file."""
    with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
        content = f.read()

    original_content = content
    for real, dummy in secrets_map.items():
        content = content.replace(real, dummy)

    if content != original_content:
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(content)
        print(f"🔑 Replaced secrets in {filepath}")

def get_files_to_commit():
    """Get tracked and untracked files, skip ignored."""
    tracked = subprocess.check_output(
        ["git", "ls-files"], encoding="utf-8"
    ).splitlines()

    untracked = subprocess.check_output(
        ["git", "ls-files", "--others", "--exclude-standard"], encoding="utf-8"
    ).splitlines()

    return tracked + untracked

def main():
    files = get_files_to_commit()

    for filepath in files:
        if os.path.isfile(filepath):
            replace_secrets_in_file(filepath)

    # Git commit + push
    subprocess.run(["git", "add", "."])
    subprocess.run(["git", "commit", "-m", "Auto commit with secrets replaced"], check=False)
    subprocess.run(["git", "push"], check=True)

if __name__ == "__main__":
    main()
