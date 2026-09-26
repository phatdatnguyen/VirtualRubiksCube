# Virtual Rubik's Cube
A 3D simulation of a 3×3 rubik's cube

![Main window](/Images/MainInterface.png)

Visit our [release page](https://github.com/phatdatnguyen/VirtualRubiksCube/releases/) to download the latest installer.

## Solver

The **Solve** button calculates a solution from the current stickers and places it in the move queue. Use **Execute** to play the solution. Middle-slice turns and whole-cube rotations are supported; colors are matched to the current centers. The history-reversal action remains available separately.

The solver uses four bounded pattern tables: bottom cross, bottom corners, middle edges, and last layer. Tables are built on demand and reused. Each phase preserves the layers already solved, and the returned sequence is checked against the input before it is queued. Solutions are not necessarily the shortest possible.

Calculation runs in the background. Resetting or closing the window cancels pending work. Invalid sticker counts, missing pieces, impossible twists/flips, and permutation parity errors are reported instead of searched.

## Build and tests

Requires Windows and the .NET 8 SDK or later.

```powershell
dotnet build
dotnet test VirtualRubiksCube.sln --configuration Release
```

The NUnit project in `tests/VirtualRubiksCube.Tests` references the application and runs without opening a window or creating an OpenGL context. Packages restore automatically. Tests are also available in Visual Studio Test Explorer through the [NUnit adapter](https://docs.nunit.org/articles/vs-test-adapter/Adapter-Installation.html).

Coverage includes:

- Solver correctness for 512 reproducible scrambles, known failure patterns, slice turns, and all cube orientations; every solution is replayed against the cube model.
- Sticker permutations, move notation and direction mapping, clone isolation, and rejection of impossible cube states.
- Animated rotation, queue order and events, history reversal, reset, and solved-state detection.
- Async solving, cancellation, stale results, and overlapping requests, using controlled task completion instead of timing assumptions.
- Piece classification, face geometry and selection, coordinate rotation, and projection.

Each parameterized case is independently discoverable, so failing seeds and moves appear in the test output. Tests use fresh cube instances. Rendering and mouse/keyboard interaction still need a manual UI smoke test.

Run only the solver tests:

```powershell
dotnet test VirtualRubiksCube.sln --filter "TestCategory=Solver"
```

Collect coverage (Cobertura XML is written under `TestResults`):

```powershell
dotnet test VirtualRubiksCube.sln --configuration Release --collect:"XPlat Code Coverage" --settings tests/coverage.runsettings --results-directory TestResults
```

Coverage includes the application code, excluding generated designer files and the legacy self-test harness. UI/rendering classes will have lower coverage because these tests do not create a window.

The original console regression check remains available with `dotnet run --project VirtualRubiksCube.csproj -- --solver-test`.
