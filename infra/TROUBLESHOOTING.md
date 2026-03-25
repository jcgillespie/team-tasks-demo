# Deployment Troubleshooting Guide

Common failure scenarios and remediation steps for Team Tasks deployment operations.

---

## Terraform Failures

### `Error: building AzureRM Client`

**Symptom**: Terraform init or plan fails with authentication error.  
**Cause**: Azure CLI session expired or wrong subscription selected.  
**Fix**:
```bash
az login
az account set --subscription <your-subscription-id>
terraform plan
```

### `Error: A resource with the ID ... already exists`

**Symptom**: Terraform apply fails claiming a resource already exists outside state.  
**Cause**: Resource was created manually or state file is stale.  
**Fix**:
```bash
# Import the resource into state, e.g. for a resource group:
terraform import azurerm_resource_group.main /subscriptions/<sub>/resourceGroups/<name>
```

### `Error: Backend configuration changed`

**Symptom**: `terraform init` fails after backend config change.  
**Fix**:
```bash
terraform init -reconfigure
```

### Terraform plan shows unexpected destroy/recreate

**Symptom**: Plan proposes destroying and recreating a critical resource.  
**Cause**: A computed attribute changed (e.g., node pool VM size).  
**Fix**: Review the plan carefully. If unintended, use `lifecycle { prevent_destroy = true }` or restore the original attribute value.

---

## Docker Build Failures

### `COPY failed: file not found`

**Symptom**: Docker build exits with a missing file error.  
**Cause**: Build context does not include the expected file.  
**Fix**: Ensure you run `docker build` from the correct directory (see Dockerfile's expected context in `server/Dockerfile` and `client/Dockerfile`).

### `npm install` / `pnpm install` fails in container

**Symptom**: Frontend Dockerfile fails during dependency installation.  
**Fix**: Confirm `pnpm-lock.yaml` is present and committed. Run `pnpm install` locally first to regenerate if needed.

---

## GitHub Actions Failures

### `Error: OIDC token request failed`

**Symptom**: GitHub Actions workflow fails at the `azure/login` step.  
**Cause**: OIDC federation not configured or subject claim mismatch.  
**Fix**: See [`infra/OIDC_SETUP.md`](OIDC_SETUP.md). Verify the federated credential subject matches the workflow ref.

### `Error: unauthorized: authentication required` (ACR push)

**Symptom**: `docker push` step fails.  
**Cause**: The GitHub Actions managed identity lacks `AcrPush` role on the ACR.  
**Fix**:
```bash
az role assignment create \
  --assignee <managed-identity-object-id> \
  --role AcrPush \
  --scope /subscriptions/<sub>/resourceGroups/<rg>/providers/Microsoft.ContainerRegistry/registries/<acr>
```

### Deployment fails but previous run succeeded

**Symptom**: `kubectl apply` step fails intermittently.  
**Cause**: AKS API server transient issue or credential expiry.  
**Fix**: Re-run the workflow. If persistent, check AKS cluster health:
```bash
az aks show --name <cluster> --resource-group <rg> --query provisioningState
```

### Validation workflow fails on PR

**Symptom**: `validate.yml` reports lint or test failure.  
**Cause**: Code changes break existing tests or linting rules.  
**Fix**: Run tests locally before pushing:
```bash
dotnet test TeamTasks.slnx
cd client && pnpm test && pnpm lint
```

---

## Kubernetes / AKS Failures

### Pods in `CrashLoopBackOff`

**Symptom**: `kubectl get pods -n <env>` shows pods crashing.  
**Fix**:
```bash
kubectl logs -n <env> <pod-name> --previous
kubectl describe pod -n <env> <pod-name>
```
Common causes: missing environment variable, Key Vault secret not accessible, incorrect image reference.

### `ImagePullBackOff`

**Symptom**: Pod cannot pull the container image.  
**Cause**: ACR pull permission missing or image tag does not exist.  
**Fix**:
```bash
# Verify image exists in ACR
az acr repository show-tags --name <acr> --repository teamtasks/api

# Check AKS has ACR pull permission
az aks check-acr --name <cluster> --resource-group <rg> --acr <acr>.azurecr.io
```

### Ingress returns 502 / 503

**Symptom**: Ingress is created but returns error for requests.  
**Cause**: Backend service not ready or health probe failing.  
**Fix**:
```bash
kubectl get endpoints -n <env>
kubectl describe ingress -n <env>
```
Ensure the service selector matches the pod labels in the deployment manifest.

### Persistent volume not mounting (API pod)

**Symptom**: API pod fails to start; logs show SQLite file access error.  
**Cause**: PersistentVolumeClaim not bound.  
**Fix**:
```bash
kubectl get pvc -n <env>
kubectl describe pvc -n <env> teamtasks-api-data
```
Check the storage class is available: `kubectl get storageclass`.

---

## Promotion Failures

### Promotion blocked by quality gate

**Symptom**: Promotion workflow exits with gate failure message.  
**Fix**: Review the gate failure output in the workflow run. Fix the identified issues before re-running promotion.

### Stage promotion blocked waiting for approval

**Symptom**: Production promotion workflow is waiting.  
**Cause**: Required reviewers haven't approved the GitHub environment.  
**Fix**: Navigate to the workflow run in GitHub → review the pending environment approval request.

### Same artifact cannot be found for promotion

**Symptom**: Promotion workflow cannot locate the image tag.  
**Cause**: ACR retention policy deleted the image, or artifact ID is incorrect.  
**Fix**:
```bash
az acr repository show-tags --name <acr> --repository teamtasks/api --orderby time_desc
```

---

## Recovery Procedures

### Rollback a deployment

```bash
bash scripts/deploy/rollback.sh <environment> <artifact-id>
```

### Re-run a failed Terraform apply (interrupted)

```bash
cd infra/terraform/environments/<env>
terraform apply  # Terraform resumes from where it stopped
```

### Re-run a GitHub Actions workflow

Use the GitHub UI "Re-run jobs" button, or use GitHub CLI:
```bash
gh run rerun <run-id>
```
