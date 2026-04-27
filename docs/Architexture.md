# DialoguePlus

## Overview

DialoguePlus (D+) is a scripting language project used to create game story scripts.

The core goal of D+ is to separate the responsibilities of programmers and scriptwriters, with the former focusing on building the game system and the latter on designing the narrative. Which provides an easily expandable framework and a concise syntax to reduce the learning curve.

D+ is a compile-then-execute scripting language based on C#, independent of any engine. The executor can be rewritten to suit different game genres.

## High-level Modules

### Compiler

The compiler's responsibility is to compile the `.dp` script into `LabelSet`, which is the data structure accepted by the executor. The compiler contains several modules, including Lexer, Parser, and SymbolTables.

Following figure shows the compilation process of D+ scripts.

![dataflow](../images/dataflow.drawio.svg)

---

### SIR & LbaelSet

`SIR` and `LabelSet` are defined in `Instruction.cs`.

**SIR** stands for **"Structured Intermediate Representation"** (because SIRs use nested structure rather than linear execution structure). Simply put, an SIR is the smallest unit of execution by the Executer; each SIR is an instruction that can be executed by the Executer.

**LabelSet** is the collection of SIR which divided using Label. It is also a parameter in the execution API exposed by Executer. The Executer uses `LabelSet.EntranceLabel` to find the entry point for execution. This property is currently set to `"@system/__main__"`, i.e. label includes SIRs that build by top-level statements.

---

### Executer

The executor's role is to execute SIRs. Executor's structure is simple, consisting of only a `Runtime` and an execution queue. The Runtime provides the executor's state during execution, and the execution queue controls the execution order.

The executor exposes two delegates: `OnDialogueAsync` and `OnMenuAsync`. By default, these two delegates point to the default methods that run in the console, but in game engines, developers need to make these two delegates point to their own implemented methods to adapt to different game genres or UI interaction designs.

---

### Runtime

In short, Runtime is the state of the executor during execution. It contains a `VariableRegistry` and a `FunctionRegistry`.

**VariableRegistry** stores variables generated during execution. Variables are divided into local variables and global variables, with global variables starting with "`global.`". Local variables are discarded after the current label is left during execution, while global variables are preserved indefinitely.

**FunctionRegistry** stores functions registered from the host system (i.e., the game engine). When a call statement is used in script, the function is looked up in the FunctionRegistry and called based on its name. Use the `FunctionRegistry.AddFunction<>()` function to register.

The pairing between runtime and executer is not fixed; different runtimes can be switched for executer if needed.
