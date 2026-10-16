# GitHub Actions CI status diagnostics

## Root cause of misleading empty responses

The connected GitHub action `fetch_commit_workflow_runs` **only returns
pull-request-triggered workflow runs**, not push runs. Most Clarify commits
are pushed straight to `main`. Therefore an empty result is **not evidence
that Actions did not run**.

Likewise `get_commit_combined_status` reads the legacy commit-status API,
not necessarily GitHub Actions check runs; `statuses: []` does not prove
a workflow has not executed.

## Source-level corrections

- `.github/workflows/shared-blazor.yml` runs on **every push**, PR and manual
  dispatch. Previously, its path filter excluded `workflow/**`,
  `*.sln` and other root files; although docs and build props had already
  been added before this change, path filters were too easy to miss.
- `.github/workflows/ci-trigger-diagnostics.yml` is a tiny, independent
  Linux job, also triggered by push/PR/dispatch. It prints commit SHA, event,
  run ID, and validates checkout and required project files.
- Heavy web and Windows checks remain separate from the trigger diagnostic;
  the Windows desktop job is currently nonblocking and must not be interpreted
  as release validation.

## How to distinguish problems

1. Visit `https://github.com/jeffadickson/Clarify/actions`, open
   **CI Trigger Diagnostics** and verify a run for the latest main SHA.
2. If the diagnostic workflow is absent, inspect repository Settings →
   Actions → General for disabled Actions, branch restrictions, required
   approvals or org-level policy. This configuration is not exposed through
   the current GitHub connector.
3. If diagnostic is green but **Shared Blazor + DSL** is red, inspect the
   first failing **web** step. It is a build/test failure, not a trigger
   failure. Use `fetch_workflow_run_jobs` and `fetch_workflow_job_logs` with
   a known run ID to capture the failure.
4. If both are absent from Actions after an eligible push, check whether
   the workflows are recognized and enabled and inspect Actions policy.
5. For a manual test, use **Run workflow** on the diagnostic workflow and
   the full workflow. Do not conflate manual and push results.

## Verification status

The source-level trigger fix and independent probe have been committed.
At this writing we have **not observed the resulting Actions run status**,
because the available connector cannot list repository push runs and the
legacy status endpoint reports no statuses. Do not mark CI green until
the actual Actions run can be inspected.

## Next cleanup after run evidence

- Split lightweight syntax/generator checks and long integration/UI jobs,
  keeping one stable required check.
- Pin dependency resolutions and cache .NET/NuGet and browser dependencies.
- Upload TRX, browser and DocFX artifacts on failure, not only success.
- Add branch-protection required checks once their names and pass rate are stable.
