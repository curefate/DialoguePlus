# D+ Syntax

## Label

**Label** and **Statement** are the cornerstones of D+ scripts. Essentially almost each line is a statement, and all statements must belong to a label.

Labels are not mandatory, but they help to make the script structure clearer and facilitate story creation. Labels are crucial for flow control; see the [Jump & Tour Statement](#jump--tour-statement) section for details.

A label can be declared like this:

```python
jump label_a             # Top level statement
label label_a:           # Declare label "label_a"
    "Hello World!"       # Statements under the label "label_a"
"Hello World!"           # Top level statement, do not belong to "label_a"
```

Note:

- The definition of a label must occur at the top level.
- Label definitions must end with a colon (:).
- The naming rules for labels are similar to variables; for example, they cannot start with a number.
- Statements within the label block must be indented like line 3.

In addition, you may noticed **"Top level statement"**. These statements are not indented and do not explicitly belong to any label. However, they will be automatically assigned to the label named "`@system/__main__`" during compilation. **This label is the default entry point when the script is executed. Therefore, every script that needs to be executed needs top level statements, usually started with a jump statement.**

## Import

```python
import other_script_1.dp
import ../other_script_2.dp
```

The keyword **import** is used to import other scripts, allowing labels from the imported script to be used in the current script.

Note:

- Import must be used at the top of the script.
- Import allows absolute paths as well as relative paths rooted from the current script's path.

## Dialogue Statement

Dialogue statements are the most commonly used statements, used to write dialogue or narration. Usage is as follows:

```python
# Narration
"After defeating the terrifying dragon, the knight rescued prisoned princess."

# Name + Dialogue
Knight "Your Highness, I have finally found you.\nWill you marry me?"
Princess "I will!"
```

## Menu Statement

Menu statements are used to create option menus.

```python
"How are you?"
"I'm good!":
    jump good
"Sad...":
    jump sad
```

Note:

- Add a colon (:) after narration dialogue statement to create a menu option.
- Indentation is necessary before the sub-block of the options.
- No limit number of options.

## Jump & Tour Statement

Jump and tour statements are used for flow control. To use them, simply add a label name after the `jump` or `tour` keyword.

The difference is that `jump` clears the execution queue and jumps to the target label, while `tour` returns to the current position after executing the target label. For example:

```python
# Jump example
Bob "Hello!"              # stmt 1
jump a                    # stmt 2
Bob "Nice to Meet you!"   # stmt 3, this dialogue statment will not be executed

label a:
    Bob "My name is Bob." # stmt 4

# Execution order: stmt 1->2->4
```

```python
# Tour example
Bob "Hello!"              # stmt 1
tour a                    # stmt 2
Bob "Nice to Meet you!"   # stmt 3

label a:
    Bob "My name is Bob." # stmt 4

# Execution order: stmt 1->2->4->3
```

## Call Statement

Call statements are used to invoke functions, which need to be registered in the host system (the game engine you are using). There are two ways to register:

1. `Runtime.Functions.AddFunction<>()`, see [details]().
2. `[DPFunction]` attribute, see [details](), only for Unity currently. Note that for instance methods, registering in this way will add an extra string parameter as the **first argument**, used to pass in the instance name..

```c#
// In Unity
public static int Add(int a, int b) ...;
void Start()
{
    // Register through runtime
    UnitiAdapter.Instance.Runtime.Functions.AddFunction<int, int, int>(Add);
}
```

```c#
// In Unity
public class Shaker : Monobehaviour
{
    [DPFunction] // Register through attr
    public void Shake(float duration) ...;
}
```

```python
# In script
"1 + 1 = {call Add(1, 1)}"
call Shake("shaker_name" ,0.5) # shaker_name should be the object name of the instance
```

Note:

- To use a function's return value, enclose the call statement in braces({}).
- Only **string**, **int**, **float**, and **bool** are supported as parameter and return value types.

## Assign Statement

D+ provides a variable system and allows you to construct expressions using variables. Use "$" to access variables.

Supported variable types include **int**, **float**, **string**, and **bool**.

```bash
$var_str = "string"
$var_int = 1
$var_int += 99
$var_float = 99.9
$var_bool = $var_int >= $var_float # =true
```

Note that variables cannot be used across labels unless declared using `$global.`. Global variables are persisted throughout a single execution.

In addition, use braces to wrap variables in a string.

```bash
$age = 18
$global.var = 18

label any:
    Bob "I am {$age} years old."         # Error, $age is a temporary variable
    Bob "I am {$global.var} years old."
```

To know more about variable system, see [details]().

## If Statement

If statements are also used for flow control; note that you need to add indentation before the sub-block.

```python
if socre >= 100:
    jump very_good
elif socre >=60:
    jump pass_exam
else:
    "Unfortunately, you failed."
```

## Ohters

- Use `"#"` to add comments.

- Konwn issue: Escape symbol "{{" do not working.
