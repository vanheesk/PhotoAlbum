# Modernization Plan: Cloud Modernization

**Project**: PhotoAlbum

---

## Technical Framework

- **Language**: C#
- **Framework**: ASP.NET Core 10.0 (net10.0)
- **Build Tool**: .NET SDK
- **Database**: SQL Server LocalDB in development; Azure SQL Database is
  already defined in the repository's infrastructure
- **Key Dependencies**: Entity Framework Core 10, SixLabors.ImageSharp

---

## Overview

> This migration prepares PhotoAlbum for reliable operation in Azure. The
> application currently stores photos on the local file system and uses a
> local SQL Server connection for development. The new architecture will:
>
> - Store photos in Azure Blob Storage so they are available across application
>   instances.
> - Use Azure SQL Database for persistent application data in the cloud.
> - Provision the required Azure resources and deploy the application to Azure
>   Container Apps.
>
> The modernization follows a cloud-readiness, infrastructure, verification,
> and deployment sequence. The application and its test project already target
> .NET 10; no framework upgrade is included. Azure resources will use managed
> identity. The repository contains no OracleDB configuration, so the
> conditional Oracle-to-PostgreSQL migration does not apply.

---

## Migration Impact Summary

| App | Original service | Azure target | Auth | Goal |
|-----|------------------|--------------|------|------|
| PhotoAlbum | Local files | Azure Blob Storage | MI | Shared photos |
| PhotoAlbum | SQL LocalDB | Azure SQL Database | MI | Cloud database |
| PhotoAlbum | Not deployed | Azure Container Apps | MI | Hosting |

---

## Open Questions & Questionnaire

- [x] Q: Should the plan provision Azure infrastructure? → A: Yes; requested
  in the modernization requirements.
- [x] Q: Should integration verification be included? → A: Yes, using real
  provisioned Azure resources.
- [x] Q: Should security/CVE remediation be included? → A: Yes, the default.
- [x] Q: Which Azure deployment target should be used? → A: Azure Container
  Apps, as configured by the repository's `azure.yaml`.
- [ ] What Azure subscription ID should be used for provisioning?
