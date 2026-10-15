# Clarify Sample — Equipment Checkout

This is a **standalone runnable ASP.NET Core/.NET 10 + SQLite application** built around a Clarify semantic dictionary. It demonstrates how generated database infrastructure can coexist with ordinary author-written C#.

## What you can do

- Browse the seeded inventory (oscilloscope, multimeter, thermal camera)
- Add equipment and borrowers
- Check equipment out to a borrower
- Return items; prevent checking out an already-loaned item
- See checkout history with borrower and equipment names

## Run

```sh
dotnet run --project examples/equipment-checkout/EquipmentCheckout.csproj
```

Open **http://127.0.0.1:5279**. On first run, the app creates `equipment-checkout.sqlite` in its working directory and seeds two borrowers and three devices. Subsequent runs preserve the data.

To inspect the source-driven generation:

```sh
node tools/generate-ef-models.mjs \
  examples/equipment-checkout/dictionary.json \
  examples/equipment-checkout/generated
node --test tests/equipment-checkout.test.mjs
bash scripts/smoke-equipment-checkout.sh
```

The smoke script uses an isolated temporary database and verifies checkout, duplicate-checkout rejection, return, and checkout again.

## Files and ownership

| File | Owner |
|---|---|
| `dictionary.json` | Clarify semantic data dictionary |
| `generated/CheckoutDbContext.g.cs` | Generated EF Core models, primary keys, foreign keys and indexes |
| `Program.cs` | User-owned C# application rules, validation, async CRUD endpoints and seeding |
| `wwwroot/index.html` | Sample responsive web presentation |
| `tests/equipment-checkout.test.mjs` | Reproducible generator parity test |

The source generator only writes into the `generated` directory. It does **not** overwrite `Program.cs` or other author-owned code.

## Architecture and limitations

This sample is a **functional hand-assembled frontend and business layer over a generated EF Core model**, not yet proof of automatic form/CRUD generation from the 40-template gallery. The full visual-to-app workflow remains a core Clarify milestone.

- **No authentication:** This is a localhost-only sample. Do not publish it on the internet.
- **Schema changes:** Uses `EnsureCreated` only for the initial demonstration database. Existing data requires reviewed migrations.
- **Concurrent checkout:** The duplicate-loan check is an application rule; a production database also needs a uniqueness/concurrency constraint to prevent races under load.
- **Validation:** Basic server-side field checks are implemented. Production auth/audit, email validation and full generated validators are still required.
- **No separate workflow engine:** Returning an item is an ordinary C# command, not an Elsa workflow.

A passing .NET build and HTTP smoke test still need to be observed. See `docs/PROJECT-STATUS-2026-10-15.md` for overall Clarify maturity.
