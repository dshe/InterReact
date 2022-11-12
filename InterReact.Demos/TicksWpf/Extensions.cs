using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;

namespace TicksWpf;

public static class Extensions
{
    public static IObservable<double> Changes(this IObservable<double> source) => source
        .DistinctUntilChanged()
        .Buffer(2, 1)
        .Where(list => list.Count == 2)
        .Select(list => list[1] - list[0]);

    public static IObservable<SolidColorBrush> ToColor(this IObservable<double> source) => source
        .Select(d =>
        {
            if (d > 0)
                return Brushes.LightGreen;
            else if (d < 0)
                return Brushes.Red;
            else
                return Brushes.White;
        });
}
