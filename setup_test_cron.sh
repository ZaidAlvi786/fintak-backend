#!/bin/bash

# Test Cron Job Setup for Auto Push
# This script sets up a test cron job to run at 11:20 PM Pakistan time today

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"

echo "🧪 Setting up TEST cron job for 11:20 PM Pakistan time..."

# Check if Python script exists
if [ ! -f "$PYTHON_SCRIPT" ]; then
    echo "❌ Error: auto_push.py not found at $PYTHON_SCRIPT"
    exit 1
fi

# Install Python dependencies (if not already installed)
echo "📦 Ensuring Python dependencies are installed..."
pip3 install -r "$SCRIPT_DIR/requirements.txt"

# Remove any existing test cron jobs
echo "🧹 Cleaning up existing test cron jobs..."
crontab -l 2>/dev/null | grep -v "TEST_AUTO_PUSH" | crontab -

# Add test cron job for 11:20 PM today
echo "⏰ Adding test cron job for 11:20 PM Pakistan time..."
(
    crontab -l 2>/dev/null
    echo "# TEST_AUTO_PUSH - Test cron job for 11:20 PM Pakistan time"
    echo "20 23 * * * cd $SCRIPT_DIR && python3 auto_push.py >> auto_push_test.log 2>&1"
) | crontab -

echo "✅ Test cron job has been set up successfully!"
echo ""
echo "📋 Test cron job details:"
echo "   - Time: 11:20 PM Pakistan time (23:20)"
echo "   - Command: cd $SCRIPT_DIR && python3 auto_push.py"
echo "   - Log file: auto_push_test.log"
echo ""
echo "📋 Current cron jobs:"
crontab -l | grep -E "(TEST_AUTO_PUSH|auto_push)"

echo ""
echo "🔧 Useful commands:"
echo "   - View test logs: tail -f $SCRIPT_DIR/auto_push_test.log"
echo "   - Test manually now: cd $SCRIPT_DIR && python3 auto_push.py"
echo "   - Remove test cron: crontab -e (delete TEST_AUTO_PUSH line)"
echo ""
echo "⏰ The test will run in about 12 minutes (at 11:20 PM PKT)"
echo "📊 You can monitor the logs in real-time with: tail -f auto_push_test.log"
