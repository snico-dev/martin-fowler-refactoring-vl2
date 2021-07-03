﻿using System;
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


            var result = $"Statement for {invoice.Customer}\n";

            foreach (var performance in invoice.Performances)
            {
                // print line for this order
                result += $"  {PlayFor(performance).Name}: {(BRL(AmountFor(performance)))} ({performance.Audience} seats)\n";
            }
            
            result += $"Amount owed is {BRL(TotalAmount(invoice))}\n";
            result += $"You earned {TotalVolumeCredits(invoice)} credits";

            return result;
        }

        private decimal TotalAmount(Invoice invoice)
        {
            decimal result = 0;
            
            foreach (var performance in invoice.Performances)
            {
                result += AmountFor(performance);
            }

            return result;
        }

        private double TotalVolumeCredits(Invoice invoice)
        {
            double result = 0;
            
            foreach (var performance in invoice.Performances)
            {
                result += VolumeCreditsFor(performance);
            }

            return result;
        }

        private string BRL(decimal number)
        {
            return (number / 100).ToString("C");
        }

        private double VolumeCreditsFor(Performance aPerformance)
        {
            double result = 0;

            // add volume credits
            result += Math.Max(aPerformance.Audience - 30, 0);

            // add extra credit for every ten comedy attendees
            if (PlayFor(aPerformance).Type == "comedy")
            {
                result += Math.Floor(Convert.ToDouble(aPerformance.Audience / 5));
            }

            return result;
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
