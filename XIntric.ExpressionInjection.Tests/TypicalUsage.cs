using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XIntric.ExpressionInjection.Tests
{

    namespace TypicalUsage
    {
        public class Airplane
        {
            public string Label { get; set; }
            public List<Flight> Flights { get; set; }
        }

        public class Flight
        {
            public string Number { get; set; }

            public DateTime Starts { get; set; }
            public DateTime Ends { get; set; }


            public Airplane Airplane { get; set; }

        }

        public static class Methods
        {
            [Injectable]
            public static bool IsOverbooked(this Airplane airplane)
                => Injector.Inject(() =>
                 airplane.Flights.Any(left => airplane.Flights.Any(right =>
                    left != right &&
                     ((left.Starts >= right.Starts && left.Starts < right.Ends)
                     || (right.Starts >= left.Starts && right.Starts < left.Ends)))));
        }

        public class Scenario1
        {

            public Scenario1()
            {
                Airplane1 = new Airplane { Label = "Airplane1" };
                Airplane2 = new Airplane { Label = "Airplane2" };

                Airplanes.Add(Airplane1);
                Airplanes.Add(Airplane2);

                Flights.AddRange(new Flight[]
                    {
                        new Flight
                        {
                            Number = "FLIGHT01",
                            Airplane = Airplane1,
                            Starts = new DateTime(2018,1,1,13,57,0),
                            Ends = new DateTime(2018,1,1,15,33,0),
                        },
                        new Flight
                        {
                            Number = "FLIGHT02_COLLIDINGWITH01",
                            Airplane = Airplane1,
                            Starts = new DateTime(2018,1,1,14,30,0),
                            Ends = new DateTime(2018,1,1,17,33,0),
                        },
                        new Flight
                        {
                            Number = "FLIGHT03",
                            Airplane = Airplane2,
                            Starts = new DateTime(2018,1,1,14,30,0),
                            Ends = new DateTime(2018,1,1,17,33,0),
                        }
                    });

                foreach(var ap in Airplanes)
                {
                    ap.Flights = Flights.Where(x => x.Airplane == ap).ToList();
                }

            }

            public List<Flight> Flights = new List<Flight>();
            public List<Airplane> Airplanes = new List<Airplane>();
            Airplane Airplane1;
            Airplane Airplane2;


            [Fact]
            public void UseInjectableMethodToGetStatusOnSingleObject()
            {
                Assert.True(Airplane1.IsOverbooked());
                Assert.False(Airplane2.IsOverbooked());
            }

            [Fact]
            public void UseInjectableMethodToGetOverview()
            {

                var stats = Airplanes.AsQueryable()
                    .EnableInjection()
                    .Select(ap => new
                    {
                        ap.Label,
                        NeedsReplanning = ap.IsOverbooked(),
                    }).ToList();
            }



        }


    }

}
