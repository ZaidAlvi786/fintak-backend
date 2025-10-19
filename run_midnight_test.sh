#!/bin/bash
# Midnight Test Wrapper Script for Cron
export PATH="/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin"
export HOME="$HOME"
cd "/Users/apple/Downloads/demo/fintak-backend"
exec "/usr/local/bin/python3" "test_midnight_push.py" >> "/Users/apple/Downloads/demo/fintak-backend/auto_push_test.log" 2>&1
