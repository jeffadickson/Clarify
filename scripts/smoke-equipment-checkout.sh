#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
port="${CLARIFY_CHECKOUT_PORT:-5281}"
tmp="$(mktemp -d)"
pid=""
cleanup(){ if [[ -n "$pid" ]]; then kill "$pid" 2>/dev/null || true; wait "$pid" 2>/dev/null || true; fi; rm -rf "$tmp"; }
trap cleanup EXIT
# Run in sample working directory; preserve the user's own working database.
cp -R examples/equipment-checkout/. "$tmp/"
rm -f "$tmp/equipment-checkout.sqlite"
# Generated context is included in the copied project by default SDK globbing.
(cd "$tmp" && dotnet run --project EquipmentCheckout.csproj -c Release --urls "http://127.0.0.1:$port" >"$tmp/server.log" 2>&1) &
pid=$!
for i in $(seq 1 60); do
 if curl --silent --fail "http://127.0.0.1:$port/api/snapshot" >/dev/null; then break; fi
 if ! kill -0 "$pid" 2>/dev/null; then cat "$tmp/server.log"; exit 1; fi
 sleep 1
done
CLARIFY_CHECKOUT_URL="http://127.0.0.1:$port" python3 - <<'PY'
import json,os,urllib.request,urllib.error
url=os.environ["CLARIFY_CHECKOUT_URL"]
def req(path,payload=None):
 data=None if payload is None else json.dumps(payload).encode()
 request=urllib.request.Request(url+path,data=data,headers={"Content-Type":"application/json"})
 try:
  with urllib.request.urlopen(request,timeout=10) as r:return r.status,json.load(r)
 except urllib.error.HTTPError as e:return e.code,json.load(e)
status,initial=req("/api/snapshot")
assert status==200 and len(initial["equipment"])>=3 and len(initial["people"])>=2
person=initial["people"][0]["id"]
equipment=initial["equipment"][0]["id"]
status,loan=req("/api/loans",{"personId":person,"equipmentId":equipment})
assert status==200, (status,loan)
status,conflict=req("/api/loans",{"personId":person,"equipmentId":equipment})
assert status==409, (status,conflict)
status,ret=req("/api/loans/"+str(loan["id"])+"/return",{})
assert status==200 and ret["returned"]
status,_=req("/api/loans/"+str(loan["id"])+"/return",{})
assert status==409
status,new=req("/api/loans",{"personId":person,"equipmentId":equipment})
assert status==200, (status,new)
status,snapshot=req("/api/snapshot")
assert status==200 and len(snapshot["loans"])==2
print("CHECKOUT_SMOKE_PASSED")
PY
