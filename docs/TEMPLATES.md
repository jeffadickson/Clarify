# Clarify template packages — architectural baseline

## Purpose
Templates are **versioned declarative generator recipes**. Developers bind templates to dictionary entities, inspect generated ClarifyDSL in the graphical/form editor, customize generated layouts, and add ordinary C# in separate user-owned files.

## Initial implementation
- `templates/master-detail/template.json` is a minimal manifest, format `clarify-template/v1`, with semantic ID, version, typed entity parameters, relationship requirement, and form recipes.
- `designer/templates.mjs` is a deterministic parameter-binding/validation engine. It does **not execute package-supplied C# or JavaScript**.
- `/studio/templates.html` is a functioning preview that chooses parent and child entities, validates their foreign-key relation, previews form DSL and downloads files.
- `designer/templates.test.mjs` covers valid, missing and unrelated entity bindings and reproducibility.

## Design rules
1. Templates may create dictionary entities/relationships, forms, reports, workflows, permission declarations, optional C# scaffolds and test scaffolds in future versions.
2. Every generated element must eventually have a stable ID, an originating package ID/version, bound parameter values, and an author-overrides layer.
3. Regeneration must not silently overwrite author-owned `.cs`, `.razor`, `.clarify` or designer customizations. Require explicit 3-way preview/merge or a generated-owned region contract.
4. Third-party packages must be inspectable and versioned. Declarative data templates may be previewed without executing untrusted code; .NET/NuGet extensions are separately trusted and approved.
5. General source APIs and dependency injection remain open: C# is first-class and no template monopolizes an extension point.
6. A package compatibility check must cover Clarify version, schema version, dependencies and generator contract.
7. Template updates should be migration/diff operations, with a reversible plan and no implicit production schema edits.
8. Tooling must work offline with local packages; remote gallery installation is optional.

## First release limitation
Current master-detail generation emits two independent form layouts; it **does not yet synthesize the nested child grid**, automatically persist the template instance in the source project, apply customizations into the visual editor, or regenerate EF Core/Blazor operations. Those are the next milestone. This is intentionally documented as an incremental proof of the package format, **not full Clarion template parity**.

## Example
Open `/studio/templates.html`, choose Parent=`Order` and Child=`OrderLine`; click **Generate preview**. Download the resulting DSL. The existing form designer must be extended to import this DSL and show editable layout controls.

## 2026-10-14 — Visual designer handoff
The template library now offers **Edit visually** for each generated form. Studio opens `/studio/designer.html?template=master-detail&parent=Order&child=OrderLine&form=Order`, instantiates the same declarative package and displays the result in the existing editable canvas and DSL source pane.

Generated DSL carries a `// @clarify-template` comment with template ID, version, parameter bindings and form entity. The parser and layout patcher preserve this origin comment when moving controls.

The designer offers **Download DSL** for any valid form. Template previews explicitly disable the development-only Customer project-source Save button: they must not overwrite unrelated application C# or form source. This is a preview/export workflow, not a completed project integration or three-way regeneration merge.

`designer/templates.test.mjs` adds a test for parsing a template-generated form and preserving metadata after an editable layout patch.

**Still outstanding:** actual child-grid widget; model-managed stable node IDs; importing exported DSL into arbitrary project form files; persistent template instances, safe upgrade/diff, C# extension generation and verified Playwright tests.
