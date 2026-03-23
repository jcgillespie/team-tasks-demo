# OpenTofu Infrastructure

This directory contains the Azure infrastructure definition for Team Tasks.

## Layout

- `modules/`: reusable building blocks used by all environments
- `environments/dev`: dev environment root module wiring
- `environments/staging`: staging environment root module wiring
- `environments/prod`: prod environment root module wiring

## Validation Commands

Run from an environment folder such as `infra/opentofu/environments/dev`.

```bash
tofu fmt -check
tofu init -backend-config=backend.hcl
tofu validate
tofu plan -var-file=dev.tfvars -out=plan.tfplan
```

## Apply Safeguard

Only apply from a previously saved plan:

```bash
tofu apply plan.tfplan
```
