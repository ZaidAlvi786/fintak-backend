#!/bin/bash

# Midnight Test Cron Job Setup
# This script sets up a test cron job for 12:10 AM (00:10) to test the system

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"
PYTHON_PATH=$(which python3)

echo "🌙 Setting up MIDNIGHT test cron job for 12:10 AM (00:10)..."

# Check if Python script exists
if [ ! -f "$PYTHON_SCRIPT" ]; then
    echo "❌ Error: auto_push.py not found at $PYTHON_SCRIPT"
    exit 1
fi

# Create a wrapper script that sets up the environment properly
WRAPPER_SCRIPT="$SCRIPT_DIR/run_auto_push.sh"
cat > "$WRAPPER_SCRIPT" << EOF
#!/bin/bash
# Auto Push Wrapper Script for Cron
export PATH="/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin"
export HOME="$HOME"
cd "$SCRIPT_DIR"
exec "$PYTHON_PATH" "$PYTHON_SCRIPT" >> "$SCRIPT_DIR/auto_push_test.log" 2>&1
EOF

chmod +x "$WRAPPER_SCRIPT"

# Remove any existing test cron jobs
echo "🧹 Cleaning up existing test cron jobs..."
crontab -l 2>/dev/null | grep -v "TEST_AUTO_PUSH" | crontab -

# Add test cron job for 12:10 AM (00:10)
echo "⏰ Adding midnight test cron job for 12:10 AM..."
(
    crontab -l 2>/dev/null
    echo "# TEST_AUTO_PUSH - Midnight test at 12:10 AM"
    echo "10 0 * * * $WRAPPER_SCRIPT"
) | crontab -

echo "✅ Midnight test cron job has been set up successfully!"
echo ""
echo "📋 Test details:"
echo "   - Time: 12:10 AM Pakistan time (00:10)"
echo "   - This is OUTSIDE the 10 AM - 10 PM schedule"
echo "   - Expected behavior: Should skip due to schedule restriction"
echo "   - Wrapper script: $WRAPPER_SCRIPT"
echo "   - Log file: auto_push_test.log"
echo ""
echo "📋 Current cron jobs:"
crontab -l

echo ""
echo "🔧 Test the wrapper script now (will show schedule restriction):"
echo "   $WRAPPER_SCRIPT"
echo ""
echo "🔧 Monitor logs:"
echo "   tail -f $SCRIPT_DIR/auto_push_test.log"
echo ""
echo "💡 This test will verify:"
echo "   1. Cron job execution"
echo "   2. Wrapper script functionality"
echo "   3. Schedule restriction (should skip at 12:10 AM)"
echo "   4. Logging system"
