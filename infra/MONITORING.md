# Monitoring and Alerting

This document describes monitoring and alerting for Team Tasks AKS deployments.

## Kubernetes Workload Health

### View pod status
```bash
kubectl get pods -n <environment>
kubectl describe pod -n <environment> <pod-name>
```

### View container logs
```bash
# API logs
kubectl logs -n <environment> -l app=teamtasks-api --tail=100

# Client logs
kubectl logs -n <environment> -l app=teamtasks-client --tail=100

# Previous container (if pod restarted)
kubectl logs -n <environment> <pod-name> --previous
```

### View deployment status
```bash
kubectl rollout status deployment/teamtasks-api -n <environment>
kubectl rollout status deployment/teamtasks-client -n <environment>
```

## Azure Monitor (AKS Insights)

Enable Container Insights on the AKS cluster to get:
- Pod CPU and memory usage
- Node health metrics
- Container log collection in Log Analytics

### Enable Container Insights via Azure CLI
```bash
az aks enable-addons \
  --name <cluster-name> \
  --resource-group <resource-group> \
  --addons monitoring \
  --workspace-resource-id <log-analytics-workspace-id>
```

### Useful KQL queries in Log Analytics

**Pod restarts in the last hour**:
```kql
KubePodInventory
| where TimeGenerated > ago(1h)
| where Namespace in ("development", "stage", "production")
| summarize Restarts = sum(RestartCount) by PodName, ContainerName, Namespace
| where Restarts > 0
| order by Restarts desc
```

**API error rate**:
```kql
ContainerLog
| where TimeGenerated > ago(1h)
| where ContainerID contains "teamtasks-api"
| where LogEntry contains "ERROR" or LogEntry contains "CRITICAL"
| project TimeGenerated, ContainerName, LogEntry
| order by TimeGenerated desc
```

## Recommended Alerts

| Alert | Metric | Threshold | Severity |
|-------|--------|-----------|---------|
| Pod restart loop | `kube_pod_container_status_restarts_total` | > 3 in 5 min | High |
| API unavailable | HTTP probe on `/api/tasks` | 3 failures | Critical |
| Node CPU high | Node CPU % | > 85% for 5 min | Medium |
| Node memory high | Node memory % | > 90% for 5 min | High |

### Create an alert rule (Azure CLI)
```bash
az monitor alert create \
  --name "teamtasks-api-unavailable" \
  --resource-group <rg> \
  --condition "avg Percentage CPU > 85" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action-group <action-group-id>
```

## GitHub Actions Workflow Notifications

Configure GitHub notifications for workflow failures:
1. Go to repository **Settings → Notifications**
2. Enable email or Slack notifications for failed Actions runs
3. Use the `workflow_run` trigger in a notification workflow to send rich alerts

## Ingress / External Availability

Check external availability of the ingress endpoint:
```bash
# Get ingress external IP
kubectl get ingress -n <environment> teamtasks-ingress

# Test connectivity
curl -v http://<external-ip>/healthz
curl -v http://<external-ip>/api/tasks
```

For production, configure an Azure Load Balancer health probe or external uptime monitor on the ingress endpoint.
