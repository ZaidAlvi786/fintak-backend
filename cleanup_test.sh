#!/bin/bash

# Cleanup Test Cron Job
# This script removes the test cron job after testing

echo "🧹 Cleaning up test cron job..."

# Remove test cron job
crontab -l 2>/dev/null | grep -v "TEST_AUTO_PUSH" | crontab -

echo "✅ Test cron job removed successfully!"
echo ""
echo "📋 Remaining cron jobs:"
crontab -l 2>/dev/null || echo "No cron jobs found"
echo ""
echo "📁 Test files created:"
echo "  - auto_push_test.log (test logs)"
echo "  - auto_push_state.json (state file)"
echo ""
echo "🔧 To clean up test files:"
echo "  - Remove logs: rm auto_push_test.log"
echo "  - Reset state: rm auto_push_state.json"
echo "  - Or keep them for reference"
