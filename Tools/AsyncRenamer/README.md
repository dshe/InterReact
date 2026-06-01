AsyncRenamer
============

Tool to rename methods marked async that don't end with "Async".

Usage:

  dotnet run --project Tools/AsyncRenamer/AsyncRenamer.csproj <path-to-solution.sln>

This will rename symbols using Roslyn workspaces and apply changes to source files.
