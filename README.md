# alertmanager-receiver-azdo

[![GitHub](https://img.shields.io/github/stars/ivanjosipovic/alertmanager-receiver-azdo?style=social)](https://github.com/IvanJosipovic/alertmanager-receiver-azdo)
[![Artifact Hub](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/alertmanager-receiver-azdo)](https://artifacthub.io/packages/helm/alertmanager-receiver-azdo/alertmanager-receiver-azdo)

## What is this?

This project is an API server which implements the [Alertmanager Webhook Receiver](https://prometheus.io/docs/operating/integrations/#alertmanager-webhook-receiver) API. This allows Alertmanager to Create and Resolve Azure DevOps work items.

## Features
- Create and resolve Azure DevOps work items
- Customizble Fields allow support for custom Azure DevOps Processes
- Authentication
  - Personal Access Token
  - Service Principle
  - Managed Identity
  - Workload Identity
- AMD64 and ARM64 support

## Installation
### Configure Helm Values

Download the default [Helm Values](https://raw.githubusercontent.com/IvanJosipovic/alertmanager-receiver-azdo/alpha/charts/alertmanager-receiver-azdo/values.yaml)

```bash
curl https://raw.githubusercontent.com/IvanJosipovic/alertmanager-receiver-azdo/alpha/charts/alertmanager-receiver-azdo/values.yaml --output values.yaml
```

Modify the settings to fit your needs
https://github.com/IvanJosipovic/alertmanager-receiver-azdo/blob/21bef42c06006a49fad64290b1af321028e41ed7/charts/alertmanager-receiver-azdo/values.yaml#L17C1-L59C22
###

### Install Helm Chart

```bash
helm repo add alertmanager-receiver-azdo https://ivanjosipovic.github.io/alertmanager-receiver-azdo

helm repo update

helm install alertmanager-receiver-azdo alertmanager-receiver-azdo/alertmanager-receiver-azdo --create-namespace --namespace alertmanager-receiver-azdo -f values.yaml
```
