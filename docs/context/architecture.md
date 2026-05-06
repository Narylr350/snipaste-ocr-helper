# Architecture

## Shape

`snipaste-ocr-helper` uses `single-app`.

## Application Boundaries

- `app` owns the Windows desktop application boundary.
- The first implementation should initialize this as a C#/.NET WPF tray application with settings UI.

## Core Workflow Boundaries

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## Shared Directories

- `tools`

## Integration Boundaries

Snipaste 只通过自动保存目录集成，不调用内部 API；OCR provider 分为本地 provider 与云 provider，云 provider 只有用户显式配置 API key 后才上传图片；剪贴板边界为 OCR 成功后自动覆盖为文本。

## Architecture Risks

- 云 OCR 准确率高但涉及隐私、费用和网络失败
- 自动覆盖剪贴板可能覆盖用户刚复制的其他内容
- 文件监听可能遇到写入未完成、重复事件和批量截图抖动
- 后台常驻需要控制 CPU、内存和通知打扰
- API key 存储需要避免明文暴露

## Open Architecture Questions

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

## Boundary Rules

- Keep business rules inside the app or service that owns them.
- Prefer shared packages for stable contracts, types, or utilities.
- Do not create a new app unless the user-facing or operational boundary is real.
- Keep docs synchronized when structure or boundaries change.
