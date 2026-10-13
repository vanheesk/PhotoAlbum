# Modernization Plan: PhotoAlbum Azure Cloud Modernization

**Project**: PhotoAlbum

---

## Technical Framework

- **Language**: C#
- **Framework**: ASP.NET Core 9.0 Razor Pages (.NET 9)
- **Build Tool**: .NET SDK / `dotnet`
- **Database**: SQL Server LocalDB
- **Key Dependencies**: Entity Framework Core 9, SixLabors.ImageSharp, xUnit

---

## Overview

> This modernization prepares PhotoAlbum for a production Azure deployment. The
> application currently stores photo files on the local filesystem and uses a
> local SQL Server connection. This repository contains one application; no
> second app or assessment report was present.
>
> The Azure architecture will:
>
> - Store uploaded photo content in Azure Blob Storage so it persists across
>   container restarts and replicas.
> - Connect application data to Azure SQL Database using managed identity.
> - Provision the Azure resources and deploy the existing containerized app to
>   Azure Container Apps.
>
> The plan preserves the application's current .NET framework version and
> addresses cloud readiness, infrastructure, security, and deployment without
> adding framework upgrade work.

---

## Migration Impact Summary

| App | Current | Azure target | Authentication | Change |
|-----|---------|--------------|----------------|--------|
| PhotoAlbum | Local disk | Blob Storage | System MI | Photo files |
| PhotoAlbum | LocalDB | Azure SQL | System MI | EF Core data |
| PhotoAlbum | Local host | Container Apps | System MI | Web deployment |

---

## Open Questions & Questionnaire

- [x] Q: Should the plan include environment/infrastructure provisioning? →
  A: Yes — provision new Azure infrastructure with the existing Bicep definition.
- [x] Q: Should the plan include integration testing? →
  A: Yes — Real mode, using the infrastructure provisioned by this plan.
- [x] Q: Should the plan include security and CVE remediation? →
  A: Yes — include the default dependency scan and remediation task.
- [x] Q: Which Azure deployment target should the plan use? →
  A: Azure Container Apps, matching the repository's existing deployment setup.
- [x] Q: Should containerization be a separate task? →
  A: No — Azure Container Apps deployment includes containerization; a Dockerfile
  already exists.
- [ ] Which Azure subscription ID should be used for provisioning? Not provided;
  select the intended active subscription before executing the plan.

---
