#!/bin/bash

# Auto Push Cron Setup Script
# This script sets up cron jobs for the auto_push.py script

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"

echo "🚀 Setting up Auto Push Cron Jobs..."

# Check if Python script exists
if [ ! -f "$PYTHON_SCRIPT" ]; then
    echo "❌ Error: auto_push.py not found at $PYTHON_SCRIPT"
    exit 1
fi

# Install Python dependencies
echo "📦 Installing Python dependencies..."
pip3 install -r "$SCRIPT_DIR/requirements.txt"

# Create cron job entries
echo "⏰ Setting up cron jobs..."

# Remove existing auto_push cron jobs
crontab -l 2>/dev/null | grep -v "auto_push.py" | crontab -

# Add new cron jobs - every 2 hours between 10 AM and 10 PM (7 times total)
# This ensures the script runs even if the system is restarted
(
    crontab -l 2>/dev/null
    echo "# Auto Push Cron Jobs - Runs every 2 hours between 10 AM and 10 PM"
    echo "0 10 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 12 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 14 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 16 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 18 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 20 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 22 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push.log 2>&1"
) | crontab -

echo "✅ Cron jobs have been set up successfully!"
echo "📋 Current cron jobs:"
crontab -l | grep auto_push

echo ""
echo "📝 Notes:"
echo "   - Script will run 7 times per day between 10 AM and 10 PM"
echo "   - Each run pushes one file to a daily branch"
echo "   - Logs are saved to auto_push.log"
echo "   - State is maintained in auto_push_state.json"
echo ""
echo "🔧 To view logs: tail -f $SCRIPT_DIR/auto_push.log"
echo "🔧 To remove cron jobs: crontab -e (then delete auto_push lines)"
echo "🔧 To test manually: cd $SCRIPT_DIR && python3 auto_push.py"
