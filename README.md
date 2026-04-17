# 惯蛋本地原型

## 项目概述

这是一个采用 `Domain / Rules / Application / Web` 四层架构实现的惯蛋本地原型，目标是先交付“单局完整、赛制简化”的可玩版本。当前版本支持 4 人本地对局、双副牌发牌、牌型识别、合法性校验、轮次推进、队伍结算以及浏览器中的简单操作界面。

## 当前规则范围

当前实现的“简化完整规则”包括：

- 4 名玩家固定座位，本地轮流操作
- 双副牌共 108 张，每人 27 张
- 默认级牌为 `2`
- 红桃级牌作为通配牌
- 支持牌型：
  - 单张
  - 对子
  - 三张
  - 三带二
  - 顺子
  - 连对
  - 钢板
  - 炸弹
  - 同花顺
  - 王炸
- 支持跟牌、过牌、三家不跟后重置牌权
- 支持一局结束后的名次、胜方队伍和双下判定

当前**不实现**：

- 进贡 / 还贡 / 抗贡
- 多局升级
- 联网房间
- AI 托管

## 项目结构

```text
src/
  Guandan.Game.Domain/
  Guandan.Game.Rules/
  Guandan.Game.Application/
  Guandan.Game.Web/
tests/
  Guandan.Game.Domain.Tests/
  Guandan.Game.Rules.Tests/
  Guandan.Game.Application.Tests/
docs/
  superpowers/specs/
  superpowers/plans/
```

## 启动方式

### 1. 编译

```bash
dotnet build guandan-game.sln --no-restore -m:1 /nodeReuse:false
```

### 2. 启动 Web 原型

```bash
dotnet run --project src/Guandan.Game.Web/Guandan.Game.Web.csproj
```

默认访问：

- `http://localhost:5000`
- `https://localhost:5001`

如需固定到 `9009` 端口，可自行设置：

```bash
ASPNETCORE_URLS=http://localhost:9009 dotnet run --project src/Guandan.Game.Web/Guandan.Game.Web.csproj
```

## 测试方式

当前测试采用**离线控制台测试宿主**，不依赖外部 NuGet 测试包，适合当前网络受限环境。

```bash
dotnet run --project tests/Guandan.Game.Domain.Tests/Guandan.Game.Domain.Tests.csproj --no-build
dotnet run --project tests/Guandan.Game.Rules.Tests/Guandan.Game.Rules.Tests.csproj --no-build
dotnet run --project tests/Guandan.Game.Application.Tests/Guandan.Game.Application.Tests.csproj --no-build
```

## 设计与计划文档

- 规格文档：`docs/superpowers/specs/2026-04-17-guandan-simplified-rules-design.md`
- 实现计划：`docs/superpowers/plans/2026-04-17-guandan-simplified-rules.md`

## 说明

1. 这是惯蛋，不是 UNO。
2. 当前优先保证规则引擎和本地原型可运行。
3. 后续若扩展进贡、多局升级，应继续遵守现有四层边界。
