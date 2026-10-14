# Authentication providers and per-application permissions

## The distinction
**Authentication** answers who has signed in. **Clarify authorization** decides what that person can do in this particular application. Elsa workflows use the same host principal; they do not maintain a separate password database.

## Provider choice
Set `Clarify:Authentication:Provider` to:
- `Identity` (default): ASP.NET Core Identity, with account/role data stored in the host's Identity EF Core database. No custom password hashing or reset implementation.
- `Oidc`: an existing standards-compliant OpenID Connect provider (potentially Frappe only when its deployment supports an appropriate, validated OIDC integration). Configure HTTPS authority, client ID, and client secret from user secrets or environment variables, never from committed config.

Example development configuration using environment variables:
```sh
Clarify__Authentication__Provider=Oidc
Clarify__Authentication__Authority=https://identity.example.invalid
Clarify__Authentication__ClientId=clarify-web
Clarify__Authentication__ClientSecret=SET_OUTSIDE_SOURCE_CONTROL
```

This is an example; the domain is intentionally nonfunctional. Production OIDC requires validated redirect URLs, a trusted issuer, appropriate scopes and provider registration. Never trust an unsigned header containing a user ID.

## Local development
`Clarify:Authentication:EnableLocalRegistration` defaults to false. The demonstration Identity API group is only mapped when Development + `Identity` + explicit registration opt-in. A real deployment needs a documented administrator onboarding path and email/password-reset services, or external SSO.

## Permission model
- Per app: membership + role + explicit action grant (all deny by default).
- A person can have different roles in `sales` and `inventory`.
- Record/tenant authorization is additional, never implied by a UI button.
- No global role grants automatic access to another application's records.
- Stable OIDC keys include issuer + subject to avoid user-ID collisions across providers. Email is never the primary authorization key.
- Workflow human tasks must use these stable IDs for candidate/assignee lookup and independently enforce the app-specific task permission.

## Unfinished acceptance
- Verify real .NET 10 compilation, OIDC sign-in callback/cookie lifecycle, account provisioning and policy enforcement.
- Remove anonymous permissions from generated Customer/Order screens.
- Add application role-management UI, audit logging, tenant isolation and migrations.
- Test disabled users, reassignment, cross-app access and Elsa task decisions.
- Review Frappe configuration before treating it as an OIDC provider; OAuth 2 support by itself is not proof of standards-compliant OIDC issuer behavior.
