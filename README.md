# snipaste-ocr-helper

## Idea

Windows 后台托盘工具，配合 Snipaste 自动保存截图目录，自动 OCR 新图片并把识别文本写入剪贴板。

## Recommended Shape

- Shape: `single-app`
- Runtime: C# / .NET desktop application
- UI: WPF recommended for the first implementation pass
- Frontend: `none`
- Backend: `none`
- Mobile: `none`
- Admin: `false`
- Worker: `false`
- Package manager: `none`
- Execution workflow: `superpowers`

## Structure

```text
snipaste-ocr-helper/
├─ .gitignore
├─ AGENTS.md
├─ AI_CONTEXT.md
├─ CLAUDE.md
├─ README.md
├─ app/
├─ docs/
│  ├─ archive/
│  ├─ context/
│  ├─ engineering/
│  ├─ product/
│  ├─ tasks/
│  └─ testing/
└─ tools/
```

## Docs Entry Points

- `AI_CONTEXT.md`
- `docs/context/project-overview.md`
- `docs/context/retrofit-mapping.md` (when present)
- `docs/context/architecture.md`
- `docs/context/tech-stack.md`
- `docs/context/development-roadmap.md`

## Core Domains

- `platform`
- `watcher`
- `ocr`
- `clipboard`
- `tray`
- `settings`
- `queue`

## Delivery Workflow

- Superpowers may load and trigger its skills according to the local agent environment.
- When superpowers is used, map its outputs into this repository's canonical paths instead of creating generic workflow directories.
- Repository documentation ownership stays with `AGENTS.md`, `AI_CONTEXT.md`, and the canonical docs under `docs/`.

## Repo-Native Workflow

Repo-native task records still own durable repository state. When superpowers is used, map its outputs back into `docs/tasks/`, `docs/testing/`, `docs/context/`, and `docs/engineering/`.
