
# DialoguePlus (D+)

![logo](images/icon_small.png)

**DialoguePlus (D+)** is a narrative scripting language and runtime framework designed for interactive story experience.
It provides a **scripting language** combined with a **compiler + executor architecture**, enabling both straightforward authoring and deep runtime customization.

D+ is engine-agnostic at its core, allow host systems to override execution behavior and integrate seamlessly with different game genres.

## Navigation

- [**D+ Syntax Reference**](docs/Syntax.md)  

- [**API Reference**](docs/API.md)

- [**Core Library Architecture**](docs/Architexture.md)

- [**Unity Adapter Documentation**](docs/UnityAdapter.md)

- [**VS Code Extension**](https://github.com/curefate/DialoguePlus-Extension)

## Unity Example

![show](images/show.gif)

> This example is from sample scene of Unity Adapter.
> PS: Yeah I can see it looks... simple, but just for now :3

## Features

- **Python-like syntax:**  
  Clean, easy, zero ambiguity.
- **Customizable:**  
  Execution behaviour can be overriden by host system, suitable for most game genres.
- **Modular and Extensible:**  
  Core library is engine-agnostic.

## Quick Start

### 1. Write a Script

Create a `.dp` file with your story:

```python
Bob "Hello! Welcome to DialoguePlus."
Bob "What's your favorite color?"
"Red":
    Bob "Red is vibrant!"
"Blue":
    Bob "Blue is calming."
Bob "Thanks for trying D+!"
```

### 2. Compile and Execute

**In Unity:**

```csharp
await DialoguePlusAdapter.Instance.ExecuteToEnd("path/to/script.dp");
```

**In C# (Console):**

```csharp
var compiler = new Compiler();
var result = compiler.Compile("path/to/script.dp");

if (result.Success)
{
    var executer = new Executer();
    executer.Prepare(result.Labels);
    await executer.AutoStepAsync(0); // Run to completion
}
```

### 3. Learn More

- **Unity Users**: See [Unity Adapter Documentation](docs/UnityAdapter.md)
- **Scripting**: See [Syntax Reference](docs/Syntax.md)
- **Integration**: See [API Reference](docs/API.md)

## Repository Structure

- **Core Library:** src/

- **Unity Adapter:** adapter/Unity

## License

[MIT License](LICENSE)
