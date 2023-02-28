global using System;
global using System.Linq;
global using System.Reactive.Linq;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Logging;
global using NodaTime;

using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("ConnectTests")]
[assembly: InternalsVisibleTo("SystemTests")]
[assembly: InternalsVisibleTo("UnitTests")]

[assembly: InternalsVisibleTo("ClientServer")]
