global using Microsoft.Extensions.Logging;
global using InterReact;
global using NodaTime;
global using Xunit;
global using Tests;
using System.Diagnostics.CodeAnalysis;

// tests will run in sequence
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

[assembly: SuppressMessage("Usage", "IDE0130: Namespace does not match folder structure")]
