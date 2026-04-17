# AGENTS.md – MvvmControls

This document provides guidance for AI agents and contributors working on the **RFBCodeWorks.MvvmControls** repository.

---

## Repository Overview

MvvmControls is a WPF MVVM helper library that consolidates boiler-plate ViewModel interactions for common WPF controls into pre-built classes. It ships alongside a set of Roslyn source generators that reduce the amount of code a consumer needs to write.

The solution (`RFBCodeWorks.Mvvm.sln`) is composed of the following projects:

| Project | Path | Status |
|---|---|---|
| **Mvvm.Controls** | `src/Mvvm.Controls/` | Primary library – active development |
| **Mvvm.Controls.SourceGenerators** | `src/Mvvm.Controls.SourceGenerators/` | Source generators – active development |
| Mvvm.IViewModel | `src/Mvvm.IViewModel/` | Interface-only library – largely complete |
| Mvvm.Dialogs | `src/Mvvm.Dialogs/` | Dialog helpers – largely complete |
| Mvvm.WebView2Integration | `src/Mvvm.WebView2Integration/` | WebView2 helpers – largely complete |
| ExampleWPF | `src/ExampleWPF/` | Demo application – reference only |
| MvvmControlsTests | `tests/MvvmControlsTests/` | Test project |

> **Focus your work on `Mvvm.Controls` and `Mvvm.Controls.SourceGenerators`**. The other libraries are considered feature-complete and should not need significant changes.

---

## Build & Test

### Prerequisites
- .NET SDK 8.0 and 10.0 (both required — CI installs both via `actions/setup-dotnet`)
- Windows Desktop runtime (WPF). Tests **cannot** run on Linux/macOS.

### Build
```shell
dotnet build -p:EnableWindowsTargeting=true
```
`EnableWindowsTargeting=true` is required when building on non-Windows hosts (e.g. Linux CI agents running without the Windows Desktop workload).

### Run Tests
```shell
dotnet test
```
Tests target `net48`, `net8.0-windows`, and `net10.0-windows`. Each TFM exercises a different Roslyn version for the source generators.

### Pack NuGet Packages
```shell
dotnet pack ./src/Mvvm.Controls/Mvvm.Controls.csproj --configuration Release
```

### CI
The GitHub Actions workflow (`.github/workflows/build-and-test.yml`) runs on `windows-latest`, restores, builds in `Release`, runs all tests, and (on pushes to `master`) packs and uploads NuGet artifacts.

---

## Project Structure

### Mvvm.Controls (`src/Mvvm.Controls/`)

Target frameworks: `net462`, `net48`, `net8.0-windows`, `net10.0-windows`.

Key namespaces and directories:

```
src/Mvvm.Controls/
├── Mvvm/
│   ├── Attributes.cs           – All source-generator attributes ([Button], [Selector], [ComboBox], etc.)
│   ├── Primitives/             – Abstract base classes (ControlBase, CommandBase, SelectorDefinition, …)
│   ├── Specialized/            – Concrete specializations (DateTimePicker, TreeView items, TwoStateButton, …)
│   ├── ComponentModel/         – Additional observable/component model helpers
│   ├── DragAndDrop/            – Drag-and-drop helpers
│   ├── EventArgs/              – Custom EventArgs types
│   ├── Helpers/                – Utility helpers
│   ├── Input/                  – ICommand implementations (RelayCommand, AsyncRelayCommand, …)
│   ├── XmlLinq/                – XDocument/XElement TreeView view-models
│   └── ViewModelBase.cs        – Abstract base ViewModel (extends ObservableObject, implements IDisposable + IViewModel)
├── WPF/
│   ├── Behaviors/              – Attached behaviors (ControlDefinitions binding, MultiItemSelection, …)
│   ├── Controls/               – Custom WPF controls (NumericUpDown, IPv4 textbox, …)
│   └── Converters/             – WPF value converters
└── Themes/                     – Resource dictionaries / control templates
```

**Class hierarchy (primitives → concrete)**

```
ObservableObject (CommunityToolkit.Mvvm)
└── ControlBase           – IsEnabled, IsVisible, Visibility
    └── ItemSource<T,E>   – Items, DisplayMemberPath, ItemSourceChanged
        └── SelectorDefinition<T,E,V> – SelectedItem, SelectedValue, SelectedValuePath
```

Command hierarchy:
```
AbstractCommand / AbstractCommand<T>
AbstractAsyncCommand / AbstractAsyncCommand<T>
AbstractButtonDefinition / AbstractButtonDefinition<T>        (also ICommand)
AbstractAsyncButtonDefinition / AbstractAsyncButtonDefinition<T>
  └── ButtonDefinition / AsyncButtonDefinition (sealed concrete)
      RelayCommand / AsyncRelayCommand (sealed, thin wrappers)
```

### Mvvm.Controls.SourceGenerators (`src/Mvvm.Controls.SourceGenerators/`)

The source generator code is compiled three times against different Roslyn SDK versions:

| Project file | Roslyn version | Used by VS/compiler |
|---|---|---|
| `SourceGenerators.Roslyn311.csproj` | 3.11 | VS2019, .NET Framework / older toolchains |
| `SourceGenerators.Roslyn410.csproj` | 4.10 | VS2022 (current) |
| `SourceGenerators.Roslyn500.csproj` | 5.0 | VS2026 / .NET 10 toolchain |

All three projects share the same source files under `src/`. Version-specific files use the naming convention `*Roslyn311.cs` / `*Roslyn3*.cs` (excluded from Roslyn4+ builds).

**Source generator structure** (one pattern repeated for each generator):

```
src/
├── ButtonGenerator/
│   ├── ButtonAttributeData.cs   – Readonly struct holding parsed attribute data
│   ├── ButtonParser.cs          – Roslyn syntax/semantic parsing
│   ├── ButtonEmitter.cs         – Source code emission (SourceWriter calls)
│   ├── _ButtonGenerator.Roslyn311.cs  – ISourceGenerator entry point (legacy)
│   └── _ButtonGenerator.Roslyn40.cs  – IIncrementalGenerator entry point (preferred)
├── Refreshable/                 – [Selector] / [ComboBox] / [ListBox] generators
├── TriggersRefresh/             – [TriggersRefresh] generator
└── IViewModelGenerator/         – [IViewModel] generator
```

Each generator follows the **Parse → Data → Emit** pattern:
1. **Parser** – `NodeSelector` filters candidate syntax nodes; `GetInfoOrDiagnostic` transforms them into an immutable data struct.
2. **Data struct** – An `IEquatable` readonly struct or record that holds all information needed for code generation (no Roslyn symbols at emit time).
3. **Emitter** – Produces source text strings via `SourceWriter`.

`DataOrDiagnostics<T>` is the return type for parse results that may produce diagnostics instead of data.

**Diagnostic IDs** (defined in `Diagnostics.cs`):

| ID | Meaning |
|---|---|
| `RFB_MVVM_000` | Unhandled generator exception |
| `RFB_MVVM_001` | Unknown target framework |
| `RFB_MVVM_002` | Unsupported language (non-C#) |
| `RFB_MVVM_003` | C# language version too low |
| `RFB_MVVM_004` | Containing class is not `partial` |
| `RFB_MVVM_005` | Invalid method signature for `[Button]` |
| `RFB_MVVM_006` | Return type does not implement `IList<T>` (for selector generators) |
| `RFB_MVVM_007` | Unable to generate a fully-qualified name |
| `RFB_MVVM_008` | Unable to determine collection type |
| `RFB_MVVM_009` | `[TriggersRefresh]` used without `[ObservableProperty]` |
| `RFB_MVVM_010` | `[ObservableProperty]` backing field is not `private` |
| `RFB_MVVM_011` | Too many parameters on refreshable method |
| `RFB_MVVM_012` | Async refreshable parameter must be `CancellationToken` or none |
| `RFB_MVVM_013` | Class does not implement required interface |

---

## Source-Generator Attributes (Consumer API)

All attributes live in the `RFBCodeWorks.Mvvm` namespace and are defined in `src/Mvvm.Controls/Mvvm/Attributes.cs`.

| Attribute | Target | Generates |
|---|---|---|
| `[IViewModel]` | Class | Adds `IViewModel` interface implementation |
| `[Button]` | Method | `ButtonDefinition` or `AsyncButtonDefinition` property |
| `[Selector]` | Method returning `IList<T>` | `RefreshableSelector<T,TList,TValue>` property |
| `[ComboBox]` | Method returning `IList<T>` | `RefreshableComboBoxDefinition<T,TValue>` property |
| `[ListBox]` | Method returning `IList<T>` | `RefreshableListBoxDefinition<T,TValue>` property |
| `[OnSelectionChanged]` | Method (with selector attr) | Hook to fire when `SelectedItem` changes |
| `[OnCollectionChanged]` | Method (with selector attr) | Hook to fire when `Items` changes |
| `[TriggersRefresh]` | Method or `[ObservableProperty]` field | Refreshes named selectors on change |

**Generated property naming convention**: Strip leading/trailing `_`, strip `On`/`Get`/`Refresh` prefixes, strip `Async`/`Command`/`Func` suffixes, then append the type suffix (e.g. `Button`, `Selector`, `ComboBox`, `ListBox`). The class containing these methods **must be `partial`**.

---

## Tests (`tests/MvvmControlsTests/`)

The test project uses **MSTest** (`MSTest.TestAdapter` + `MSTest.TestFramework`).

Sub-directories:
- `Mvvm.SourceGenerators.Tests/` – Source generator unit tests. Each test compiles a small C# input file embedded as a resource (under `GeneratorInputs/`) and asserts on the generated output.
- `Mvvm.Tests/` – Runtime tests for Mvvm.Controls classes.
- `WPF.Behaviors.Tests/` – Tests for WPF attached behaviors.
- `WPF.Controls.Tests/` – Tests for custom WPF controls.

When adding or modifying a source generator, add or update the corresponding generator input file in `Mvvm.SourceGenerators.Tests/GeneratorInputs/` and add it as an `<EmbeddedResource>` in `Mvvm.Controls.Tests.csproj`. Then add or update the test in the appropriate `*GeneratorTests.cs` file.

---

## Key Conventions

- **Namespace root**: `RFBCodeWorks` (assembly name prefix) / `RFBCodeWorks.Mvvm` (runtime namespace).
- **C# language version**: `Latest` in `Mvvm.Controls`. The `CSharp9_MissingComponents.cs` shim is included for `init`-setter support on older targets.
- **Nullability**: `#nullable enable` is used in new source files.
- **`partial` requirement**: Any class using source-generator attributes must be declared `partial`. The generator emits a `RFB_MVVM_004` error diagnostic otherwise.
- **Async selectors**: Methods decorated with `[Selector]`/`[ComboBox]`/`[ListBox]` may return `IList<T>`, `Task<IList<T>>`, or `ValueTask<IList<T>>`. Async methods may optionally accept a single `CancellationToken` parameter.
- **Strong naming**: The test assembly is strong-named using `MvvmControls.snk`.
- **XAML namespaces** (for use in consumer XAML):
  - Converters: `https://github.com/RFBCodeWorks/MvvmControls/WPF.Converters`
  - Controls: `https://github.com/RFBCodeWorks/MvvmControls/WPF.Controls`
  - Behaviors: `https://github.com/RFBCodeWorks/MvvmControls/WPF.Behaviors`
  - Specialized ViewModels: `https://github.com/RFBCodeWorks/MvvmControls/Mvvm/Specialized`

---

## Filing Issues

Use the GitHub issue templates:
- **Bug Report** – general bugs (`.github/ISSUE_TEMPLATE/bug-report.md`)
- **Source Generator Issue** – generator bugs including the input code, what was generated, and what was expected (`.github/ISSUE_TEMPLATE/source-generator-issue.md`)
