# API Draft

## Applicability

This project does not currently have a standalone API boundary, so this document remains a light placeholder until an API surface is introduced.

## API Consumers and Workflows

- 设置监听目录并启动后台监听
- Snipaste 保存新截图后自动 OCR 并覆盖剪贴板文本
- 连续截图时按队列去重处理
- 暂停和恢复目录监听
- 切换 OCR provider 并保存配置
- OCR 失败后记录日志并继续处理后续图片

## MVP API Scope

首版没有对外 HTTP API；主要边界是内部 OCR provider 接口、本地配置读写接口、Windows 剪贴板适配层和文件系统监听适配层。云 OCR provider 作为可配置外部 API 边界。

## Integration Notes

Snipaste 只通过自动保存目录集成，不调用内部 API；OCR provider 分为本地 provider 与云 provider，云 provider 只有用户显式配置 API key 后才上传图片；剪贴板边界为 OCR 成功后自动覆盖为文本。

## Out of Scope

不实现截图功能；不替代 Snipaste UI；首版不做完整 OCR 历史管理；首版不做多平台支持；首版不做复杂图片编辑或区域框选；首版不强制接入某个云 OCR。

## Notes

- This file is only a starting point for structure.
- Replace these route groups with concrete contracts once implementation begins when the project has a real API boundary.
