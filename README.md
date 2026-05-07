# Snipaste OCR Helper

Snipaste OCR Helper 是一个 Windows 后台托盘工具，用来把 Snipaste 截图自动变成可复制的文字。

它会监控 Snipaste 的自动保存截图目录。当新截图保存完成后，程序会在本地执行 OCR，把识别出来的文字写入剪贴板，并保留识别记录，方便你回看、复制和排查失败原因。

## 解决什么痛点

很多人用 Snipaste 截图后，还要手动打开 OCR 软件、拖入图片、复制结果。这个流程很打断工作：截图已经保存了，但文字还要再手动处理一次。

Snipaste OCR Helper 解决的是这个重复动作：

- 截图保存后自动识别，不需要手动导入图片。
- 识别文本自动进入剪贴板，可以直接粘贴到聊天、文档、代码编辑器里。
- OCR 完全在本地运行，不需要上传截图。
- 识别记录、托盘状态和日志能告诉你每张图是否识别成功。
- 首次启动会引导你选择截图目录、OCR 引擎并下载需要的模型/语言包。

## 主要功能

- 监控 Snipaste 自动保存截图目录。
- 自动等待图片写入稳定后再识别。
- 默认使用 RapidOCR，本地识别效果更好。
- 可切换 Tesseract，并下载常用语言包。
- 支持中英文界面随系统语言切换。
- 自动把识别文本写入剪贴板。
- 提供识别记录窗口，可查看和复制历史结果。
- 托盘菜单支持打开设置、查看识别记录、暂停/恢复监控和退出。
- 支持开机自启动。
- 支持识别后图片处理：不删除、识别成功后删除、总是删除到回收站。
- 设置页可打开日志文件夹，方便排查下载或识别问题。

## 技术栈

- .NET 8
- WPF
- Windows Forms NotifyIcon / Clipboard
- RapidOcrNet
- TesseractOCR
- xUnit
- Inno Setup

## 安装和使用

1. 在 GitHub Releases 下载最新的 `SnipasteOcrHelper-*-setup.exe` 安装器。
2. 运行安装器并启动程序。
3. 首次启动时，根据引导页选择：
   - Snipaste 自动保存截图目录
   - OCR 引擎，默认推荐 RapidOCR
   - 对应 OCR 资源并下载
4. 完成后程序会在后台托盘运行。
5. 使用 Snipaste 截图并保存到监控目录后，识别文本会自动写入剪贴板。

## 数据位置

- 设置文件：`%APPDATA%\SnipasteOcrHelper\settings.json`
- Tesseract 语言包：`%APPDATA%\SnipasteOcrHelper\tessdata`
- RapidOCR 模型：`%APPDATA%\SnipasteOcrHelper\rapidocr-models`
- 日志文件：`%LOCALAPPDATA%\SnipasteOcrHelper\logs\app.log`

## 隐私说明

Snipaste OCR Helper 使用本地 OCR。截图不会被上传到云端服务。

如果你在设置页下载 RapidOCR 模型或 Tesseract 语言包，程序只会向对应模型/语言包下载源发起网络请求。

## 开源协议

本项目使用 MIT License。你可以自由使用、复制、修改、分发和用于商业用途，只需保留许可证声明。
