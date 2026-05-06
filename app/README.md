# 主应用

## 作用

垂直 MVP 的 WPF 托盘应用。

## 项目

- `SnipasteOcrHelper.App`：目标框架为 `net8.0-windows` 的 Windows 托盘应用。
- `SnipasteOcrHelper.Tests`：覆盖设置、文件监控基础逻辑、队列编排、适配器、开机启动、日志和应用图标接入的 xUnit 测试。

## 构建和测试

```bash
dotnet test SnipasteOcrHelper.sln
dotnet build SnipasteOcrHelper.sln -c Release
```

## 发布

MVP 发布目标是依赖框架的 win-x64 单文件应用：

```bash
dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true --source https://api.nuget.org/v3/index.json
```

输出路径：

```text
app/SnipasteOcrHelper.App/bin/Release/net8.0-windows/win-x64/publish/SnipasteOcrHelper.App.exe
```

目标机器需要安装 .NET 8 Windows Desktop Runtime。exe 包含内置应用图标，嵌入 `eng` 和 `chi_sim` tessdata，并在启动时把 tessdata 解压到 `%LOCALAPPDATA%\SnipasteOcrHelper\tessdata`。
