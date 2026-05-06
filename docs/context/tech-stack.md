# Tech Stack

## Selected Stack

- Runtime: C# / .NET desktop application
- Desktop UI: WPF for the first implementation pass
- Frontend: `none`
- Backend: `none`
- Mobile: `none`
- Package manager: `none`

## Current Fit

There is no standalone backend boundary in the current structure. The `single-app` root should become the Windows tray application, with file watching, OCR orchestration, settings, queueing, and clipboard integration kept inside the desktop app boundary.

## Product and Workflow Fit

- MVP scope: 用户可设置 Snipaste 自动保存目录；工具后台监听新增图片；等待图片文件稳定后加入 OCR 队列；OCR 成功后自动将文本写入剪贴板；托盘可显示运行状态并暂停或恢复；设置中可选择 OCR provider 并预留云 OCR API key 配置；保留基础日志用于排查失败。
- Core workflow pressure: 用户设置 Snipaste 自动保存目录；工具监听新截图；等待文件写入稳定；执行 OCR；将识别文本自动写入剪贴板；托盘显示状态并允许暂停恢复。
- Integration needs: Snipaste 只通过自动保存目录集成，不调用内部 API；OCR provider 分为本地 provider 与云 provider，云 provider 只有用户显式配置 API key 后才上传图片；剪贴板边界为 OCR 成功后自动覆盖为文本。

## Validation Strategy

单元测试覆盖文件去重、防抖策略、OCR provider 结果处理和剪贴板适配层；手动验证 Snipaste 保存新截图后自动 OCR、连续截图不漏不重复、大图写入未完成时不提前 OCR、OCR 失败后队列继续工作、暂停恢复监听生效、设置目录变更后监听切换生效。

## Open Technical Questions

- 首个云 OCR provider 选哪家
- API key 是否需要接入 Windows Credential Manager
- OCR 失败是否弹通知还是只写日志
- 后续是否加入 OCR 历史与手动重试界面

## Notes

- These are initial structure decisions, not final implementation lock-in.
- Update this file once runtime versions, infra, and tooling are chosen.
