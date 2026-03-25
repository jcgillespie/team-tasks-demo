---
status: awaiting_human_verify
trigger: "Investigate issue: tf-federated-identity-schema-mismatch\n\nSummary: The Terraform/OpenTofu file infra/modules/identity/oidc.tf shows editor/schema errors that conflict with successful tofu validation. Need to find root cause and fix if possible."
created: 2026-03-25T00:00:00Z
updated: 2026-03-25T14:34:00Z
---

## Current Focus

hypothesis: The applied workspace root-module hints should let the Terraform language server re-index shared module files through the initialized env roots after a VS Code reload.
test: Reload the VS Code window and re-open infra/modules/identity/oidc.tf, then re-check diagnostics.
expecting: The editor should stop requiring `parent_id` and accept `user_assigned_identity_id` for azurerm_federated_identity_credential.
next_action: User reloads VS Code and confirms whether oidc.tf diagnostics are cleared.

## Symptoms

expected: The federated identity credential resource in infra/modules/identity/oidc.tf should use the current AzureRM-compatible arguments and have no editor/runtime errors.
actual: VS Code/terraform language diagnostics report: Required attribute "parent_id" not specified and Unexpected attribute "user_assigned_identity_id" not expected here. However, tofu validate succeeded for both infra/envs/dev and infra/envs/prod after updating the file.
errors: Required attribute "parent_id" not specified: An attribute named "parent_id" is required here. Unexpected attribute: An attribute named "user_assigned_identity_id" is not expected here.
reproduction: Open infra/modules/identity/oidc.tf in VS Code after changing azurerm_federated_identity_credential from resource_group_name/parent_id to user_assigned_identity_id. The editor reports schema errors. Running tofu init -backend=false -upgrade and tofu validate in infra/envs/dev and infra/envs/prod succeeds.
started: Started after updating the file to current AzureRM terminology. Runtime/provider docs indicate user_assigned_identity_id is current. The issue appears limited to editor/schema diagnostics.

## Eliminated

## Evidence

- timestamp: 2026-03-25T14:07:00Z
	checked: infra/modules/identity/oidc.tf and get_errors for that file
	found: VS Code diagnostics still report "parent_id" required and "user_assigned_identity_id" unexpected on azurerm_federated_identity_credential.
	implication: The mismatch is reproducible in-editor and is not a stale user report.

- timestamp: 2026-03-25T14:08:00Z
	checked: infra/envs/dev/backend.tf, infra/envs/prod/backend.tf, infra/envs/dev/.terraform.lock.hcl, infra/envs/prod/.terraform.lock.hcl
	found: Both roots pin hashicorp/azurerm ~> 4.65 and both lock files resolve registry.opentofu.org/hashicorp/azurerm at 4.65.0.
	implication: OpenTofu validation is using a current provider version that should support the newer federated identity credential arguments.

- timestamp: 2026-03-25T14:09:00Z
	checked: infra/modules/identity/main.tf, variables.tf, outputs.tf
	found: The identity module has no local terraform/required_providers block; provider versioning is defined only in the root environments.
	implication: Editor diagnostics for files under the module may depend on external provider context rather than a module-local lock/config.

- timestamp: 2026-03-25T14:14:00Z
	checked: local tooling and workspace directories
	found: OpenTofu 1.11.5 is installed, both env roots have initialized .terraform directories, and the VS Code extensions directory contains hashicorp.terraform-2.39.0 but no OpenTofu-specific extension.
	implication: Editor diagnostics are likely coming from the Terraform extension stack rather than from OpenTofu-aware tooling.

- timestamp: 2026-03-25T14:22:00Z
	checked: /Users/joshgillespie/Library/Application Support/Code/User/settings.json and hashicorp.terraform extension package/readme files
	found: User settings contain no Terraform/OpenTofu overrides; the installed extension is HashiCorp Terraform 2.39.0 with terraform-ls 0.38.5, and its readme documents workspace `.vscode/settings.json` support for `terraform.languageServer.rootModules` when a workspace contains multiple root modules.
	implication: There is a plausible repo-local fix by teaching the language server which directories are the real Terraform/OpenTofu roots for this workspace.

- timestamp: 2026-03-25T14:24:00Z
	checked: local provider binaries
	found: The only azurerm provider binaries found on disk are the repo-local OpenTofu installs at infra/envs/dev/.terraform and infra/envs/prod/.terraform, both version 4.65.0 under registry.opentofu.org/hashicorp/azurerm.
	implication: The conflicting schema is unlikely to be coming from another local azurerm plugin binary and is more likely an editor cache/context issue.

- timestamp: 2026-03-25T14:31:00Z
	checked: /Users/joshgillespie/src/team-tasks-demo/gsd/.vscode/settings.json and immediate post-change diagnostics
	found: Added `terraform.languageServer.rootModules` for `/infra/envs/dev` and `/infra/envs/prod`; oidc.tf diagnostics did not change immediately, while the settings file itself has no errors.
	implication: The workspace fix is syntactically valid, but the running language-server session has not yet reloaded its configuration or re-indexed the module.

- timestamp: 2026-03-25T14:32:00Z
	checked: hashicorp.terraform extension client code
	found: The extension reacts to Terraform configuration changes by prompting for a VS Code window reload before applying them.
	implication: Immediate in-process clearing of the stale diagnostics is not expected; human reload is required to verify the fix.

## Resolution

root_cause: The HashiCorp Terraform language server was diagnosing a shared module file without explicit workspace root-module context, so it kept using stale/incompatible schema information for `azurerm_federated_identity_credential` even though OpenTofu validation from the actual env roots succeeded with AzureRM 4.65.
fix: Added workspace Terraform language server root-module hints in `.vscode/settings.json` for `infra/envs/dev` and `infra/envs/prod` so the editor can resolve shared modules through the initialized OpenTofu roots.
verification: Self-checks complete: runtime validation still passes separately, the workspace settings file is valid JSON, and the remaining step is a VS Code reload plus re-check of editor diagnostics.
files_changed: [.vscode/settings.json]
