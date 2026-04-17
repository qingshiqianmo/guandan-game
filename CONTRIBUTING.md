# 贡献指南

欢迎为惯蛋项目贡献代码！以下是参与贡献的指南。

## 开发环境

### 要求
- .NET 9.0 SDK 或更高版本
- Git
- 代码编辑器（推荐 Visual Studio Code 或 Visual Studio）

### 设置开发环境
1. 克隆仓库：
   ```bash
   git clone https://github.com/qingshiqianmo/guandan-game.git
   cd guandan-game
   ```

2. 恢复依赖：
   ```bash
   dotnet restore
   ```

3. 构建项目：
   ```bash
   dotnet build
   ```

4. 运行测试：
   ```bash
   dotnet test
   ```

5. 运行Web应用：
   ```bash
   cd src/Guandan.Game.Web
   dotnet run
   ```

## 项目架构

项目采用四层架构：

### 1. Domain层 (`src/Guandan.Game.Domain`)
- 核心业务对象：牌、玩家、游戏状态等
- 值对象和实体定义
- 不包含业务规则

### 2. Rules层 (`src/Guandan.Game.Rules`)
- 纯规则引擎：牌型识别、合法性验证、比较规则
- 无状态，可独立测试
- 不依赖外部服务

### 3. Application层 (`src/Guandan.Game.Application`)
- 用例编排：开始游戏、出牌、过牌等
- 协调Domain和Rules层
- 处理命令和查询

### 4. Web层 (`src/Guandan.Game.Web`)
- API端点
- 本地Web界面
- 依赖注入配置

## 开发流程

### 1. 创建分支
```bash
git checkout -b feature/your-feature-name
```

### 2. 遵循规范驱动开发
- 先更新设计规格文档 (`docs/superpowers/specs/`)
- 再更新实现计划 (`docs/superpowers/plans/`)
- 最后编写代码

### 3. 编写测试
- 每个新功能都应包含测试
- 测试按层分离：Domain、Rules、Application
- 使用参数化测试覆盖边界情况

### 4. 代码规范
- 使用C#命名约定
- 添加XML文档注释
- 保持方法简洁（不超过20行）
- 遵循单一职责原则

### 5. 提交代码
```bash
git add .
git commit -m "feat: 添加新功能描述"
git push origin feature/your-feature-name
```

### 6. 创建Pull Request
- 在GitHub上创建Pull Request
- 描述功能变更
- 链接相关Issue（如果有）
- 等待代码审查

## 测试策略

### 单元测试
- Domain层：验证核心对象行为
- Rules层：验证规则逻辑
- Application层：验证用例编排

### 集成测试
- Web层：验证API端点
- 端到端：验证完整游戏流程

### 测试运行
```bash
# 运行所有测试
dotnet test

# 运行特定项目测试
dotnet test tests/Guandan.Game.Rules.Tests
```

## 代码审查标准

### 必须满足
- 代码编译通过
- 所有测试通过
- 遵循项目架构
- 有适当的测试覆盖

### 建议满足
- 代码可读性好
- 有适当的文档注释
- 遵循SOLID原则
- 性能考虑

## 问题报告

### Bug报告
1. 描述问题现象
2. 提供重现步骤
3. 包含环境信息
4. 如果有错误日志，一并提供

### 功能请求
1. 描述需求场景
2. 说明预期行为
3. 提供参考实现（如果有）

## 许可证

通过提交代码，您同意您的贡献将在MIT许可证下发布。