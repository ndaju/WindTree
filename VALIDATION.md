# Validation

| Test | Execution method | Result | Evidence or observations | Limitations |
|---|---|---|---|---|
| Restore/build | `dotnet build WindTree.slnx` | Passed | Zero warnings and zero errors after WPF UI 4.3.0 integration | Compilation is not live WFP verification |
| Unit tests | `dotnet test WindTree.slnx --no-build` | Passed | 3/3: executable normalization; profile/theme/rule persistence; corrupt settings recovery | Native WFP calls excluded |
| Published EXE | `dotnet publish ... -r win-x64 --self-contained true -p:PublishSingleFile=true` | Passed | `publish\WindTree-win-x64\WindTree.exe` created | No code-signing |
| UI smoke launch | Start published `WindTree.exe`, wait 3 seconds | Passed | Process remained running before clean termination | Does not exercise dialogs or WFP changes |
| WFP real filtering | Elevated controlled network test | Not tested | Administrator session and local test endpoint unavailable | Do not infer enforcement from this table |
