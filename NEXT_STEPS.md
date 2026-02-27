# RecruitAI Service — Recommended Next Implementation Steps

This document summarizes the highest-impact next tasks after reviewing the current codebase.

## 1) Fix authentication and authorization flow issues (P0)

- Return `401 Unauthorized` for invalid login credentials instead of `500`.
  - `AuthController.Login` currently catches all exceptions and maps them to 500, while `UserService.LoginAsync` throws `InvalidOperationException` for invalid credentials.
- Validate authenticated claims safely before calling `Guid.Parse(...)` in candidate and recruiter endpoints to avoid runtime errors when claim values are missing or malformed.
- In recruiter chat session creation, remove unused `recruiterId` local variable and enforce recruiter-role access explicitly.

## 2) Introduce request validation (P0)

- Add model validation attributes to DTOs (`[Required]`, length constraints, email format checks, password minimum rules).
- Enable consistent validation responses for bad payloads.
- Add file validation for resume uploads (allowed MIME types/extensions and max file size).

## 3) Harden security for stored files (P0)

- Sanitize uploaded filenames and avoid persisting raw `file.FileName` in generated paths.
- Move resume storage to non-public/private storage abstraction with signed URL access if needed.
- Add antivirus/scan hook before accepting uploaded files.

## 4) Implement resume text extraction pipeline (P1)

- Replace placeholder logic (`resumeText = ""`) with real extraction support for PDF/DOCX.
- Store extraction metadata (status, error reason, processing time) in DB.
- Process extraction asynchronously (background queue/job) to keep API responsive.

## 5) Add recruiter chat ownership and permissions checks (P1)

- Ensure message write/read operations verify that the session belongs to the authenticated recruiter.
- Add role-based authorization for recruiter chat endpoints.
- Add basic anti-abuse controls (message length cap, rate limits).

## 6) Move service code to async EF Core patterns (P1)

- Replace sync `FirstOrDefault` calls in async service methods with `FirstOrDefaultAsync` and add cancellation-token support.
- Ensure all DB operations consistently use async APIs.

## 7) Improve API consistency and architecture boundaries (P2)

- Introduce interfaces for all services (not only recruiter chat) and depend on abstractions in controllers.
- Centralize exception handling with middleware for consistent error contracts.
- Consider splitting DTO mapping into dedicated mappers for maintainability.

## 8) Expand observability (P2)

- Add structured logs around auth, profile updates, uploads, and chat actions.
- Add request correlation IDs and include user/session context in logs (without sensitive data).
- Add health checks for DB and storage dependencies.

## 9) Testing strategy (P0/P1)

- Add unit tests for `UserService`, `CandidateService`, and `RecruiterChatService` (success + failure paths).
- Add integration tests for auth flows and candidate/recruiter protected endpoints.
- Add upload endpoint tests for invalid type/oversize files and claim parsing failures.

## 10) Documentation and local developer experience (P2)

- Expand `README.md` with setup instructions, migration commands, env vars, and example API usage.
- Add architecture overview and endpoint matrix with required roles.
- Add `.http` / Postman examples for end-to-end smoke tests.

---

## Suggested implementation order

1. Auth/error mapping and claims safety.
2. DTO/request validation and file upload hardening.
3. Chat session ownership/authorization checks.
4. Resume extraction async pipeline.
5. Test coverage + docs improvements.
