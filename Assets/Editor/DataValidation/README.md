# 数据验证工具

## 目标

- 将主链路数据错误前移到编辑器阶段
- 减少运行时冗长校验代码
- 不向运行时程序集引入验证依赖

## 入口

- 全量校验：`Tools/数据校验/校验全部主链路数据`
- 轻量校验：`Assets/Data` 下资产导入或修改后自动触发

## 结构

- `DataValidationTool.cs`：工具入口与结果输出
- `IDataValidationSelfCheck`：资产自校验接口

## 代码结构说明

```text
DataValidationTool
  -> 统一扫描 Assets/Data 下的 ScriptableObject 资产
  -> 调用实现 IDataValidationSelfCheck 的资产
  -> 汇总并输出 Console 日志

各资产类
  -> 在 ValidateSelf 中声明自身本地规则
```

说明：

- 本地规则放在资产类里，规则离字段最近，便于维护
- 工具层只负责扫描、调度、汇总，不承担业务规则

## 当前规则

- 资产自校验：各资产类实现 `ValidateSelf`
