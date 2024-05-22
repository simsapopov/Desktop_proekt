// MainViewModel.cs
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;

public class MainViewModel
{
    public SeriesCollection SeriesCollection { get; set; }
    public string[] Labels { get; set; }
    public Func<double, string> Formatter { get; set; }

    public MainViewModel()
    {
        // Creating series for each product with its interest rates across currencies
        SeriesCollection = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Product A",
                Values = new ChartValues<double> { 1.5, 1.7, 1.9 } // Example rates: USD, EUR, BGN
            },
            new LineSeries
            {
                Title = "Product B",
                Values = new ChartValues<double> { 2.0, 2.2, 2.4 }
            },
            new LineSeries
            {
                Title = "Product C",
                Values = new ChartValues<double> { 2.5, 2.7, 2.9 }
            }
        };

        // Labels for the currencies
        Labels = new[] { "USD", "EUR", "BGN" };

        // Formatter for y-axis values (interest rates)
        Formatter = value => value.ToString("N2") + "%";
    }
}
