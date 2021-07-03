using System;
using System.Collections.Generic;

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

    public class Example01
    {
        private Dictionary<string, Play> _plays { get; set; }

        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            _plays = plays;

            decimal totalAmount = 0;
            double volumeCredits = 0;

            var result = $"Statement for {invoice.Customer}\n";

            foreach (var performance in invoice.Performances)
            {
                volumeCredits += VolumeCreditsFor(performance);

                // print line for this order
                result += $"  {PlayFor(performance).Name}: {(AmountFor(performance) / 100).ToString("C")} ({performance.Audience} seats)\n";
                totalAmount += AmountFor(performance);
            }

            result += $"Amount owed is {(totalAmount / 100).ToString("C")}\n";
            result += $"You earned {volumeCredits} credits";

            return result;
        }

        private double VolumeCreditsFor(Performance aPerformance)
        {
            double volumeCredits = 0;

            // add volume credits
            volumeCredits += Math.Max(aPerformance.Audience - 30, 0);

            // add extra credit for every ten comedy attendees
            if (PlayFor(aPerformance).Type == "comedy")
            {
                volumeCredits += Math.Floor(Convert.ToDouble(aPerformance.Audience / 5));
            }

            return volumeCredits;
        }

        private Play PlayFor(Performance performance)
        {
            return _plays[performance.PlayId];
        }

        private decimal AmountFor(Performance aPerformance)
        {
            decimal result;

            switch (PlayFor(aPerformance).Type)
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
                    throw new Exception($"unknow type for: {PlayFor(aPerformance).Type}");
            }

            return result;
        }
    }
}
