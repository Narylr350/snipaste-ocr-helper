# Snipaste OCR Helper

Snipaste OCR Helper 是一个 Windows 托盘工具，用于配合 Snipaste 的自动保存截图目录，把新截图中的文字自动识别并写入剪贴板。

## 功能

- 监控 Snipaste 自动保存截图目录
- 等待图片文件写入稳定后自动 OCR
- 识别到非空文本时自动写入剪贴板
- 支持中英文混合识别（`eng+chi_sim`）
- 内置 Tesseract 语言数据，首次启动自动解压
- 托盘菜单支持打开设置、暂停/恢复监控和退出
- 支持开机自启动开关

## 运行要求

- Windows x64
- .NET 8 Windows Desktop Runtime

## 使用方法

1. 从 GitHub Releases 下载最新的 `SnipasteOcrHelper` exe。
2. 启动程序。
3. 首次启动会打开设置窗口。
4. 在设置窗口中选择 Snipaste 的截图自动保存目录。
5. 保存后程序会在后台托盘运行。
6. Snipaste 保存新截图后，识别文本会自动写入剪贴板。

## 数据位置

- 设置文件：`%APPDATA%\SnipasteOcrHelper\settings.json`
- 日志文件：`%LOCALAPPDATA%\SnipasteOcrHelper\logs\app.log`
- OCR 语言数据：`%LOCALAPPDATA%\SnipasteOcrHelper\tessdata`

## 注意事项

- 程序会在识别成功后直接覆盖当前剪贴板文本。
- 当前版本使用本地 Tesseract OCR，不上传截图。
- exe 体积较大是因为内置了中英文 OCR 语言数据。
