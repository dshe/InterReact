using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
namespace TicksWpf;

public static class Xtensions
{
    extension(IObservable<double> source)
    {
        public IObservable<double> Changes() => source
            .DistinctUntilChanged()
            .Buffer(2, 1)
            .Where(list => list.Count == 2)
            .Select(list => list[1] - list[0]);

        public IObservable<SolidColorBrush> ToColor() => source
            .Select(d =>
            {
                if (d > 0)
                    return Brushes.LightGreen;
                if (d < 0)
                    return Brushes.Red;
                return Brushes.White;
            });
    }
}
