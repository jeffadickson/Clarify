#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT
export CLARIFY_SQLITE_TEST_DIR="$tmp"
python3 - <<'PY'
import os, sqlite3
from pathlib import Path
root=Path(os.environ['CLARIFY_SQLITE_TEST_DIR'])
db=sqlite3.connect(root/'sales.db')
db.executescript('''
CREATE TABLE "Customer" ("Id" INTEGER PRIMARY KEY, "Name" TEXT NOT NULL, "Email" TEXT);
CREATE TABLE "Product" ("Id" INTEGER PRIMARY KEY, "Name" TEXT NOT NULL, "PriceCents" INTEGER NOT NULL);
CREATE TABLE "Order" ("Id" INTEGER PRIMARY KEY, "CustomerId" INTEGER NOT NULL, "Date" TEXT NOT NULL);
CREATE TABLE "OrderLine" ("Id" INTEGER PRIMARY KEY, "OrderId" INTEGER NOT NULL,
 "ProductId" INTEGER NOT NULL, "Quantity" INTEGER NOT NULL, "UnitPriceCents" INTEGER NOT NULL);
INSERT INTO "Product" ("Id","Name","PriceCents") VALUES (1,'Preserved',1234);
''')
db.commit()
db.close()
PY
plan="examples/relational-generation/product-notes.migration.json"
id="$(python3 -c 'import json,sys;print(json.load(open(sys.argv[1]))["planId"])' "$plan")"
# No approval: migration must fail without touching the database.
if dotnet run --project tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -c Release -- \
  --plan "$plan" --db "$tmp/sales.db" --approve WRONG --backup "$tmp/rejected.db"; then
  echo "ERROR: migration accepted an incorrect approval" >&2; exit 1
fi
test ! -e "$tmp/rejected.db"
python3 - "$plan" "$tmp/tampered.json" <<'PY'
import json,sys
with open(sys.argv[1]) as file: data=json.load(file)
data['operations'][0]['column']='Tampered'
with open(sys.argv[2],'w') as file: json.dump(data,file)
PY
if dotnet run --project tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -c Release -- \
  --plan "$tmp/tampered.json" --db "$tmp/sales.db" --approve "$id" --backup "$tmp/tampered-backup.db"; then
  echo "ERROR: modified migration plan was accepted" >&2; exit 1
fi
test ! -e "$tmp/tampered-backup.db"
dotnet run --project tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -c Release -- \
  --plan "$plan" --db "$tmp/sales.db" --approve "$id" --backup "$tmp/backup.db"
python3 - <<'PY'
import os,sqlite3
from pathlib import Path
root=Path(os.environ['CLARIFY_SQLITE_TEST_DIR'])
db=sqlite3.connect(root/'sales.db')
backup=sqlite3.connect(root/'backup.db')
assert 'Notes' in [r[1] for r in db.execute('PRAGMA table_info("Product")')]
assert 'Notes' not in [r[1] for r in backup.execute('PRAGMA table_info("Product")')]
assert db.execute('SELECT Name,PriceCents FROM Product WHERE Id=1').fetchone()==('Preserved',1234)
assert db.execute('SELECT COUNT(*) FROM "__ClarifyMigrationHistory"').fetchone()[0]==1
db.close()
backup.close()
PY
# Reapplication is blocked by schema preflight/history, and backup is not overwritten.
if dotnet run --project tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -c Release -- \
  --plan "$plan" --db "$tmp/sales.db" --approve "$id" --backup "$tmp/second.db"; then
  echo "ERROR: repeated migration was allowed" >&2; exit 1
fi
test ! -e "$tmp/second.db"
echo "Clarify SQLite migration smoke passed: approval, backup, preservation, schema change and replay protection."
