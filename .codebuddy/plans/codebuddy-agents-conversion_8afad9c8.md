---
name: codebuddy-agents-conversion
overview: 将 .codebuddy/agents/ 目录下的 5 个 Unity Agent 文件转换为 CodeBuddy 语法格式，添加必要的 YAML 头部配置
todos:
  - id: update-unity-architect
    content: 为 unity-architect.md 添加 YAML 头部配置
    status: completed
  - id: update-unity-code-reviewer
    content: 为 unity-code-reviewer.md 添加 YAML 头部配置
    status: completed
  - id: update-unity-design-reviewer
    content: 为 unity-design-reviewer.md 添加 YAML 头部配置
    status: completed
  - id: update-unity-developer
    content: 为 unity-developer.md 添加 YAML 头部配置
    status: completed
  - id: update-unity-performance
    content: 为 unity-performance.md 添加 YAML 头部配置
    status: completed
---

## 用户需求

将 `/Users/lifan/Solitaire2/.codebuddy/agents/` 目录下的 5 个 Unity Agent 文件修改为符合 CodeBuddy 语法格式。

## 当前问题

这些 Agent 文件目前只有纯 Markdown 内容，缺少 CodeBuddy 要求的 YAML 头部配置（Front Matter）。

## 目标

为每个 Agent 文件添加标准的 YAML 头部，包含：

- name: Agent 标识名
- description: 角色描述
- model: 使用的模型
- tools: 可用工具列表
- agentMode: agentic 或 chat
- enabled: 是否启用
- enabledAutoRun: 是否自动运行

## 需要修改的文件

1. unity-architect.md - Unity架构设计代理
2. unity-code-reviewer.md - Unity代码审查代理
3. unity-design-reviewer.md - Unity设计审查代理
4. unity-developer.md - Unity开发代理
5. unity-performance.md - Unity性能优化代理

## 技术方案

### CodeBuddy Agent 文件格式规范

CodeBuddy 的 Agent 文件采用 YAML Front Matter + Markdown 内容的格式：

```
---
name: agent-name
description: 角色描述
model: auto
tools: tool1, tool2, tool3
agentMode: agentic
enabled: true
enabledAutoRun: true
---
# Markdown 内容
```

### 工具选择

根据每个 Agent 的职责，分配适当的工具：

**通用工具**（所有 Agent）：

- `list_dir`, `search_file`, `search_content`, `read_file` - 代码探索
- `read_lints` - 代码检查
- `replace_in_file`, `write_to_file` - 文件修改

**开发相关工具**（unity-developer）：

- `execute_command` - 执行命令
- `create_rule` - 创建规则
- `delete_file` - 删除文件

**高级工具**（unity-architect, unity-developer）：

- `preview_url` - 预览
- `web_fetch`, `web_search` - 网络搜索
- `automation_update` - 自动化更新

### 每个 Agent 的配置

| Agent | 核心职责 | 特殊工具 |
| --- | --- | --- |
| unity-architect | 架构设计、重构规划 | web_search, web_fetch, automation_update |
| unity-design-reviewer | 设计审查、质询 | 基础工具 |
| unity-developer | 代码实现、功能开发 | execute_command, create_rule, delete_file, use_skill |
| unity-code-reviewer | 代码质量审查 | 基础工具 |
| unity-performance | 性能分析优化 | 基础工具 |


### 统一配置值

- model: auto（自动选择模型）
- agentMode: agentic（代理模式）
- enabled: true（启用）
- enabledAutoRun: true（自动运行）