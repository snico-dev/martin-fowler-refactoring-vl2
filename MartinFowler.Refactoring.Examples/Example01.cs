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

        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            decimal totalAmount = 0;
            double volumeCredits = 0;

            var result = $"Statement for {invoice.Customer}\n";

            foreach (var performance in invoice.Performances)
            {
                var play = plays[performance.PlayId];

                decimal amount = 0;

                // partner name: extract function
                amount = AmountFor(performance, play);

                // add volume credits
                volumeCredits += Math.Max(performance.Audience - 30, 0);

                // add extra credit for every ten comedy attendees
                if (play.Type == "comedy")
                {
                    volumeCredits += Math.Floor(Convert.ToDouble(performance.Audience / 5));
                }

                // print line for this order
                result += $"  {play.Name}: {(amount / 100).ToString("C")} ({performance.Audience} seats)\n";
                totalAmount += amount;
            }

            result += $"Amount owed is {(totalAmount / 100).ToString("C")}\n";
            result += $"You earned {volumeCredits} credits";

            return result;
        }

        private decimal AmountFor(Performance performance, Play play)
        {
            decimal result;
            
            switch (play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (performance.Audience > 30)
                    {
                        result += 1000 * (performance.Audience - 30);
                    }
                    break;
                case "comedy":
                    result = 30000;
                    if (performance.Audience > 20)
                    {
                        result += 10000 + 500 * (performance.Audience - 20);
                    }
                    result += 300 * performance.Audience;
                    break;
                default:
                    throw new Exception($"unknow type for: {play.Type}");
            }

            return result;
        }
    }
}
