# AI_CONTEXT

## Purpose

This file is the mandatory bootstrap entry for the repository.

It exists to reduce repeated full-project context gathering in new AI conversations. The root `AGENTS.md` and `CLAUDE.md` should route new sessions here first.

This file should stay compressed: summarize the project and point to canonical documents instead of duplicating long-form detail.

## Project

`snipaste-ocr-helper` is currently structured as `single-app`.

Working product summary:

- goal: Windows 后台托盘工具，配合 Snipaste 自动保存截图目录，自动 OCR 新图片并把识别文本写入剪贴板。
- target users: 经常使用 Snipaste 截图并需要快速提取图片文字的 Windows 用户。
- core flow: 用户设置 Snipaste 自动保存目录；工具监听新截图；等待文件写入稳定；执行 OCR；将识别文本自动写入剪贴板；托盘显示状态并允许暂停恢复。

Active application roots:
- `app`: Main application

Primary domains:
- `platform`
- `watcher`
- `ocr`
- `clipboard`
- `tray`
- `settings`
- `queue`

## Required Reading Order

For any non-trivial task, read in this order before implementation:

1. `AI_CONTEXT.md`
2. `docs/context/project-overview.md`
3. `docs/context/retrofit-mapping.md` (when present)
4. `docs/context/development-roadmap.md`
5. `docs/context/architecture.md`
6. `docs/context/tech-stack.md`
7. the relevant `docs/tasks/<module>/INDEX.md` files for the task
8. relevant files under `docs/engineering/`
9. relevant files under `docs/tasks/`

## Canonical Ownership

- product goal and active surfaces: `docs/context/project-overview.md`
- legacy-to-canonical structure mapping: `docs/context/retrofit-mapping.md` (when present)
- target route and milestone phases: `docs/context/development-roadmap.md`
- structure and boundaries: `docs/context/architecture.md`
- stack decisions and runtime baseline: `docs/context/tech-stack.md`
- engineering contracts: `docs/engineering/`
- current module delivery state: the affected `docs/tasks/<module>/INDEX.md` files
- module and work records: `docs/tasks/`

## Why This Layering Exists

- `AGENTS.md` / `CLAUDE.md` provide the universal entry rule.
- `AI_CONTEXT.md` gives the shortest possible whole-project bootstrap.
- `docs/context/` stores stable project meaning and structure.
- `docs/engineering/` stores contracts that implementation work must follow.
- `docs/tasks/<module>/INDEX.md` stores the evolving state summary for each module.
- `docs/tasks/` stores module-local working history and current notes.
- `docs/archive/` stores historical docs, archived project materials, and superseded execution support docs.

## Delivery Workflow

- Superpowers may load and trigger its skills according to the local agent environment.
- When superpowers is used, map its outputs into this repository's canonical paths instead of creating generic workflow directories.
- Repository documentation ownership stays with `AGENTS.md`, `AI_CONTEXT.md`, and the canonical docs under `docs/`.

## Workflow Compatibility Rules

- Repository-local instructions override generic external workflow defaults.
- `using-superpowers` may be used for complex execution work, but it must adapt to this repository's structure.
- Do not create generic workflow artifact paths such as `docs/superpowers/**` when this repository already defines canonical locations.
- Use this repository's current structure for durable artifacts:
  - stable project context -> `docs/context/`
  - product intent -> `docs/product/`
  - engineering contracts -> `docs/engineering/`
  - task and module working records -> `docs/tasks/`
- When a generic skill suggests a different artifact layout, reinterpret that guidance so outputs land in the canonical paths above.

## Repo-Native Workflow

Repo-native task records still own durable repository state. When superpowers is used, map its outputs back into `docs/tasks/`, `docs/testing/`, `docs/context/`, and `docs/engineering/`.

## Superpowers Artifact Mapping

Superpowers skills may load and run normally. Treat them as execution guidance, not documentation ownership.

If a complex task uses `using-superpowers` or related skills, map generic outputs into this repository as follows:

- brainstorming spec or design docs -> the active task record under `docs/tasks/<domain>/` or another canonical project doc if that is the real owner
- writing-plans implementation plans -> the active task record under `docs/tasks/<domain>/` unless a separate repository-level plan doc is intentionally introduced
- verification notes and evidence summaries -> `docs/testing/`
- durable architecture or boundary decisions -> `docs/context/architecture.md`
- durable API and engineering contracts -> `docs/engineering/`

Do not preserve generic superpowers directory names when they conflict with the repository's canonical layout.

## Constraint Scope

- This repository should store repository-level workflow constraints only.
- Do not import personal machine-specific preferences, branch habits, or source-repo-specific rules unless they are intentionally adopted into this repository.
- If a future team wants extra local conventions, write them explicitly in this repository instead of inheriting them accidentally from another project.

## Execution Expectations

1. define scope
2. confirm the target app or module
3. keep the task impact as local to one module as possible
4. make the smallest structure-aware change
5. write back the latest effective task state to the affected `docs/tasks/<module>/INDEX.md` files
6. update affected docs if boundaries or contracts move
7. archive superseded history under `docs/archive/` when it is no longer active context
8. leave the repository consistent for the next task
