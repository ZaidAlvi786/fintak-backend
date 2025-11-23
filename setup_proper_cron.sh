#!/bin/bash

# Proper Auto Push Cron Setup Script
# This script sets up reliable cron jobs for the auto_push.py script

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"

echo "🚀 Setting up Proper Auto Push Cron Jobs..."

# Check if Python script exists
if [ ! -f "$PYTHON_SCRIPT" ]; then
    echo "❌ Error: auto_push.py not found at $PYTHON_SCRIPT"
    exit 1
fi

# Install Python dependencies
echo "📦 Installing Python dependencies..."
pip3 install -r "$SCRIPT_DIR/requirements.txt" 2>/dev/null || echo "⚠️  Could not install requirements.txt (file may not exist)"

# Remove ALL existing auto_push cron jobs
echo "🧹 Cleaning up existing cron jobs..."
crontab -l 2>/dev/null | grep -v "auto_push.py" | grep -v "run_midnight_test.sh" | crontab -

# Add new cron jobs - every 2 hours between 10 AM and 10 PM (7 times total)
echo "⏰ Setting up new cron jobs..."
(
    crontab -l 2>/dev/null
    echo "# Auto Push Cron Jobs - Runs every 2 hours between 10 AM and 10 PM"
    echo "0 10 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 12 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 14 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 16 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 18 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 20 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
    echo "0 22 * * * cd $SCRIPT_DIR && /usr/bin/python3 auto_push.py >> auto_push.log 2>&1"
) | crontab -

echo "✅ Cron jobs have been set up successfully!"
echo "📋 Current cron jobs:"
crontab -l | grep -E "(auto_push|midnight)"

echo ""
echo "📝 Configuration Summary:"
echo "   - Script runs 7 times per day: 10 AM, 12 PM, 2 PM, 4 PM, 6 PM, 8 PM, 10 PM"
echo "   - Each run pushes one file to a daily branch (bug-fixes-YYYY-MM-DD)"
echo "   - Logs are saved to auto_push.log"
echo "   - State is maintained in auto_push_state.json"
echo "   - Uses absolute path to python3 for reliability"
echo ""
echo "🔧 Useful commands:"
echo "   - Test manually: cd $SCRIPT_DIR && python3 auto_push.py"
echo "   - View logs: tail -f $SCRIPT_DIR/auto_push.log"
echo "   - Check state: cat $SCRIPT_DIR/auto_push_state.json"
echo "   - View all cron jobs: crontab -l"
echo "   - Remove cron jobs: crontab -e (then delete auto_push lines)"
echo ""
echo "✅ Auto Push system is ready and will start working during business hours!"

