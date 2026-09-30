# CLAUDE.md

Small ASP.NET Core Web API for managing tasks. It is a learning project, intentionally not a production system: keep changes simple and minimal, and keep it easy to reason about end-to-end.

## Commands

Run from the repo root (`TaskManagement.slnx` ties both projects together).

```
dotnet build                                                          # build solution
dotnet run --project src/TaskManagement.Api                           # run API (http://localhost:5135, swagger at /swagger)
dotnet test                                                           # run all tests
dotnet test --filter "FullyQualifiedName~TaskServiceTests"            # run one test class
dotnet test --filter "FullyQualifiedName~Post_EmptyTitle_Returns400"  # run one test
```

## Project gotchas (not obvious from the code)

- **No DTOs, on purpose.** Controller actions bind and return `TaskItem` directly. Do not introduce DTOs or extra abstraction layers unless asked.
- **No `Async` suffix on controller actions** (`GetById`, not `GetByIdAsync`). MVC strips the suffix when resolving action names, which breaks the `nameof(...)` used in `CreatedAtAction`.
- **No EF Core migrations.** The schema is created by `Database.EnsureCreated()` in `Program.cs`. After changing an entity (`TaskItem`, `AppDbContext`), delete `src/TaskManagement.Api/taskmanagement.db` so it is recreated; `EnsureCreated()` never migrates an existing file.
- **Validation is declarative on the model** (`[Required(AllowEmptyStrings = false)]` on `Title`). `[ApiController]` returns 400 automatically, so do not check `ModelState` manually in controllers or services.
- **`public partial class Program;`** at the bottom of `Program.cs` must stay: `WebApplicationFactory<Program>` in the integration tests depends on it.
- Tests use SQLite in-memory (`DataSource=:memory:`) and never touch the real `taskmanagement.db`.

## Working rules

### Evidence, no guessing
- Every claim ("root cause is X", "fixed", "tests pass") must be backed by concrete evidence: the code you actually read (file and line) or command output you actually ran.
- Never guess or invent results. If something was not run or verified, say so explicitly.
- Report your own mistakes and failing results as openly as successes.

### Plan first
- Present a plan and wait for my explicit OK before acting when a task: (a) edits 2 or more files, (b) changes an entity or the DB schema, or (c) performs any git write (commit, push).
- Reading files and running `dotnet build` / `dotnet test` do not need a plan.

### Coding
- Every `public` or `internal` method in `src/` gets an XML doc comment with `<summary>`, a `<param>` for each parameter, and `<returns>` when it returns a value.
- Controllers talk to `ITaskService` only, never to `AppDbContext` directly.

### Testing
- Before writing tests for a feature, present a test plan and wait for my OK. It must list cases for: happy path, validation errors, not found, and edge cases.
- Never mark a case pass or fail without actually running it.
- Evidence means the raw `dotnet test` output pasted in the reply: pass/fail counts, and for every failing test its name and error message.

### Source control
- Before every commit, list every file changed, each marked created / edited / deleted.
- Commit only after the full `dotnet test` run shows 0 failed, with the output pasted as evidence.
- Report what will be pushed before every push.
- Never commit secrets: `.env`, `appsettings.*.json` containing secrets, `*.pfx`, `*.key`, user-secrets files, or anything holding API keys or connection strings with passwords. If unsure, ask.