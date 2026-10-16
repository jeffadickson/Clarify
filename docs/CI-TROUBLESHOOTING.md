# GitHub Actions CI triage

## Why earlier status checks showed nothing
The available GitHub connector's `fetch_commit_workflow_runs` action
currently filters to **pull_request** runs and returns only one page. Clarify
mostly commits directly to `main`, which triggers **push** workflows.
Therefore an empty response does NOT mean GitHub Actions failed to run.
Legacy combined commit statuses may also be empty when checks are recorded
through GitHub's Checks API rather than commit Statuses API.

**Source of truth:** [Clarify Actions](https://github.com/jeffadickson/Clarify/actions)
and the selected workflow run's job summary/logs.

## Fast diagnostics workflow
`.github/workflows/ci-diagnostics.yml` has three independent jobs, triggered
on every push, pull request or manual dispatch:

1. **Runner and repository visibility:** confirms a hosted runner, checkout,
   source files, SHA/ref, and Node environment.
2. **Node tests and generated source:** runs foundational DSL/dictionary and
   example generator tests without .NET dependencies.
3. **.NET restore and parser build:** isolates SDK installation, NuGet package
   restore, and Parlot compilation.

The main `shared-blazor.yml` workflow now also supports **Run workflow**
(manual `workflow_dispatch`) and tracks documentation/build configuration
changes. It retains the full CI acceptance pipeline.

## When workflows do not appear
- Open **Actions** in the public repository. Look for
  **CI Diagnostics (Fast)** and **Shared Blazor + DSL**.
- If workflows aren't listed, check **Settings → Actions → General** for
  enabled actions and repository/org policies; workflow files must exist on
  the default branch. A disabled workflow must be re-enabled by the owner.
- If a run is listed as queued, inspect runner availability and repository
  Actions usage limits.
- If a run is skipped, inspect branch, path filters, event type and skip-CI
  commit markers. The fast diagnostics workflow intentionally has no filters.
- If a run fails before jobs, inspect the workflow YAML validation error.
- If the fast .NET job fails during restore, inspect exact NuGet source,
  package version, credentials, SDK and lock-file messages.
- If native parser build fails, inspect the first C# compiler diagnostic
  rather than inferring an Actions outage.

## Known source issue fixed
`src/Clarify.Dsl/ClarifyFormParser.cs` had doubled regex backslashes inside
a **verbatim C# regex string** for `grid columns` syntax. That matched
literal backslashes rather than whitespace/digits, rejecting valid grid lines.
Corrected to ordinary regex tokens in the C# verbatim string. This is a
source-level fix; an observed green run is still required.

## Evidence required to claim green CI
Capture a full run URL, workflow name, branch, commit SHA, attempt, start/end
times, and each required job's conclusion. For a failing job, retain the
first concrete failed step and relevant compiler/test log. Successful Node
tests do not imply .NET, Playwright, SQLite or DocFX passed.

## Manual quick checks
```sh
node --test designer/schema.test.mjs designer/clarifydsl.test.mjs designer/dependencies.test.mjs
node --test tests/ef-generation.test.mjs tests/equipment-checkout.test.mjs
dotnet --info
dotnet restore tests/Clarify.Dsl.Tests/Clarify.Dsl.Tests.csproj
dotnet build tests/Clarify.Dsl.Tests/Clarify.Dsl.Tests.csproj -c Release
```
