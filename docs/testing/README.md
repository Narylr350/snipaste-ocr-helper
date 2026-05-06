# Testing Notes

Use this directory for validation notes that should survive across AI sessions.

## Testing Strategy

单元测试覆盖文件去重、防抖策略、OCR provider 结果处理和剪贴板适配层；手动验证 Snipaste 保存新截图后自动 OCR、连续截图不漏不重复、大图写入未完成时不提前 OCR、OCR 失败后队列继续工作、暂停恢复监听生效、设置目录变更后监听切换生效。

## Standard Checks

- Define the concrete test, lint, typecheck, and build commands when the runtime is initialized.
- Keep each command here once it exists so future AI sessions can verify work without rediscovery.

## Manual Checks

Use these product workflows as manual validation targets until automated coverage exists:

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## Recent Evidence

- No implementation verification has been recorded yet. Add dated evidence here after the first build, test, or manual validation run.

## Known Gaps

Risks and gaps to account for:

- 云 OCR 准确率高但涉及隐私、费用和网络失败
- 自动覆盖剪贴板可能覆盖用户刚复制的其他内容
- 文件监听可能遇到写入未完成、重复事件和批量截图抖动
- 后台常驻需要控制 CPU、内存和通知打扰
- API key 存储需要避免明文暴露

Open validation questions:

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

Keep task-specific detail in `docs/tasks/<module>/`. Keep durable validation rules and cross-module evidence here.
