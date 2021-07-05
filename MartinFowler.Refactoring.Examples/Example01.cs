using System;
using System.Collections.Generic;
using System.Linq;

namespace MartinFowler.Refactoring.Examples
{
    public class Customer
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class PerformanceRequest
    {
        public string PlayId { get; set; }
        public int Audience { get; set; }

        public PerformanceRequest(string playId, int audience)
        {
            PlayId = playId;
            Audience = audience;
        }
    }

    public class Invoice
    {
        public Customer Customer { get; set; }
        public List<PerformanceRequest> Performances { get; set; }

        public Invoice(Customer customer, List<PerformanceRequest> performances)
        {
            Customer = customer;
            Performances = performances;
        }
    }

    public class Play
    {
        public Play(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class StatementData
    {
        public Customer Customer { get; set; }
        public List<Performance> Performances { get; set; }
        public decimal TotalAmount { get; set; }
        public double TotalVolumeCredits { get; set; }
    }

    public class Performance
    {
        public Play Play { get; set; }
        public int Audience { get; set; }
        public decimal Amount { get; set; }
        public double VolumeCredits { get; set; }

        public Performance(Play play, int audience)
        {
            Play = play;
            Audience = audience;
            Amount = AmountFor();
            VolumeCredits = VolumeCreditsFor();
        }

        private double VolumeCreditsFor()
        {
            double result = 0;

            // add volume credits
            result += Math.Max(Audience - 30, 0);

            // add extra credit for every ten comedy attendees
            if (Play.Type == "comedy")
            {
                result += Math.Floor(Convert.ToDouble(Audience / 5));
            }

            return result;
        }

        private decimal AmountFor()
        {
            decimal result;

            switch (Play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (Audience > 30)
                    {
                        result += 1000 * (Audience - 30);
                    }
                    break;
                case "comedy":
                    result = 30000;
                    if (Audience > 20)
                    {
                        result += 10000 + 500 * (Audience - 20);
                    }
                    result += 300 * Audience;
                    break;
                default:
                    throw new Exception($"unknow type for: {Play.Type}");
            }

            return result;
        }
    }

    public class StatementFactory
    {
        private Dictionary<string, Play> _plays;

        private StatementFactory()
        {

        }

        public static StatementData Create(Invoice invoice, Dictionary<string, Play> plays)
        {
            return new StatementFactory().Build(invoice, plays);
        }

        private StatementData Build(Invoice invoice, Dictionary<string, Play> plays)
        {
            _plays = plays;

            var performances = EnrichPerformance(invoice.Performances);

            return new StatementData
            {
                Customer = invoice.Customer,
                Performances = performances,
                TotalAmount = TotalAmount(performances),
                TotalVolumeCredits = TotalVolumeCredits(performances)
            };
        }

        private List<Performance> EnrichPerformance(List<PerformanceRequest> performances)
        {
            return performances
                .Select(x => new Performance(PlayFor(x), x.Audience))
                .ToList();
        }

        private Play PlayFor(PerformanceRequest performance)
        {
            return _plays[performance.PlayId];
        }

        private decimal TotalAmount(List<Performance> performances)
        {
            return performances
                .Select(x => x.Amount)
                .Aggregate((x, y) => x + y);
        }

        private double TotalVolumeCredits(List<Performance> performances)
        {
            return performances
                .Select(x => x.VolumeCredits)
                .Aggregate((x, y) => x + y);
        }
    }

    public class Example01
    {
        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            return RenderTextPlain(StatementFactory.Create(invoice, plays));
        }

        private string RenderTextPlain(StatementData data)
        {
            var result = $"Statement for {data.Customer}\n";

            foreach (var performance in data.Performances)
            {
                result += $"  {performance.Play.Name}: {(BRL(performance.Amount))} ({performance.Audience} seats)\n";
            }

            result += $"Amount owed is {BRL(data.TotalAmount)}\n";
            result += $"You earned {data.TotalVolumeCredits} credits";
            return result;
        }

        private string RenderHtml(StatementData data)
        {
            var result = $"<h1> Statement for {data.Customer}</h1>\n";

            result += "<table>\n";

            result += "<tr><th>play</th><th>seats</th><th>cost</th></tr>";

            foreach (var performance in data.Performances)
            {
                result += $"<tr><td>{performance.Play.Name}</td><td>{performance.Audience}</td>";
                result += $"<td>{ BRL(performance.Amount)}</td></tr>\n";
            }

            result += "</table>\n";

            return result;
        }

        private string BRL(decimal number)
        {
            return (number / 100).ToString("C");
        }
    }
}
