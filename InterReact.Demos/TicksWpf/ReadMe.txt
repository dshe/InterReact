This demo displays market depth.

Note that the IB demo account does not provide market depth data.

        internal static IObservable<Color> ToColor(this IObservable<double?> source)
        {
            return source.Select(change =>
            {
                if (change == null) // no previous value available
                    return Color.DarkGray;
                if (change > 0)
                    return Color.Green;
                if (change < 0)
                    return Color.Red;
                return Color.White;
            });
        }
