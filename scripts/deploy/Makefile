SHELL := /usr/bin/env bash
.DEFAULT_GOAL := help

ENVIRONMENT ?= development
ARTIFACT_ID ?=

## help: Show available targets
.PHONY: help
help:
	@echo "Team Tasks Deployment Operations"
	@echo ""
	@echo "Usage: make <target> [ENVIRONMENT=<env>] [ARTIFACT_ID=<id>]"
	@echo ""
	@grep -E '^## ' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ": "}; {printf "  %-30s %s\n", $$2, $$3}'

## validate-app: Run all application tests (backend + frontend)
.PHONY: validate-app
validate-app:
	@echo ">>> Running backend tests..."
	dotnet test $(shell git -C ../.. rev-parse --show-toplevel)/TeamTasks.slnx
	@echo ">>> Running frontend lint and tests..."
	cd $(shell git -C ../.. rev-parse --show-toplevel)/client && pnpm lint && pnpm test

## tf-init: Initialise Terraform for the given environment
.PHONY: tf-init
tf-init:
	@echo ">>> Initialising Terraform for $(ENVIRONMENT)..."
	cd $(shell git -C ../.. rev-parse --show-toplevel)/infra/terraform/environments/$(ENVIRONMENT) && \
	terraform init \
	  -backend-config="resource_group_name=$${TF_BACKEND_RESOURCE_GROUP}" \
	  -backend-config="storage_account_name=$${TF_BACKEND_STORAGE_ACCOUNT}" \
	  -backend-config="container_name=tfstate" \
	  -backend-config="key=$(ENVIRONMENT).tfstate"

## tf-validate: Validate Terraform configuration
.PHONY: tf-validate
tf-validate:
	@echo ">>> Validating Terraform for $(ENVIRONMENT)..."
	cd $(shell git -C ../.. rev-parse --show-toplevel)/infra/terraform/environments/$(ENVIRONMENT) && \
	terraform validate

## tf-plan: Run Terraform plan for the given environment
.PHONY: tf-plan
tf-plan:
	@echo ">>> Planning Terraform for $(ENVIRONMENT)..."
	cd $(shell git -C ../.. rev-parse --show-toplevel)/infra/terraform/environments/$(ENVIRONMENT) && \
	terraform plan -out=tfplan

## tf-apply: Apply Terraform plan for the given environment
.PHONY: tf-apply
tf-apply:
	@echo ">>> Applying Terraform for $(ENVIRONMENT)..."
	cd $(shell git -C ../.. rev-parse --show-toplevel)/infra/terraform/environments/$(ENVIRONMENT) && \
	terraform apply tfplan

## tf-outputs: Show Terraform outputs for the given environment
.PHONY: tf-outputs
tf-outputs:
	@echo ">>> Terraform outputs for $(ENVIRONMENT):"
	cd $(shell git -C ../.. rev-parse --show-toplevel)/infra/terraform/environments/$(ENVIRONMENT) && \
	terraform output

## status: Show deployment status for the given environment
.PHONY: status
status:
	@bash $(shell git -C ../.. rev-parse --show-toplevel)/scripts/deploy/environment-status.sh $(ENVIRONMENT)

## validate-deploy: Validate the deployed workloads in the given environment
.PHONY: validate-deploy
validate-deploy:
	@bash $(shell git -C ../.. rev-parse --show-toplevel)/scripts/deploy/validate-deployment.sh $(ENVIRONMENT) "$(ARTIFACT_ID)"

## rollback: Roll back the given environment to a previous artifact
.PHONY: rollback
rollback:
	@if [ -z "$(ARTIFACT_ID)" ]; then echo "ARTIFACT_ID is required. Usage: make rollback ENVIRONMENT=<env> ARTIFACT_ID=<id>"; exit 1; fi
	@bash $(shell git -C ../.. rev-parse --show-toplevel)/scripts/deploy/rollback.sh $(ENVIRONMENT) $(ARTIFACT_ID)

## build-api: Build API Docker image locally
.PHONY: build-api
build-api:
	@ROOT=$(shell git -C ../.. rev-parse --show-toplevel) && \
	docker build -f $${ROOT}/server/Dockerfile -t teamtasks/api:local $${ROOT}

## build-client: Build client Docker image locally
.PHONY: build-client
build-client:
	@ROOT=$(shell git -C ../.. rev-parse --show-toplevel) && \
	docker build -f $${ROOT}/client/Dockerfile -t teamtasks/client:local $${ROOT}/client
