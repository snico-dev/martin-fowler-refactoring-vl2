using FluentAssertions;
using MartinFowler.Refactoring.Examples;
using System.Collections.Generic;
using Xunit;

namespace MartinFowler.Refactoring.Tests
{
    public class Example01Test
    {
        [Fact]
        public void GivenInvoiceWithAListdePerformances_WhenExecuteStatement_ShouldReturnCalculateAInvoiceResult()
        {
            var example01 = new Example01();

            var customer = new Customer() { Name = "BigCo" };

            var plays = new Dictionary<string, Play>()
            {
                {
                    "hamlet", new Play("Hamlet","tragedy")
                },
                {
                    "as-like", new Play("As You Like It","comedy")
                },
                {
                    "othello", new Play("Othello","tragedy")
                }
            };

            var invoice = new Invoice(customer, new List<Performance> { 
                new Performance("hamlet", 55),
                new Performance("as-like", 35),
                new Performance("othello", 40)
            });
            
            var result = example01.Statement(
                invoice,
                plays);

            var expected = "Statement for BigCo\n";
            expected += "  Hamlet: R$ 650,00 (55 seats)\n";
            expected += "  As You Like It: R$ 580,00 (35 seats)\n";
            expected += "  Othello: R$ 500,00 (40 seats)\n";
            expected += "Amount owed is R$ 1.730,00\n";
            expected += "You earned 47 credits";

            result.Should().Be(expected);
        }
    }
}
