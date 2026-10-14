#!/usr/bin/env bash
# Launch generated Blazor host, verify Studio assets and runtime routes.
set -euo pipefail
cd "$(dirname "$0")/.."
port="${CLARIFY_SMOKE_PORT:-5167}"
log="$(mktemp)"
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-build -c Release --project src/Clarify.Demo.Web --urls "http://127.0.0.1:$port" >"$log" 2>&1 &
pid=$!
cleanup(){ kill "$pid" 2>/dev/null || true; wait "$pid" 2>/dev/null || true; cat "$log"; rm -f "$log"; }
trap cleanup EXIT
for i in $(seq 1 50); do
  if curl --silent --fail "http://127.0.0.1:$port/studio" >/dev/null; then break; fi
  if ! kill -0 "$pid" 2>/dev/null; then echo "Web host exited during startup" >&2; exit 1; fi
  sleep .4
done
for route in /studio /studio/schema /studio/designer.html /studio/clarifydsl.mjs /studio/dictionary.json /customers /orders; do
  curl --silent --fail --max-time 8 "http://127.0.0.1:$port$route" >/dev/null ||
     { echo "FAILED HTTP route $route" >&2; exit 1; }
  echo "PASS GET $route"
done
echo "CLARIFY_WEB_SMOKE_PASSED"
