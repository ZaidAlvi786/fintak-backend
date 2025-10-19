#!/bin/bash

# Tomorrow's Test Cron Job Setup
# This script sets up a test cron job for tomorrow morning at 10:30 AM

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"
PYTHON_PATH=$(which python3)

echo "🌅 Setting up test cron job for tomorrow morning (10:30 AM)..."

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

# Add test cron job for tomorrow at 10:30 AM
echo "⏰ Adding test cron job for tomorrow 10:30 AM..."
(
    crontab -l 2>/dev/null
    echo "# TEST_AUTO_PUSH - Tomorrow morning test at 10:30 AM"
    echo "30 10 * * * $WRAPPER_SCRIPT"
) | crontab -

echo "✅ Tomorrow's test cron job has been set up successfully!"
echo ""
echo "📋 Test details:"
echo "   - Time: Tomorrow 10:30 AM Pakistan time"
echo "   - This is within the 10 AM - 10 PM schedule"
echo "   - Wrapper script: $WRAPPER_SCRIPT"
echo "   - Log file: auto_push_test.log"
echo ""
echo "📋 Current cron jobs:"
crontab -l

echo ""
echo "🔧 Test the wrapper script now:"
echo "   $WRAPPER_SCRIPT"
echo ""
echo "🔧 Monitor logs:"
echo "   tail -f $SCRIPT_DIR/auto_push_test.log"
