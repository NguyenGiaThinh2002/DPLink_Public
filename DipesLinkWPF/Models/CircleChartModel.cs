﻿using DipesLink.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace DipesLink.Models
{
    public class CircleChartModel : ViewModelBase
    {
        public IEnumerable<ISeries> Series { get; set; }
        private GaugeItem _gaugeItem;
        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                _gaugeItem.Value.Value = value;
            }
        }
        public CircleChartModel()
        {
            _gaugeItem = new GaugeItem(0, series =>
            {
                series.MaxRadialColumnWidth = 10;
                series.DataLabelsSize = 20;
                series.DataLabelsFormatter = point => $"{point.PrimaryValue}%";
            });
            Series = GaugeGenerator.BuildSolidGauge(_gaugeItem);
            
        }
    }
}
