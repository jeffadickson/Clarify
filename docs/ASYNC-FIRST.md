# Clarify async-first generation

## Implemented source increment
- `ICustomerStore.PageAsync`: cancellation-aware bounded paging.
- `SqliteCustomerStore.PageAsync`: EF Core async query, stable Name/Id ordering, page-size bound 1–200 and a one-extra-row HasNext check.
- `CustomerPage.razor`: Next/Previous, loading state, cancellation of superseded browse requests, and disposal cancellation.
- `tools/generate-async-contracts.mjs`: produces cancellable browse contracts for every entity in the example dictionary.
- `tests/async-generator.test.mjs`: checks deterministic generated source.
- `tests/Clarify.M0.Tests/AsyncBrowseTests.cs`: file-backed SQLite paging and cancellation test.

## Intended default
Use async for database, network, files, and slow I/O; synchronous C# remains appropriate for small CPU-only calculations. Propagate CancellationToken, bound paging and concurrency, and use separate EF Core DbContext instances for concurrent operations. Never invoke .Result/.Wait() on async I/O. Handle cancellation as cancellation, not a business error.

## Performance measurement
Async primarily improves thread utilization and throughput under I/O-bound concurrent load, not single-query execution latency. Benchmark p50/p95/p99 latency, throughput, CPU, allocation rate, connection-pool waits and database saturation with realistic datasets. Use Clarify's OpenTelemetry instrumentation to correlate requests and EF Core activity. Background jobs require a durable queue and do not follow automatically from async/await.

## Limitations
The generated C# interfaces are a **scaffold example**, not wired into every generated application. Real generic EF Core store generation, async report renderer, cancellation of save operations, production-safe page cursors, retry/idempotency policies, browser tests and performance benchmarks remain outstanding. Tests are committed but no passing .NET CI execution has been observed.
