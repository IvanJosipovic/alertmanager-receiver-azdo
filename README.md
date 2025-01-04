# alertmanager-receiver-azdo

[![GitHub](https://img.shields.io/github/stars/ivanjosipovic/alertmanager-receiver-azdo?style=social)](https://github.com/IvanJosipovic/alertmanager-receiver-azdo)
[![Artifact Hub](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/alertmanager-receiver-azdo)](https://artifacthub.io/packages/helm/alertmanager-receiver-azdo/alertmanager-receiver-azdo)
![Downloads](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2Fipitio%2Fbackage%2Frefs%2Fheads%2Findex%2FIvanJosipovic%2Falertmanager-receiver-azdo%2Falertmanager-receiver-azdo%25252Falertmanager-receiver-azdo.json&query=%24.downloads&label=downloads)

## What is this?

This project is an API server which implements the [Alertmanager Webhook Receiver](https://prometheus.io/docs/operating/integrations/#alertmanager-webhook-receiver) API. This allows Alertmanager to Create and Resolve Azure DevOps work items.

## Features
- Create and resolve Azure DevOps work items
- Customizable Fields allow support for custom Azure DevOps Processes
- Authentication
  - Personal Access Token
  - Service Principle
  - Workload Identity
- AMD64 and ARM64 support

## Installation
### Configure Helm Values

Download the default [Helm Values](https://raw.githubusercontent.com/IvanJosipovic/alertmanager-receiver-azdo/alpha/charts/alertmanager-receiver-azdo/values.yaml)

```bash
curl https://raw.githubusercontent.com/IvanJosipovic/alertmanager-receiver-azdo/alpha/charts/alertmanager-receiver-azdo/values.yaml --output values.yaml
```

Modify the settings to fit your needs

### Install Helm Chart

```bash
helm repo add alertmanager-receiver-azdo https://ivanjosipovic.github.io/alertmanager-receiver-azdo

helm repo update

helm install alertmanager-receiver-azdo alertmanager-receiver-azdo/alertmanager-receiver-azdo --create-namespace --namespace alertmanager-receiver-azdo -f values.yaml
```

### Create Alertmanager Config

```yaml
apiVersion: monitoring.coreos.com/v1alpha1
kind: AlertmanagerConfig
metadata:
  name: azdo
  namespace: monitoring
  labels:
    alertmanagerConfig: azdo
spec:
  route:
    groupBy: ['namespace']
    groupWait: 30s
    groupInterval: 5m
    repeatInterval: 12h
    receiver: 'webhook'
  receivers:
  - name: 'webhook'
    webhookConfigs:
    - url: 'http://alertmanager-receiver-azdo.alertmanager-receiver-azdo.svc.cluster.local:8080/alert'
```
