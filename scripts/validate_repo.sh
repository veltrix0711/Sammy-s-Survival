#!/usr/bin/env bash
set -e
missing=0
check() { [ -d "$1" ] || { echo "Missing dir: $1"; missing=1; }; }
check Game || true
check Docs || true
check .github || true
exit $missing


