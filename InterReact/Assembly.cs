global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using NodaTime;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reactive.Linq;
global using System.Threading;
global using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("InterReact.Tests")]
[assembly: InternalsVisibleTo("ClientServer")]

[assembly: SuppressMessage("Usage", "IDE0130: Namespace does not match folder structure")]
[assembly: SuppressMessage("Usage", "CA1873: Evaluation of this argument may be expensive")]
[assembly: SuppressMessage("Usage", "CA1848: Use the LoggerMessage delegates")]
[assembly: SuppressMessage("Usage", "CA1031: Catch a more specific allowed exception type")]
[assembly: SuppressMessage("Design","CA1034: Nested types should not be visible")]
[assembly: SuppressMessage("Naming","CA1708: Identifiers should differ by more than case")]
