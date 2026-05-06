# AGENTS.md

## Bootstrap Rule

For any non-trivial task, start with `AI_CONTEXT.md`.

Do not try to understand the whole repository by scanning random files first. Use the bootstrap chain:

1. `AGENTS.md`
2. `AI_CONTEXT.md`
3. `docs/context/project-overview.md`
4. `docs/context/retrofit-mapping.md` (when present)
5. `docs/context/development-roadmap.md`
6. `docs/context/architecture.md`
7. `docs/context/tech-stack.md`
8. the relevant `docs/tasks/<module>/INDEX.md` files for the task
9. relevant files under `docs/engineering/`
10. relevant files under `docs/tasks/`

## Why This Exists

This repository is organized to reduce token waste across repeated AI sessions.

- `AGENTS.md` is the stable entrypoint.
- `AI_CONTEXT.md` is the compressed whole-project index.
- canonical detail is pushed into `docs/context/`, `docs/engineering/`, and `docs/tasks/`.

## Project Notes

- Project: `snipaste-ocr-helper`
- Shape: `single-app`
- Execution workflow: `superpowers`
- Product goal: Windows 后台托盘工具，配合 Snipaste 自动保存截图目录，自动 OCR 新图片并把识别文本写入剪贴板。
- Target users: 经常使用 Snipaste 截图并需要快速提取图片文字的 Windows 用户。

## Working Rule

When structure, architecture, or domain ownership changes, update `AI_CONTEXT.md`, the affected `docs/tasks/<module>/INDEX.md` files, and the affected canonical docs in the same change.

## Repository Structure Priority

- Repository-local structure and docs layout take priority over generic skill defaults.
- Superpowers skills may load and run normally, but they do not own this repository's documentation structure.
- Before writing any plan, design note, verification note, or task state from a superpowers workflow, map it back to the canonical paths below.
- If an external workflow suggests `docs/superpowers/**` or another generic artifact path, adapt it to this repository's canonical structure instead.
- Use:
  - `docs/context/` for stable context
  - `docs/product/` for product intent
  - `docs/engineering/` for technical contracts
  - `docs/tasks/` for task and module records
  - `docs/archive/` for historical docs, older project materials, and superseded planning artifacts
- Do not copy workflow habits from other repositories unless they are intentionally adopted here.

## Mandatory Writeback

- `docs/tasks/<module>/INDEX.md` files are the core module-state files for this repository.
- After each completed task, the AI must write back the latest effective status, notable shipped changes, and next-task implications to the affected module `INDEX.md` files.
- A single task should minimize its module impact whenever possible. Prefer work that stays within one module instead of spreading changes across unrelated modules.
- If a task genuinely crosses module boundaries, update every affected module `INDEX.md` so the repository state stays accurate.
- If historical execution docs or older project materials are no longer active, move them under `docs/archive/` instead of leaving them mixed into active context.

## Superpowers Mapping

Superpowers skills may load and run normally. Treat them as execution guidance, not documentation ownership.

If a complex task uses `using-superpowers` or related skills, map generic outputs into this repository as follows:

- brainstorming spec or design docs -> the active task record under `docs/tasks/<domain>/` or another canonical project doc if that is the real owner
- writing-plans implementation plans -> the active task record under `docs/tasks/<domain>/` unless a separate repository-level plan doc is intentionally introduced
- verification notes and evidence summaries -> `docs/testing/`
- durable architecture or boundary decisions -> `docs/context/architecture.md`
- durable API and engineering contracts -> `docs/engineering/`

Do not preserve generic superpowers directory names when they conflict with the repository's canonical layout.

## Repo-Native Workflow

Repo-native task records still own durable repository state. When superpowers is used, map its outputs back into `docs/tasks/`, `docs/testing/`, `docs/context/`, and `docs/engineering/`.
