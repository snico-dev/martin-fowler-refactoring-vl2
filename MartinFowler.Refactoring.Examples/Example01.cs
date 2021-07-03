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

    public class Performance
    {
        public string PlayId { get; set; }
        public int Audience { get; set; }

        public Performance(string playId, int audience)
        {
            PlayId = playId;
            Audience = audience;
        }
    }

    public class Invoice
    {
        public Customer Customer { get; set; }
        public List<Performance> Performances { get; set; }

        public Invoice(Customer customer, List<Performance> performances)
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
        public List<PerformanceEnriched> Performances { get; set; }
        public decimal TotalAmount { get; set; }
        public double TotalVolumeCredits { get; set; }
    }

    public class PerformanceEnriched
    {
        public Play Play { get; set; }
        public int Audience { get; set; }
        public decimal Amount { get; set; }
        public double VolumeCredits { get; set; }
    }

    public class Example01
    {
        private Dictionary<string, Play> _plays { get; set; }
        private Invoice _invoice;

        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            _plays = plays;
            _invoice = invoice;

            var statementData = EnrichStatementData(invoice);

            return RenderTextPlain(statementData);
        }

        private StatementData EnrichStatementData(Invoice invoice)
        {
            var performances = EnrichPerformance(invoice);

            return new StatementData
            {
                Customer = invoice.Customer,
                Performances = performances,
                TotalAmount = TotalAmount(performances),
                TotalVolumeCredits = TotalVolumeCredits(performances)
            };
        }

        private List<PerformanceEnriched> EnrichPerformance(Invoice invoice)
        {
            return invoice.Performances
                .Select(x => {
                    var performance = new PerformanceEnriched
                    {
                        Play = PlayFor(x),
                        Audience = x.Audience
                    };
                    performance.Amount = AmountFor(performance);
                    performance.VolumeCredits = VolumeCreditsFor(performance);

                    return performance;
                }).ToList();
        }

        private string RenderTextPlain(StatementData data)
        {
            var result = $"Statement for {data.Customer}\n";

            foreach (var performance in data.Performances)
            {
                // print line for this order
                result += $"  {performance.Play.Name}: {(BRL(performance.Amount))} ({performance.Audience} seats)\n";
            }

            result += $"Amount owed is {BRL(data.TotalAmount)}\n";
            result += $"You earned {data.TotalVolumeCredits} credits";
            return result;
        }

        private decimal TotalAmount(List<PerformanceEnriched> perfomances)
        {
            decimal result = 0;

            foreach (var performance in perfomances)
            {
                result += AmountFor(performance);
            }

            return result;
        }

        private double TotalVolumeCredits(List<PerformanceEnriched> performances)
        {
            double result = 0;

            foreach (var performance in performances)
            {
                result += VolumeCreditsFor(performance);
            }

            return result;
        }

        private string BRL(decimal number)
        {
            return (number / 100).ToString("C");
        }

        private double VolumeCreditsFor(PerformanceEnriched aPerformance)
        {
            double result = 0;

            // add volume credits
            result += Math.Max(aPerformance.Audience - 30, 0);

            // add extra credit for every ten comedy attendees
            if (aPerformance.Play.Type == "comedy")
            {
                result += Math.Floor(Convert.ToDouble(aPerformance.Audience / 5));
            }

            return result;
        }

        private Play PlayFor(Performance performance)
        {
            return _plays[performance.PlayId];
        }

        private decimal AmountFor(PerformanceEnriched aPerformance)
        {
            decimal result;

            switch (aPerformance.Play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (aPerformance.Audience > 30)
                    {
                        result += 1000 * (aPerformance.Audience - 30);
                    }
                    break;
                case "comedy":
                    result = 30000;
                    if (aPerformance.Audience > 20)
                    {
                        result += 10000 + 500 * (aPerformance.Audience - 20);
                    }
                    result += 300 * aPerformance.Audience;
                    break;
                default:
                    throw new Exception($"unknow type for: {aPerformance.Play.Type}");
            }

            return result;
        }
    }
}
