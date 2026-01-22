# D+ Syntax

## Label

**Label** and **Statement** are the cornerstones of D+ scripts. Essentially almost each line is a statement, and all statements must belong to a label.

Labels are not mandatory, but they help to make the script structure clearer and facilitate story creation. Labels are crucial for flow control; see the [Jump & Tour Statement](#jump--tour-statement) section for details.

A label can be declared like this:

```dp
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

In addition, you may noticed **"Top level statement"**. These statements are not indented and do not explicitly belong to any label. However, they will be automatically assigned to the label named "`@system/\_\_main\_\_`" during compilation. **This label is the default entry point when the script is executed. Therefore, every script that needs to be executed needs top level statements, usually started with a jump statement.**

## Import

```dp
import other_script_1.dp
import ../other_script_2.dp
```

The keyword **import** is used to import other scripts, allowing labels from the imported script to be used in the current script.

Note:

- Import must be used at the top of the script.

- Import allows absolute paths as well as relative paths rooted from the current script's path.

## Dialogue Statement

Dialogue statements are the most commonly used statements, used to write dialogue or narration. Usage is as follows:

```dp
"After defeating the terrifying dragon, the knight rescued prisoned princess."
Knight "Your Highness, I have finally found you. Will you marry me?"
Princess "I do!"
```

## Menu Statement

## Jump & Tour Statement

## Call Statement

## Assign Statement

## If Statement

## Ohters
