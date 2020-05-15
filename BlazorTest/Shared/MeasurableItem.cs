using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTest.Shared
{
    public class MeasurableItem : IMeasurableItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public double? RealMesure { get; set; }

        public double GetHeight(double sizeContainer)
        {
            if (RealMesure.HasValue)
            {
                //Console.WriteLine("real mesure " + RealMesure.Value.ToString());
                return RealMesure.Value;
            }
            else
            {
                var compute = (Text.Length / (sizeContainer / 10) * 20) + 10;
                //Console.WriteLine("compute mesure " + compute.ToString());
                return compute;
            }
        }
    }

    public interface IMeasurableItem
    {
        double GetHeight(double sizeContainer);

        double? RealMesure { get; set; }
    }
}
