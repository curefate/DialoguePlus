
# DialoguePlus (D+)

![logo](images/icon_small.png)

**DialoguePlus (D+)** is a narrative scripting language and runtime framework designed for interactive story experience.
It provides a **scripting language** combined with a **compiler + executor architecture**, enabling both straightforward authoring and deep runtime customization.

D+ is engine-agnostic at its core, allow host systems to override execution behavior and integrate seamlessly with different game genres.

## Navigation

- [**D+ Grammar Reference**](docs/Grammar.md)  

- [**Unity Adapter Documentation**](docs/UnityAdapter.md)

- [**Core Library Architecture**](docs/Architexture.md)

- [**VS Code Extension**](https://github.com/curefate/DialoguePlus-Extension)

## Unity Example

![show](images/show.gif)

> This example is from sample scene of Unity Adapter.

## Features

- **Python-like grammar:** Clean, easy, unambiguous.
- **Customizable:** Execution behaviour can be overriden by host system, suitable for most game genres.
- **Modular and Extensible:** Core library is engine-agnostic.

## Repository Structure

- **Core Library**  
  The D+ compiler, runtime executor, and core data structures

- **Unity Adapter**  
  Unity-specific integration layer and example project

- **VS Code Extension**  
  Editor support for D+ scripting

## License

[MIT License](LICENSE)
