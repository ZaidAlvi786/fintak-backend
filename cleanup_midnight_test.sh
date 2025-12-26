#!/bin/bash

# Cleanup Midnight Test
# This script removes the midnight test cron job and cleans up test files

echo "🧹 Cleaning up midnight test..."

# Remove test cron job
crontab -l 2>/dev/null | grep -v "TEST_AUTO_PUSH" | crontab -

echo "✅ Midnight test cron job removed successfully!"
echo ""
echo "📋 Remaining cron jobs:"
crontab -l 2>/dev/null || echo "No cron jobs found"
echo ""
echo "📁 Test files created:"
echo "  - auto_push_test.log (test logs)"
echo "  - auto_push_state.json (state file)"
echo "  - test_midnight_push.py (test script)"
echo "  - run_midnight_test.sh (wrapper script)"
echo ""
echo "🔧 To clean up test files:"
echo "  - Remove logs: rm auto_push_test.log"
echo "  - Reset state: rm auto_push_state.json"
echo "  - Remove test scripts: rm test_midnight_push.py run_midnight_test.sh"
echo "  - Or keep them for reference"
echo ""
echo "📊 Final test results:"
if [ -f "auto_push_test.log" ]; then
    echo "  - Log file size: $(wc -l < auto_push_test.log) lines"
    echo "  - Last entry: $(tail -1 auto_push_test.log)"
fi

<!-- Auto-push timestamp: 2025-12-26 14:10:32 -->