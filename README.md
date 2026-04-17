# 惯蛋（Guandan）Web应用 🃏

![.NET](https://github.com/qingshiqianmo/guandan-game/actions/workflows/dotnet.yml/badge.svg)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET Version](https://img.shields.io/badge/.NET-9.0-purple.svg)

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

## 🚀 快速开始

### 运行要求
- .NET 9.0 SDK 或更高版本

### 运行步骤
1. 克隆仓库：
   ```bash
   git clone https://github.com/qingshiqianmo/guandan-game.git
   cd guandan-game
   ```

2. 构建项目：
   ```bash
   dotnet build
   ```

3. 运行Web应用：
   ```bash
   cd src/Guandan.Game.Web
   dotnet run
   ```

4. 打开浏览器访问：
   - http://localhost:9009

## 📁 项目结构

```text
src/
  Guandan.Game.Domain/          # 领域模型层
  Guandan.Game.Rules/           # 规则引擎层
  Guandan.Game.Application/     # 应用服务层
  Guandan.Game.Web/             # Web界面层
tests/
  Guandan.Game.Domain.Tests/    # 领域模型测试
  Guandan.Game.Rules.Tests/     # 规则引擎测试
  Guandan.Game.Application.Tests/ # 应用服务测试
docs/
  superpowers/specs/            # 设计规格文档
  superpowers/plans/            # 实现计划文档
```
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

## 📚 API 文档

### 游戏端点

#### 开始游戏
```http
POST /api/game/start
```

**响应示例：**
```json
{
  "status": "InProgress",
  "levelRank": "Two",
  "currentTurn": 0,
  "players": [
    {
      "seat": 0,
      "cardCount": 27,
      "isCurrentTurn": true,
      "cards": [
        {"id": "D1-Clubs-4", "text": "梅花4"},
        // ... 更多牌
      ]
    }
    // ... 其他玩家
  ]
}
```

#### 出牌
```http
POST /api/game/play
Content-Type: application/json

{
  "playerSeat": 0,
  "cardIds": ["D1-Clubs-4", "D1-Clubs-5"]
}
```

#### 过牌
```http
POST /api/game/pass
Content-Type: application/json

{
  "playerSeat": 1
}
```

#### 查询游戏状态
```http
GET /api/game/state
```

#### 重新开始
```http
POST /api/game/restart
```

## 🏗️ 架构设计

### 四层架构
1. **Domain层**：核心业务对象（牌、玩家、游戏状态）
2. **Rules层**：纯规则引擎（牌型识别、合法性验证）
3. **Application层**：用例编排（开始游戏、出牌、过牌）
4. **Web层**：API端点和Web界面

### 设计原则
- **关注点分离**：每层有明确的职责
- **可测试性**：规则引擎可独立测试
- **可扩展性**：支持后续添加进贡、多局升级
- **规范驱动**：先有设计规格，后有代码实现

## 🤝 贡献

欢迎贡献！请参阅 [CONTRIBUTING.md](CONTRIBUTING.md) 了解如何参与开发。

### 开发流程
1. 阅读设计规格文档
2. 遵循实现计划
3. 编写测试
4. 提交Pull Request

## 📄 许可证

本项目基于 MIT 许可证开源。详见 [LICENSE](LICENSE) 文件。

## 🙏 致谢

- 项目采用规范驱动开发（Spec-Driven Development）
- 使用Codex生成，Claw作为监督者
- 感谢所有扑克牌游戏爱好者的反馈

## 📞 联系

- GitHub: [qingshiqianmo](https://github.com/qingshiqianmo)
- 项目地址: https://github.com/qingshiqianmo/guandan-game

---

## 说明

1. 这是惯蛋，不是 UNO。
2. 当前优先保证规则引擎和本地原型可运行。
3. 后续若扩展进贡、多局升级，应继续遵守现有四层边界。
4. 项目采用规范驱动开发，确保代码质量。
