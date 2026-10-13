# Modernization Plan: Cloud and Azure Modernization

**Project**: PhotoAlbum

---

## Technical Framework

- **Language**: C# (.NET 10)
- **Framework**: ASP.NET Core 10 Razor Pages
- **Build Tool**: .NET SDK / dotnet CLI
- **Database**: SQL Server LocalDB; target Azure SQL Database
- **Key Dependencies**: Entity Framework Core 10, SixLabors.ImageSharp

---

## Overview

> This modernization prepares PhotoAlbum for cloud operation and deploys it to Azure. The application currently stores uploaded images on local disk and uses SQL Server LocalDB for photo metadata. The repository already contains Azure Container Apps deployment configuration and Bicep infrastructure for the intended cloud services. The new architecture will:
>
> - Store uploaded photos in Azure Blob Storage so files are not tied to an individual app instance.
> - Use Azure SQL Database for persistent photo metadata.
> - Provision the required Azure resources and deploy the application to Azure Container Apps.
>
> The plan focuses on cloud readiness, infrastructure, security validation, and deployment. The application already targets .NET 10; no framework upgrade is included. The repository contains one application, and no Oracle Database was found.

---

## Migration Impact Summary

| Application | Original Service | New Azure Service | Authentication | Comments |
|-------------|------------------|-------------------|----------------|----------|
| PhotoAlbum | Local photo files | Azure Blob Storage | Managed identity | Store uploads independently of app instances |
| PhotoAlbum | SQL Server LocalDB | Azure SQL Database | Managed identity | Persist photo metadata in Azure |
| PhotoAlbum | Local application hosting | Azure Container Apps | Managed identity | Provision and deploy using existing Azure assets |

---

## Security Compliance

Scan project dependencies for known CVEs and remediate identified vulnerabilities before deployment. Verify the application builds and its tests pass after remediation.

---

## Open Questions & Questionnaire

- [x] Q: Should the plan provision infrastructure? → A: Yes; provision the requested Azure resources, building on the repository's existing Bicep configuration.
- [x] Q: Should integration testing be included? → A: Yes; use real provisioned Azure services, following the questionnaire default when infrastructure is provisioned.
- [x] Q: Should dependency security and CVE remediation be included? → A: Yes; use the default.
- [x] Q: Which Azure deployment target should be used? → A: Azure Container Apps, matching the existing `azure.yaml` and Bicep configuration.
- [x] Q: Should a separate containerization task be included? → A: No; Container Apps deployment includes containerization.
- [ ] What Azure subscription ID and deployment region should be used? Provide these when provisioning the resources.
