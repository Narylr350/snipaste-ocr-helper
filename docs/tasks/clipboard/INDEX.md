# clipboard Module Index

## Current Status

`clipboard` is part of the initial `snipaste-ocr-helper` scaffold.

## Active Scope

用户可设置 Snipaste 自动保存目录；工具后台监听新增图片；等待图片文件稳定后加入 OCR 队列；OCR 成功后自动将文本写入剪贴板；托盘可显示运行状态并暂停或恢复；设置中可选择 OCR provider 并预留云 OCR API key 配置；保留基础日志用于排查失败。

## North Star Contribution

This domain supports the core flow:

用户设置 Snipaste 自动保存目录；工具监听新截图；等待文件写入稳定；执行 OCR；将识别文本自动写入剪贴板；托盘显示状态并允许暂停恢复。

## Implemented Features

- No implementation has shipped yet. This file records the intended domain state for the first build.

## Pending Features

Key workflows to implement or validate:

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`
- Product intent: `docs/product/idea.md`

## Validation

单元测试覆盖文件去重、防抖策略、OCR provider 结果处理和剪贴板适配层；手动验证 Snipaste 保存新截图后自动 OCR、连续截图不漏不重复、大图写入未完成时不提前 OCR、OCR 失败后队列继续工作、暂停恢复监听生效、设置目录变更后监听切换生效。

## Known Issues

- 云 OCR 准确率高但涉及隐私、费用和网络失败
- 自动覆盖剪贴板可能覆盖用户刚复制的其他内容
- 文件监听可能遇到写入未完成、重复事件和批量截图抖动
- 后台常驻需要控制 CPU、内存和通知打扰
- API key 存储需要避免明文暴露

## Next Useful Moves

Open questions:

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
