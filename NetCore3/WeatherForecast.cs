using System;
using Newtonsoft.Json;

namespace NetCore3
{
    [JsonObject(MemberSerialization.OptOut)]
    public class WeatherForecast
    {
        public DateTime Date { get; set; }


        public int TemperatureC { get; set; }

        [JsonIgnore]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }


    }

    [JsonObject(MemberSerialization.OptOut)]
    public class MortgageParameters
    {

        public decimal Principal { get; set; }

        public decimal Rate { get; set; }
        public int Frequency { get; set; }
        public int Amortization { get; set; }

    }

    public class Payment
    {

        public string Description { get; set; }

        public decimal MonthlyPayment { get; set; }

        public decimal InstallPayment { get; set; }

        public decimal TotalCostOfBorrow { get; set; }

        public decimal TotalPayment { get; set; }
        public int Frequency { get; set; }

        public string Freq { get; set; }

        public decimal AmountSave { get; set; }
    }
}
