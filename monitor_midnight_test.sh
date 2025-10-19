#!/bin/bash

# Monitor Midnight Test
# This script helps you monitor the midnight test execution

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOG_FILE="$SCRIPT_DIR/auto_push_test.log"
STATE_FILE="$SCRIPT_DIR/auto_push_state.json"

echo "🌙 MIDNIGHT TEST MONITOR"
echo "========================"
echo ""

# Show current time
echo "⏰ Current time: $(date)"
echo "🎯 Test scheduled for: 12:10 AM Pakistan time (00:10)"
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
echo "  - Test manually: python3 test_midnight_push.py"
echo "  - Remove test cron: crontab -e (delete TEST_AUTO_PUSH line)"
echo ""

# Countdown to 12:10 AM
current_time=$(date +%H:%M)
target_time="00:10"

if [ "$current_time" != "$target_time" ]; then
    echo "⏳ Time until test execution:"
    echo "   Current: $current_time"
    echo "   Target:  $target_time"
    echo ""
    echo "💡 The test will run automatically at 12:10 AM PKT"
    echo "   This test WILL actually push 1 file (schedule bypassed for testing)"
    echo "   You can monitor it with: tail -f $LOG_FILE"
    echo ""
    echo "🎯 Expected behavior:"
    echo "   1. Create branch: bug-fixes-2025-10-20"
    echo "   2. Select 1 random file"
    echo "   3. Commit and push the file"
    echo "   4. Log all activity"
fi
