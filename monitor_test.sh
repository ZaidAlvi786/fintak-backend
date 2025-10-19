#!/bin/bash

# Monitor Test Cron Job
# This script helps you monitor the test cron job execution

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOG_FILE="$SCRIPT_DIR/auto_push_test.log"
STATE_FILE="$SCRIPT_DIR/auto_push_state.json"

echo "🔍 Auto Push Test Monitor"
echo "========================="
echo ""

# Show current time
echo "⏰ Current time: $(date)"
echo "🎯 Test scheduled for: 11:20 PM Pakistan time"
echo ""

# Check if log file exists
if [ -f "$LOG_FILE" ]; then
    echo "📋 Recent log entries:"
    echo "---------------------"
    tail -10 "$LOG_FILE"
    echo ""
else
    echo "📝 Log file doesn't exist yet (will be created when cron runs)"
    echo ""
fi

# Check state file
if [ -f "$STATE_FILE" ]; then
    echo "📊 Current state:"
    echo "----------------"
    cat "$STATE_FILE" | python3 -m json.tool 2>/dev/null || cat "$STATE_FILE"
    echo ""
else
    echo "📊 No state file yet (will be created on first run)"
    echo ""
fi

# Show cron job
echo "⏰ Active cron job:"
echo "------------------"
crontab -l | grep TEST_AUTO_PUSH
echo ""

echo "🔧 Monitoring commands:"
echo "  - Watch logs in real-time: tail -f $LOG_FILE"
echo "  - Check state: cat $STATE_FILE"
echo "  - Test manually: python3 auto_push.py"
echo "  - Remove test cron: crontab -e (delete TEST_AUTO_PUSH line)"
echo ""

# Countdown to 11:20 PM
current_time=$(date +%H:%M)
target_time="23:20"

if [ "$current_time" != "$target_time" ]; then
    echo "⏳ Time until test execution:"
    echo "   Current: $current_time"
    echo "   Target:  $target_time"
    echo ""
    echo "💡 The test will run automatically at 11:20 PM PKT"
    echo "   You can monitor it with: tail -f $LOG_FILE"
fi
