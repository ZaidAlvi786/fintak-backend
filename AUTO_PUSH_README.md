# Auto Push System

This system automatically pushes one file at a time to daily branches, running 7 times per day between 10 AM and 10 PM.

## Features

- ✅ **One file per push**: Each execution commits and pushes exactly one file
- ✅ **Daily branches**: Creates new branch `bug-fixes-YYYY-MM-DD` each day
- ✅ **Scheduled execution**: 7 pushes per day between 10 AM and 10 PM
- ✅ **System reliability**: Works with cron jobs and system services
- ✅ **State persistence**: Remembers what files have been pushed today
- ✅ **Webhook notifications**: Optional Slack/Discord notifications
- ✅ **Error handling**: Comprehensive error handling and logging

## Quick Setup

1. **Install dependencies**:
   ```bash
   pip3 install -r requirements.txt
   ```

2. **Run setup script**:
   ```bash
   ./setup_auto_push.sh
   ```

3. **Test manually**:
   ```bash
   python3 auto_push.py
   ```

## Configuration

Edit `auto_push.py` to modify:

```python
PUSHES_PER_DAY = 7        # Number of pushes per day
START_HOUR = 10           # Start time (10 AM)
END_HOUR = 22             # End time (10 PM)
WEBHOOK_URL = ""          # Slack/Discord webhook URL
```

## Usage Modes

### 1. Cron Jobs (Recommended)
- Runs every 2 hours between 10 AM and 10 PM
- Survives system restarts
- Reliable and lightweight

### 2. System Service
- **Linux**: Uses systemd service
- **macOS**: Uses launchd service
- Runs continuously with automatic restart

### 3. Manual Execution
```bash
# Single push
python3 auto_push.py

# Continuous scheduler
python3 auto_push.py --scheduler
```

## File Structure

```
fintak-backend/
├── auto_push.py              # Main script
├── requirements.txt          # Python dependencies
├── setup_auto_push.sh        # Comprehensive setup script
├── setup_cron.sh            # Cron-only setup script
├── auto_push.service        # Systemd service file
├── auto_push_state.json     # State file (created automatically)
└── auto_push.log           # Log file (created automatically)
```

## State Management

The system maintains state in `auto_push_state.json`:

```json
{
  "current_branch": "bug-fixes-2024-01-15",
  "last_push_date": "2024-01-15",
  "pushes_today": 3,
  "pushed_files": ["file1.cs", "file2.cs", "file3.cs"]
}
```

## Monitoring

### View Logs
```bash
tail -f auto_push.log
```

### Check State
```bash
cat auto_push_state.json
```

### Check Cron Jobs
```bash
crontab -l
```

### Check Service Status (Linux)
```bash
sudo systemctl status auto_push
```

## Troubleshooting

### Common Issues

1. **ModuleNotFoundError: No module named 'requests'**
   ```bash
   pip3 install -r requirements.txt
   ```

2. **Git authentication issues**
   - Ensure SSH keys are set up
   - Or use HTTPS with stored credentials

3. **Permission denied**
   ```bash
   chmod +x setup_auto_push.sh
   chmod +x setup_cron.sh
   ```

4. **Cron jobs not running**
   - Check cron service is running
   - Verify file paths in crontab
   - Check logs in `auto_push.log`

### Reset State
```bash
rm auto_push_state.json
```

### Disable System
```bash
# Remove cron jobs
crontab -e  # Delete auto_push lines

# Stop systemd service (Linux)
sudo systemctl stop auto_push
sudo systemctl disable auto_push

# Unload launchd service (macOS)
launchctl unload ~/Library/LaunchAgents/com.auto_push.plist
```

## Webhook Notifications

Set `WEBHOOK_URL` in `auto_push.py` to receive notifications:

- **Slack**: Use Slack webhook URL
- **Discord**: Use Discord webhook URL (change `"text"` to `"content"` in code)

## Branch Naming

Daily branches follow the pattern: `bug-fixes-YYYY-MM-DD`

Example: `bug-fixes-2024-01-15`

## File Selection

The system:
1. Gets all tracked and untracked files
2. Filters out files already pushed today
3. Randomly selects one file per push
4. Commits and pushes the selected file

## Schedule

- **Frequency**: 7 pushes per day
- **Hours**: 10 AM to 10 PM
- **Interval**: Approximately every 2 hours
- **Days**: Every day of the week

## Security Notes

- Script runs with current user permissions
- No sensitive data is logged
- Git operations use existing authentication
- State file contains only file paths and metadata
