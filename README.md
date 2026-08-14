# Fitness Tracker API - Build With .Net

An ASP.Net Core Web API for managing a fitness gym system - members, trainers, exercises and workout sessions - built with a string focus on clean API design and Onion Architecture.

## Architecture

The project follows **Onion Architecture**, structure into four layers:

- **Domain** - core entities, DTOs and enums
- **Repository** - generic repository pattern for data access
- **Service** - business logic and validation rules
- **Web** - controllers, request/response models and API endpoints
The full data model is available as an ER diagram in this repository.

## Features

- **Full CRUD** for all domain entities, backed by a local PostgreSQL database, with pagination and eager loading (via *.Include()*).
- **ETL Integration** - a background service that syncs exercise data from external (wger.de) API into the local *Exercise*
- **External API Integration**
  - *Outbound*: calls to the wger API for exercise data.
  - *Inbound*: an API-key-authenticated endpoint that lets external systems submit workout session data.
- **Async Message Queue Processing** — inbound workout session submissions are accepted immediately and queued for background processing via Quartz.
- **Business Rule Enforcement** — workout sessions can only be created for members with an active, non-expired membership.
- **Authentication** — JWT-based authentication for internal users (register/login);
- **Excel Import/Export** — export a member's full workout history to a formatted `.xlsx` and import workout sessions from a spreadsheet with per-row validation.

## Testing

All endpoints have been manually tested end to end using Postman, including edge cases such as invalid foreign keys, expired membership.
