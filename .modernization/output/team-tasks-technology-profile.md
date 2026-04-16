# Technology Profile — Team Tasks Demo (`appmod`)

Technology-specific inventory for reverse-engineering and migration. Source: `server/**/*.csproj`, `client/package.json`, `client/pnpm-lock.yaml`, `server/TeamTasks.Api/appsettings.json`, `server/TeamTasks.Api/Program.cs`, `client/vite.config.ts`, `client/tsconfig*.json`, `client/eslint.config.js`.

---

## .NET

| Item | Value | Source |
|------|--------|--------|
| Target framework | **net10.0** | `server/TeamTasks.Api/TeamTasks.Api.csproj:4`, `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj:4` |
| API project SDK | `Microsoft.NET.Sdk.Web` | `server/TeamTasks.Api/TeamTasks.Api.csproj:1` |
| Test project SDK | `Microsoft.NET.Sdk` | `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj:1` |
| Nullable reference types | enabled | both `.csproj` files |
| Implicit usings | enabled | both `.csproj` files |
| Test project | `IsPackable` false | `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj:7` |

**NuGet — `server/TeamTasks.Api/TeamTasks.Api.csproj`**

| Package | Version |
|---------|---------|
| Microsoft.EntityFrameworkCore | 10.0.5 |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.5 |
| Microsoft.AspNetCore.OpenApi | 10.0.5 |
| Swashbuckle.AspNetCore | 10.1.5 |

**NuGet — `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj`**

| Package | Version | Notes |
|---------|---------|--------|
| coverlet.collector | 8.0.1 | `PrivateAssets=all` |
| Microsoft.EntityFrameworkCore.InMemory | 10.0.5 | |
| Microsoft.NET.Test.Sdk | 18.3.0 | |
| xunit | 2.9.3 | |
| xunit.runner.visualstudio | 3.1.5 | `PrivateAssets=all` |

**Project reference:** test project → `server/TeamTasks.Api/TeamTasks.Api.csproj`.

**SDK pinning:** No `global.json` or `Directory.Build.props` in repo; .NET SDK version is not pinned in source control here. [NEEDS CLARIFICATION: installed SDK on build machines]

---

## Data store (SQLite)

| Item | Value | Source |
|------|--------|--------|
| Database file name | **teamtasks.db** | `server/TeamTasks.Api/appsettings.json:3`; fallback `Program.cs:14` |
| Connection string pattern | `Data Source=teamtasks.db` | same |

---

## Node client (`client/`)

| Item | Value | Source |
|------|--------|--------|
| Package manager | **pnpm** (lockfile present) | `client/pnpm-lock.yaml` |
| Lockfile format | `lockfileVersion: '9.0'` | `client/pnpm-lock.yaml:1` |
| Module system | ESM (`"type": "module"`) | `client/package.json:5` |

### Direct dependencies — declared vs resolved (`pnpm-lock.yaml` importers)

**`dependencies` (runtime)**

| Package | package.json range | Resolved version (lock) |
|---------|--------------------|-------------------------|
| react | ^19.2.4 | **19.2.4** |
| react-dom | ^19.2.4 | **19.2.4** |

**`devDependencies`**

| Package | package.json range | Resolved version (lock) |
|---------|--------------------|-------------------------|
| @eslint/js | ^10.0.1 | **10.0.1** |
| @testing-library/jest-dom | ^6.9.1 | **6.9.1** |
| @testing-library/react | ^16.3.2 | **16.3.2** |
| @testing-library/user-event | ^14.6.1 | **14.6.1** |
| @types/node | ^25.5.0 | **25.5.0** |
| @types/react | ^19.2.14 | **19.2.14** |
| @types/react-dom | ^19.2.3 | **19.2.3** |
| @vitejs/plugin-react | ^6.0.1 | **6.0.1** |
| eslint | ^10.1.0 | **10.1.0** |
| eslint-plugin-react-hooks | ^7.0.1 | **7.0.1** |
| eslint-plugin-react-refresh | ^0.5.2 | **0.5.2** |
| globals | ^17.4.0 | **17.4.0** |
| jsdom | ^29.0.1 | **29.0.1** |
| typescript | ~6.0.2 | **6.0.2** |
| typescript-eslint | ^8.57.2 | **8.57.2** |
| vite | ^8.0.2 | **8.0.2** |
| vitest | ^4.1.1 | **4.1.1** |

Resolved versions taken from `client/pnpm-lock.yaml` `importers`.`dependencies` / `devDependencies` entries (lines 9–68).

### Notable transitive (for build/test toolchain)

| Package | Version | Role (from lock graph) |
|---------|---------|-------------------------|
| esbuild | 0.27.4 | Vite dependency (`vite@8.0.2(...)(esbuild@0.27.4)`) |
| @testing-library/dom | 10.4.1 | Peer of Testing Library packages |

---

## Client tooling (scripts and configs)

| Tool | Version | Role |
|------|---------|------|
| **Vite** | 8.0.2 | Dev server, production build (`package.json` scripts `dev`, `build`, `preview`) |
| **@vitejs/plugin-react** | 6.0.1 | React + Fast Refresh in Vite |
| **TypeScript** | 6.0.2 | `tsc -b` in build script; project references `tsconfig.app.json`, `tsconfig.node.json` |
| **Vitest** | 4.1.1 | Unit/integration tests (`test`, `test:watch`); config merged via `vitest/config` in `vite.config.ts` |
| **jsdom** | 29.0.1 | Vitest `environment: 'jsdom'` (`client/vite.config.ts`) |
| **ESLint** | 10.1.0 | `lint` script: `eslint .` |
| **@eslint/js** | 10.0.1 | Flat config base (`client/eslint.config.js`) |
| **typescript-eslint** | 8.57.2 | Type-aware lint configs in `eslint.config.js` |
| **eslint-plugin-react-hooks** | 7.0.1 | React Hooks rules |
| **eslint-plugin-react-refresh** | 0.5.2 | React Refresh rules for Vite |
| **globals** | 17.4.0 | Browser globals for ESLint |

**npm scripts** (`client/package.json`): `dev` (vite), `build` (tsc -b && vite build), `test` (vitest run), `test:watch` (vitest), `lint` (eslint .), `preview` (vite preview).

**TypeScript compiler options (high level):** `tsconfig.app.json` — target ES2022, strict, bundler resolution, `jsx: react-jsx`, Vitest globals types. `tsconfig.node.json` — target ES2023, includes `vite.config.ts`.

---

## Test stack summary

| Layer | Technologies | Versions |
|-------|----------------|----------|
| **.NET API** | xUnit, Microsoft.NET.Test.Sdk, coverlet.collector, EF Core InMemory | 2.9.3, 18.3.0, 8.0.1, 10.0.5 |
| **React client** | Vitest, Testing Library (React, jest-dom, user-event), jsdom | 4.1.1, 16.3.2 / 6.9.1 / 14.6.1, 29.0.1 |

---

## Repository gaps (technology)

- No `.sln` file in repo root (projects build via `.csproj` directly). [inferred from filesystem]
- No `Dockerfile` or `.github` workflows found in repo for CI/CD tooling. [inferred from filesystem]
- Node **engines** field not set in `client/package.json`. [NEEDS CLARIFICATION: supported Node major]

---

## Traceability index

| Artifact | Path |
|----------|------|
| API csproj | `server/TeamTasks.Api/TeamTasks.Api.csproj` |
| Test csproj | `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj` |
| Client manifest | `client/package.json` |
| pnpm lock | `client/pnpm-lock.yaml` |
| SQLite name | `server/TeamTasks.Api/appsettings.json`, `server/TeamTasks.Api/Program.cs` |
| Vite + Vitest | `client/vite.config.ts` |
| ESLint flat config | `client/eslint.config.js` |
