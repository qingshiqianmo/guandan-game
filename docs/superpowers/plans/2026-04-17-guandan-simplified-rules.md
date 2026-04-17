# 惯蛋简化完整规则 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 交付一个采用 Domain/Rules/Application/Web 四层架构的本地惯蛋原型，支持单局完整流程与简化完整规则。

**Architecture:** 以 `GameState` 为核心聚合，Rules 层提供纯规则判定，Application 层编排命令与状态流转，Web 层通过 Minimal API 和静态页面驱动本地对局。测试按层分离，优先保证 Rules 与 Application 可回归。

**Tech Stack:** .NET 9、ASP.NET Core Minimal API、xUnit

---

### Task 1: 初始化解决方案与基础工程

**Files:**
- Create: `guandan-game.sln`
- Create: `src/Guandan.Game.Domain/`
- Create: `src/Guandan.Game.Rules/`
- Create: `src/Guandan.Game.Application/`
- Create: `src/Guandan.Game.Web/`
- Create: `tests/Guandan.Game.Domain.Tests/`
- Create: `tests/Guandan.Game.Rules.Tests/`
- Create: `tests/Guandan.Game.Application.Tests/`
- Create: `.editorconfig`
- Create: `Directory.Build.props`

- [ ] 创建解决方案与 4 个主项目、3 个测试项目
- [ ] 为所有项目启用 `<Nullable>enable</Nullable>` 和隐式全局 using
- [ ] 建立项目引用：`Rules -> Domain`，`Application -> Domain + Rules`，`Web -> Application + Domain + Rules`
- [ ] 建立测试项目引用到各自目标项目
- [ ] 运行 `dotnet build`，确认空骨架可编译

### Task 2: 实现 Domain 基础模型

**Files:**
- Create: `src/Guandan.Game.Domain/Cards/card_suit.cs`
- Create: `src/Guandan.Game.Domain/Cards/card_rank.cs`
- Create: `src/Guandan.Game.Domain/Cards/card.cs`
- Create: `src/Guandan.Game.Domain/Players/player_seat.cs`
- Create: `src/Guandan.Game.Domain/Players/player_hand.cs`
- Create: `src/Guandan.Game.Domain/Game/game_status.cs`
- Create: `src/Guandan.Game.Domain/Game/played_combo.cs`
- Create: `src/Guandan.Game.Domain/Game/round_state.cs`
- Create: `src/Guandan.Game.Domain/Game/game_state.cs`
- Create: `src/Guandan.Game.Domain/Game/player_finish_record.cs`
- Create: `src/Guandan.Game.Domain/Game/settlement_result.cs`

- [ ] 定义牌、花色、牌点、玩家座位和值对象
- [ ] 定义 `GameState` 聚合，包含玩家手牌、当前回合、桌面主牌、过牌计数、已完成名次
- [ ] 提供受控状态变更方法，避免 Web 直接改字段
- [ ] 编写 Domain 单元测试验证发牌后手牌数量、出牌移除、名次记录

### Task 3: 实现牌堆与发牌能力

**Files:**
- Create: `src/Guandan.Game.Domain/Cards/deck_factory.cs`
- Create: `src/Guandan.Game.Application/Abstractions/ishuffle_service.cs`
- Create: `src/Guandan.Game.Application/Services/default_shuffle_service.cs`
- Modify: `src/Guandan.Game.Domain/Game/game_state.cs`
- Test: `tests/Guandan.Game.Domain.Tests/Cards/deck_factory_tests.cs`
- Test: `tests/Guandan.Game.Application.Tests/Services/default_shuffle_service_tests.cs`

- [ ] 生成双副牌 108 张且牌张分布正确
- [ ] 实现可注入洗牌服务，方便测试使用固定顺序
- [ ] 将发牌逻辑保持在 Application 层，Domain 只接收已分配手牌
- [ ] 编写测试验证总牌数、大小王数量、每家 27 张

### Task 4: 实现 Rules 牌力顺序与通配规则

**Files:**
- Create: `src/Guandan.Game.Rules/Ranking/game_rank_context.cs`
- Create: `src/Guandan.Game.Rules/Ranking/card_ordering_service.cs`
- Create: `src/Guandan.Game.Rules/Wildcards/wildcard_expansion_service.cs`
- Create: `src/Guandan.Game.Rules/Wildcards/wildcard_assignment.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Ranking/card_ordering_service_tests.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Wildcards/wildcard_expansion_service_tests.cs`

- [ ] 定义局级上下文，支持当前级牌与红桃配牌识别
- [ ] 实现牌点大小比较与排序规则
- [ ] 实现红桃级牌作为通配牌的替换枚举
- [ ] 用测试覆盖非通配、单通配、多通配和不能替代王的场景

### Task 5: 实现牌型识别

**Files:**
- Create: `src/Guandan.Game.Rules/Patterns/pattern_type.cs`
- Create: `src/Guandan.Game.Rules/Patterns/pattern_descriptor.cs`
- Create: `src/Guandan.Game.Rules/Patterns/pattern_recognizer.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Patterns/pattern_recognizer_tests.cs`

- [ ] 支持单张、对子、三张、三带二、顺子、连对、钢板、炸弹、同花顺、王炸
- [ ] 将“识别”和“比较”分离，识别输出统一 `PatternDescriptor`
- [ ] 用参数化测试覆盖全部牌型与非法组合
- [ ] 覆盖通配牌参与成型的关键用例

### Task 6: 实现牌型比较与合法性校验

**Files:**
- Create: `src/Guandan.Game.Rules/Validation/play_validation_result.cs`
- Create: `src/Guandan.Game.Rules/Validation/play_comparison_service.cs`
- Create: `src/Guandan.Game.Rules/Validation/play_validation_service.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Validation/play_comparison_service_tests.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Validation/play_validation_service_tests.cs`

- [ ] 实现同型比较规则
- [ ] 实现炸弹、同花顺、王炸的跨类型压制关系
- [ ] 实现“必须轮到当前玩家”“手牌必须真实持有”“首轮不可过牌”等校验
- [ ] 测试覆盖压不过、越权出牌、牌不在手、首轮过牌失败

### Task 7: 实现轮次推进与结算规则

**Files:**
- Create: `src/Guandan.Game.Rules/Rounds/round_flow_rule.cs`
- Create: `src/Guandan.Game.Rules/Settlement/settlement_rule.cs`
- Modify: `src/Guandan.Game.Domain/Game/game_state.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Rounds/round_flow_rule_tests.cs`
- Test: `tests/Guandan.Game.Rules.Tests/Settlement/settlement_rule_tests.cs`

- [ ] 实现出牌成功后的回合推进
- [ ] 实现过牌累积与三家连续不跟后的新轮重置
- [ ] 实现玩家出完后的名次追加
- [ ] 实现同队两人都出完即结束，并输出胜方与双下标志

### Task 8: 实现 Application 用例层

**Files:**
- Create: `src/Guandan.Game.Application/Commands/start_game_command.cs`
- Create: `src/Guandan.Game.Application/Commands/play_cards_command.cs`
- Create: `src/Guandan.Game.Application/Commands/pass_turn_command.cs`
- Create: `src/Guandan.Game.Application/Dtos/game_view.cs`
- Create: `src/Guandan.Game.Application/Dtos/player_view.cs`
- Create: `src/Guandan.Game.Application/Dtos/action_result.cs`
- Create: `src/Guandan.Game.Application/Services/game_application_service.cs`
- Create: `src/Guandan.Game.Application/Abstractions/igame_session_store.cs`
- Create: `src/Guandan.Game.Application/Services/in_memory_game_session_store.cs`
- Test: `tests/Guandan.Game.Application.Tests/Services/game_application_service_tests.cs`

- [ ] 实现开局、出牌、过牌、查询状态、重开用例
- [ ] 使用内存态 `GameSessionStore` 保存当前本地对局
- [ ] 把 Domain/Rules 结果映射成 Web 友好的 DTO
- [ ] 编写流程测试，覆盖开局到结束的完整链路

### Task 9: 实现 Web API 与本地页面

**Files:**
- Create: `src/Guandan.Game.Web/Program.cs`
- Create: `src/Guandan.Game.Web/Endpoints/game_endpoints.cs`
- Create: `src/Guandan.Game.Web/wwwroot/index.html`
- Create: `src/Guandan.Game.Web/wwwroot/app.css`
- Create: `src/Guandan.Game.Web/wwwroot/app.js`
- Test: `tests/Guandan.Game.Application.Tests/Web/api_contract_smoke_tests.cs`

- [ ] 暴露 `start / state / play / pass / restart` 接口
- [ ] 首页展示四家手牌数、当前玩家、桌面牌、操作日志
- [ ] 支持点选当前玩家手牌并提交出牌或过牌
- [ ] 保持 UI 简单，不把规则逻辑放进前端

### Task 10: 工程收尾与验证

**Files:**
- Modify: `README.md`
- Modify: `.editorconfig`
- Modify: `Directory.Build.props`

- [ ] 更新 `README.md`，补充运行方式、项目结构和当前规则范围
- [ ] 运行 `dotnet test`
- [ ] 运行 `dotnet format`
- [ ] 再次运行 `dotnet build` 与 `dotnet test`，确认格式化未引入回归

## 计划自检

- 规格覆盖：已覆盖四层架构、简化完整规则、核心流程、错误处理、测试策略
- 占位检查：无 `TODO`、`TBD`、模糊描述占位
- 一致性检查：所有任务围绕单局本地原型，不包含超范围的联网或进贡流程
