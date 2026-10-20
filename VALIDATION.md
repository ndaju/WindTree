# Validation

| Test | Execution method | Result | Evidence or observations | Limitations |
|---|---|---|---|---|
| Build | `dotnet build WindTree.slnx` | Passed | Zero warnings and zero errors | Compilation is not live WFP verification |
| Unit tests | `dotnet test WindTree.slnx --no-build` | Passed | 3/3: executable normalization; profile/theme/rule persistence; corrupt settings recovery | Native WFP calls excluded |
| Debug UI launch | Start `src\WindTree\bin\Debug\net8.0-windows\WindTree.exe`, wait 5 seconds | Passed | Responsive `WindTree` main window handle `1182424` was created | UI actions not automated |
| Published UI launch | Publish self-contained EXE, start it, wait 8 seconds | Passed | Responsive `WindTree` main window handle `854772` was created | No code-signing |
| WFP real filtering | Elevated controlled network test | Not tested | Administrator session and local test endpoint unavailable | Do not infer enforcement from this table |
