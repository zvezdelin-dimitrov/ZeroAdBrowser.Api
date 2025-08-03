# ZeroAdBrowser.Server

## Overview
ZeroAdBrowser.Server is a modular solution for providing tracker data via both a REST API and Azure Functions. It leverages Azure Blob Storage and Redis Cache for efficient data storage and retrieval.

## Projects

- **ZeroAdBrowser.Api**  
  ASP.NET Core Web API exposing tracker data at `/trackers`.

- **ZeroAdBrowser.AzureFunctions**  
  Azure Functions project exposing tracker data via serverless HTTP endpoints.

- **ZeroAdBrowser.TrackersProvider**  
  Shared library handling tracker data retrieval, caching, and storage logic.

## Features

- Fetches tracker lists from a remote URL.
- Caches tracker data in Redis for performance.
- Stores tracker data in Azure Blob Storage.
- Exposes tracker data via REST API and Azure Functions.

## Configuration

Configuration is managed via `trackersprovider.appsettings.json`, environment variables, and user secrets.  
Key settings include:

- Azure Blob Storage connection string, container, and blob name
- Redis Cache connection string, instance name, cache key, and duration
- Tracker list source URL.
