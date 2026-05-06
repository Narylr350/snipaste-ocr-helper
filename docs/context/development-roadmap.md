# Development Roadmap

## Goal Route

先交付稳定的目录监听 OCR 剪贴板 MVP，再逐步加入云 OCR、历史记录、手动重试、打包安装和自启动。

## MVP Definition

用户可设置 Snipaste 自动保存目录；工具后台监听新增图片；等待图片文件稳定后加入 OCR 队列；OCR 成功后自动将文本写入剪贴板；托盘可显示运行状态并暂停或恢复；设置中可选择 OCR provider 并预留云 OCR API key 配置；保留基础日志用于排查失败。

## Milestones

- Phase 1: 目录监听、本地或占位 OCR provider、自动写剪贴板、托盘与设置
- Phase 2: 云 OCR provider、密钥配置、失败提示、性能参数
- Phase 3: OCR 历史、结果编辑、手动重试、快捷键
- Phase 4: 打包安装、自启动、自动更新

## Success Metrics

- 新截图保存后常见情况下 1-3 秒内完成 OCR 并写入剪贴板
- 连续截图时不重复处理同一文件
- 不读取未写完的图片文件
- OCR 失败不会阻塞后续图片处理
- 托盘状态能让用户判断工具是否正在运行
- 用户能在设置里完成目录和 OCR provider 配置

## Explicit Non-Goals

不实现截图功能；不替代 Snipaste UI；首版不做完整 OCR 历史管理；首版不做多平台支持；首版不做复杂图片编辑或区域框选；首版不强制接入某个云 OCR。

## Key Workflow Sequence

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## Risks and Open Questions

Risks:

- 云 OCR 准确率高但涉及隐私、费用和网络失败
- 自动覆盖剪贴板可能覆盖用户刚复制的其他内容
- 文件监听可能遇到写入未完成、重复事件和批量截图抖动
- 后台常驻需要控制 CPU、内存和通知打扰
- API key 存储需要避免明文暴露

Open questions:

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

## Notes

- This roadmap is for high-level delivery direction.
- Keep detailed implementation work in `docs/tasks/` and engineering contracts in `docs/engineering/`.
- Update this file when product scope or milestone meaning changes.
