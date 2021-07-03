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

        private List<Performance> EnrichPerformance(Invoice invoice)
        {
            return invoice.Performances
                .Select(x => new Performance(
                    PlayFor(x),
                    x.Audience
                )).ToList();
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

        private decimal TotalAmount(List<Performance> perfomances)
        {
            decimal result = 0;

            foreach (var performance in perfomances)
            {
                result += performance.Amount;
            }

            return result;
        }

        private double TotalVolumeCredits(List<Performance> performances)
        {
            double result = 0;

            foreach (var performance in performances)
            {
                result += performance.VolumeCredits;
            }

            return result;
        }

        private string BRL(decimal number)
        {
            return (number / 100).ToString("C");
        }



        private Play PlayFor(PerformanceRequest performance)
        {
            return _plays[performance.PlayId];
        }


    }
}
