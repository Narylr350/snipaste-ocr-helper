# Project Overview

## Product Goal

Windows 后台托盘工具，配合 Snipaste 自动保存截图目录，自动 OCR 新图片并把识别文本写入剪贴板。

## Target Users

经常使用 Snipaste 截图并需要快速提取图片文字的 Windows 用户。

## Core Flow

用户设置 Snipaste 自动保存目录；工具监听新截图；等待文件写入稳定；执行 OCR；将识别文本自动写入剪贴板；托盘显示状态并允许暂停恢复。

## MVP Scope

用户可设置 Snipaste 自动保存目录；工具后台监听新增图片；等待图片文件稳定后加入 OCR 队列；OCR 成功后自动将文本写入剪贴板；托盘可显示运行状态并暂停或恢复；设置中可选择 OCR provider 并预留云 OCR API key 配置；保留基础日志用于排查失败。

## Explicit Non-Goals

不实现截图功能；不替代 Snipaste UI；首版不做完整 OCR 历史管理；首版不做多平台支持；首版不做复杂图片编辑或区域框选；首版不强制接入某个云 OCR。

## Success Metrics

- 新截图保存后常见情况下 1-3 秒内完成 OCR 并写入剪贴板
- 连续截图时不重复处理同一文件
- 不读取未写完的图片文件
- OCR 失败不会阻塞后续图片处理
- 托盘状态能让用户判断工具是否正在运行
- 用户能在设置里完成目录和 OCR provider 配置

## Key Workflows

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## Project Shape

The repository uses `single-app`.

## Delivery Direction

The canonical delivery direction lives in `docs/context/development-roadmap.md`.



## Active Applications

- `app`: Main application

## Core Domains

- `platform`
- `watcher`
- `ocr`
- `clipboard`
- `tray`
- `settings`
- `queue`

## Risks

- 云 OCR 准确率高但涉及隐私、费用和网络失败
- 自动覆盖剪贴板可能覆盖用户刚复制的其他内容
- 文件监听可能遇到写入未完成、重复事件和批量截图抖动
- 后台常驻需要控制 CPU、内存和通知打扰
- API key 存储需要避免明文暴露

## Open Questions

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

## Active Task State

The core evolving task-state file lives in `docs/tasks/platform/INDEX.md`.

## Historical Archive

Historical docs, older project materials, and superseded execution support files should be moved under `docs/archive/`.
