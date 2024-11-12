global using Microsoft.Extensions.Logging;
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

[assembly: InternalsVisibleTo("ConnectTests")]
[assembly: InternalsVisibleTo("SystemTests")]
[assembly: InternalsVisibleTo("UnitTests")]
[assembly: InternalsVisibleTo("ClientServer")]

[assembly: SuppressMessage("Usage", "IDE0130: Namespace does not match folder structure")]
[assembly: SuppressMessage("Usage", "CA1848: Use the LoggerMessage delegates")]
[assembly: SuppressMessage("Usage", "CA1031:Catch a more specific allowed exception type,")]
