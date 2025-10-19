#!/bin/bash

# Fixed Test Cron Job Setup for Auto Push
# This script sets up a test cron job with proper permissions for macOS

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"
PYTHON_PATH=$(which python3)

echo "🔧 Setting up FIXED test cron job for macOS..."

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

# Add test cron job using the wrapper script
echo "⏰ Adding fixed test cron job..."
(
    crontab -l 2>/dev/null
    echo "# TEST_AUTO_PUSH - Fixed test cron job for macOS"
    echo "20 23 * * * $WRAPPER_SCRIPT"
) | crontab -

echo "✅ Fixed test cron job has been set up successfully!"
echo ""
echo "📋 Fixed cron job details:"
echo "   - Time: 11:20 PM Pakistan time (23:20)"
echo "   - Wrapper script: $WRAPPER_SCRIPT"
echo "   - Python path: $PYTHON_PATH"
echo "   - Working directory: $SCRIPT_DIR"
echo "   - Log file: auto_push_test.log"
echo ""
echo "📋 Current cron jobs:"
crontab -l | grep -E "(TEST_AUTO_PUSH|auto_push)"

echo ""
echo "🔧 Useful commands:"
echo "   - View test logs: tail -f $SCRIPT_DIR/auto_push_test.log"
echo "   - Test wrapper manually: $WRAPPER_SCRIPT"
echo "   - Test script directly: cd $SCRIPT_DIR && python3 auto_push.py"
echo "   - Remove test cron: crontab -e (delete TEST_AUTO_PUSH line)"
echo ""
echo "💡 The wrapper script handles environment setup for cron jobs on macOS"
