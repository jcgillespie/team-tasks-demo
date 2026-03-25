# First Provisioning and Promotion Checklist

Use this checklist when provisioning a new environment or running the first end-to-end promotion for Team Tasks.

## Pre-Provisioning

- [ ] Azure subscription is active and accessible (`az account show`)
- [ ] Required resource provider registrations confirmed:
  - `az provider register --namespace Microsoft.ContainerService`
  - `az provider register --namespace Microsoft.ContainerRegistry`
  - `az provider register --namespace Microsoft.KeyVault`
  - `az provider register --namespace Microsoft.Network`
- [ ] Terraform >= 1.6 installed and in PATH (`terraform version`)
- [ ] Docker available locally (`docker info`)
- [ ] `.NET 10 SDK` installed (`dotnet --version`)
- [ ] `pnpm` installed (`pnpm --version`)
- [ ] `kubectl` installed (`kubectl version --client`)

## Backend State Bootstrap (Once per subscription)

- [ ] Resource group `teamtasks-tfstate` created
- [ ] Storage account created for Terraform state
- [ ] Container `tfstate` created in storage account

## Environment Provisioning (Per Environment)

- [ ] `terraform.tfvars` created from example template
- [ ] `subscription_id` filled in with correct value
- [ ] `github_org` filled in correctly
- [ ] `terraform init` succeeds with correct backend config
- [ ] `terraform validate` passes with no errors
- [ ] `terraform plan` reviewed — no unexpected destroy operations
- [ ] `terraform apply` completed successfully
- [ ] Terraform outputs captured and saved to GitHub Actions environment variables

## GitHub Setup

- [ ] GitHub environment `development` created in repo settings
- [ ] GitHub environment `stage` created in repo settings
- [ ] GitHub environment `production` created in repo settings
- [ ] Production environment has required reviewers set
- [ ] OIDC federation configured (see `infra/OIDC_SETUP.md`)
- [ ] GitHub secrets added per environment:
  - `AZURE_CLIENT_ID`
  - `AZURE_TENANT_ID`
  - `AZURE_SUBSCRIPTION_ID`
  - `TF_BACKEND_STORAGE_ACCOUNT`
  - `TF_BACKEND_RESOURCE_GROUP`
- [ ] GitHub environment variables added per environment:
  - `ACR_NAME`
  - `ACR_LOGIN_SERVER`
  - `AKS_CLUSTER_NAME`
  - `AKS_RESOURCE_GROUP`

## First Deployment

- [ ] Validate workflow (`validate.yml`) triggered on PR and passes
- [ ] Build workflow (`build.yml`) triggered on merge to main and passes
- [ ] Images visible in ACR (`az acr repository list --name <acr>`)
- [ ] Kubernetes namespaces applied:
  - `kubectl apply -f infra/kubernetes/namespaces/development.yaml`
- [ ] Initial deployments applied in development:
  - `kubectl apply -f infra/kubernetes/api-deployment.yaml -n development`
  - `kubectl apply -f infra/kubernetes/api-service.yaml -n development`
  - `kubectl apply -f infra/kubernetes/client-deployment.yaml -n development`
  - `kubectl apply -f infra/kubernetes/client-service.yaml -n development`
  - `kubectl apply -f infra/kubernetes/ingress.yaml -n development`
- [ ] Deploy-to-development workflow completes successfully
- [ ] API health check passes in development
- [ ] Client accessible via ingress in development

## First Promotion

- [ ] Quality gate evaluation passes for dev-to-stage
- [ ] Promote-to-stage workflow completes successfully
- [ ] Services validated in stage

## Production Promotion

- [ ] Quality gate evaluation passes for stage-to-production
- [ ] Promotion PR reviewed and merged (if using PR flow)
- [ ] Production approval request reviewed and approved in GitHub
- [ ] Promote-to-production workflow completes successfully
- [ ] Services validated in production
