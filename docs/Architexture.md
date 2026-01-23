# DialoguePlus

## Overview

DialoguePlus (D+) is a scripting language project used to create game story scripts.

The core goal of D+ is to separate the responsibilities of programmers and scriptwriters, with the former focusing on building the game system and the latter on designing the narrative. Which provides an easily expandable framework and a concise syntax to reduce the learning curve.

D+ is a compile-then-run scripting language based on C#, independent of any engine. The executor can be rewritten to suit different game genres.


```mermaid
flowchart LR

    %% =========================
    %% SOURCE SCRIPT
    %% =========================
    Script["`.dp` Script File"]

    %% =========================
    %% COMPILER (Subgraph)
    %% =========================
    subgraph Compiler["Compiler"]
        direction LR
        Lexer["Lexer"]
        Parser["Parser"]
        Diagnostic["Diagnostic System"]
    end

    %% =========================
    %% SIR ARTIFACT
    %% =========================
    SIR["SIR (Structured Intermediate Representation)"]

    %% =========================
    %% EXECUTER
    %% =========================
    Executer["Executer (Host-customizable)"]

    %% =========================
    %% RUNTIME (Subgraph)
    %% =========================
    subgraph Runtime["Runtime (Host-controllable)"]
        direction LR
        Funcs["Function Registry"]
        Vars["Variables Store"]
    end

    %% =========================
    %% HOST (Game Engine)
    %% =========================
    subgraph Host["Host System / Game Engine"]
        direction TB
        HostExec["Override / Extend Executer"]
        HostRt["Access Runtime\n(Variables / Functions)"]
        HostUI["UI / Gameplay Integration"]
    end

    %% =========================
    %% MAIN DATA FLOW
    %% =========================
    Script -->|"compile"| Compiler
    Compiler -->|"emit"| SIR
    SIR --> Executer
    Executer -->|"executes with"| Runtime

    %% =========================
    %% HOST INTERACTION (DOTTED LINKS)
    %% =========================
    HostExec -.-> Executer
    HostRt -.-> Runtime
    Executer -->|"dialogue/events"| HostUI
    HostUI -->|"player input"| Executer
    Diagnostic -. "errors / warnings" .-> HostUI
