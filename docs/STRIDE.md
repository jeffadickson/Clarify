# Clarify STRIDE security threat model (draft)

**Important:** STRIDE here refers to security threat modeling (Spoofing, Tampering, Repudiation, Information disclosure, Denial of service, Elevation of privilege); it is separate from the numbered requirements-tracking process in `REQUIREMENTS.md`.

| Threat | Boundary | Control / verification |
|---|---|---|
| Spoofing | Studio remote access, generated login | Loopback by default, explicit auth for remote access, authentication integration tests |
| Tampering | Dictionary and generated artifacts | Stable model IDs, manifest hashes, reviewed source diffs, extension lockfiles |
| Repudiation | Record edits, generation and migration approvals | Audit records with actor, time, revision, trace ID |
| Information disclosure | Database fields, reports, logs | Server-side field authorization, secret redaction, report access tests |
| Denial of service | Import, expressions, plugins, API endpoints | Resource limits, bounded expressions, rate limits, cancellation tests |
| Elevation of privilege | Extensions, generated endpoints, cross-tenant access | Capability policy, out-of-process untrusted extensions, authorization tests |

Threat modeling must be updated as trust boundaries and implementation change.
