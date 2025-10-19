#!/bin/bash

# Comprehensive Auto Push Setup Script
# This script sets up both cron jobs and systemd service for maximum reliability

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/auto_push.py"
SERVICE_FILE="$SCRIPT_DIR/auto_push.service"

echo "🚀 Setting up Auto Push System..."

# Check if Python script exists
if [ ! -f "$PYTHON_SCRIPT" ]; then
    echo "❌ Error: auto_push.py not found at $PYTHON_SCRIPT"
    exit 1
fi

# Install Python dependencies
echo "📦 Installing Python dependencies..."
pip3 install -r "$SCRIPT_DIR/requirements.txt"

# Function to setup cron jobs
setup_cron() {
    echo "⏰ Setting up cron jobs..."
    
    # Remove existing auto_push cron jobs
    crontab -l 2>/dev/null | grep -v "auto_push.py" | crontab -
    
    # Add new cron jobs - every 2 hours between 10 AM and 10 PM (7 times total)
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
    
    echo "✅ Cron jobs set up successfully!"
}

# Function to setup systemd service (Linux only)
setup_systemd() {
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        echo "🔧 Setting up systemd service..."
        
        # Copy service file to systemd directory
        sudo cp "$SERVICE_FILE" /etc/systemd/system/
        
        # Reload systemd and enable service
        sudo systemctl daemon-reload
        sudo systemctl enable auto_push.service
        
        echo "✅ Systemd service set up successfully!"
        echo "🔧 To start service: sudo systemctl start auto_push"
        echo "🔧 To check status: sudo systemctl status auto_push"
    else
        echo "⚠️  Systemd not available on this system (macOS/Windows)"
    fi
}

# Function to setup launchd (macOS only)
setup_launchd() {
    if [[ "$OSTYPE" == "darwin"* ]]; then
        echo "🍎 Setting up launchd service for macOS..."
        
        # Create plist file for launchd
        cat > "$HOME/Library/LaunchAgents/com.auto_push.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.auto_push</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/bin/python3</string>
        <string>$PYTHON_SCRIPT</string>
        <string>--scheduler</string>
    </array>
    <key>WorkingDirectory</key>
    <string>$SCRIPT_DIR</string>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>$SCRIPT_DIR/auto_push.log</string>
    <key>StandardErrorPath</key>
    <string>$SCRIPT_DIR/auto_push.log</string>
</dict>
</plist>
EOF
        
        # Load the service
        launchctl load "$HOME/Library/LaunchAgents/com.auto_push.plist"
        
        echo "✅ Launchd service set up successfully!"
        echo "🔧 To unload service: launchctl unload ~/Library/LaunchAgents/com.auto_push.plist"
    fi
}

# Main setup
echo "Choose setup method:"
echo "1) Cron jobs only (recommended for most systems)"
echo "2) System service (systemd/launchd) + cron backup"
echo "3) Both methods"

read -p "Enter choice (1-3): " choice

case $choice in
    1)
        setup_cron
        ;;
    2)
        if [[ "$OSTYPE" == "linux-gnu"* ]]; then
            setup_systemd
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            setup_launchd
        else
            echo "⚠️  System service not supported on this OS, falling back to cron"
            setup_cron
        fi
        ;;
    3)
        setup_cron
        if [[ "$OSTYPE" == "linux-gnu"* ]]; then
            setup_systemd
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            setup_launchd
        fi
        ;;
    *)
        echo "❌ Invalid choice, setting up cron jobs only"
        setup_cron
        ;;
esac

echo ""
echo "📋 Setup Summary:"
echo "   - Script pushes one file at a time"
echo "   - Creates daily branches (bug-fixes-YYYY-MM-DD)"
echo "   - Runs 7 times per day between 10 AM and 10 PM"
echo "   - State maintained in auto_push_state.json"
echo "   - Logs saved to auto_push.log"
echo ""
echo "🔧 Useful commands:"
echo "   - Test manually: cd $SCRIPT_DIR && python3 auto_push.py"
echo "   - View logs: tail -f $SCRIPT_DIR/auto_push.log"
echo "   - Check state: cat $SCRIPT_DIR/auto_push_state.json"
echo "   - View cron jobs: crontab -l"
echo ""
echo "✅ Auto Push system is ready!"
