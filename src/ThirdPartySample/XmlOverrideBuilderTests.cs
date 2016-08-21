using Arithmomaniac.XmlOverride;
using System.Collections.Generic;
using System.Xml.Serialization;
using Xunit;
using StatePrinting;

namespace Arithmomaniac.XmlOverride.Tests
{


    public class XmlOverrideBuilderTests
    {
        private class Continent
        {
            public string Name { get; set; }
            public List<Country> Countries { get; set; }
        }

        private class Country
        {
            public string Name { get; set; }
            public string Capital { get; set; }
        }

        private static XmlAttributeOverrides GetOverrides()
        {
            return new XmlOverrideBuilder()
                .Configure<Continent>(x => {
                    x.ForRoot().XmlRoot("continent");
                    x.ForMember(y => y.Name).XmlAttribute("name");
                    x.ForMember(y => y.Countries).XmlElement("state");
                })
                .Configure<Country>(x => {
                    x.ForMember(y => y.Name).XmlAttribute("name");
                    x.ForMember(y => y.Capital).XmlAttribute("capital");
                })
                .Commit();
        }

        private static XmlAttributeOverrides GetOverridesRaw()
        {
            var overrides = new XmlAttributeOverrides();

            overrides.Add(typeof(Continent), new XmlAttributes { XmlRoot = new XmlRootAttribute("continent") });

            overrides.Add(typeof(Continent), "Name", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("name")});

            overrides.Add(typeof(Continent), "Countries", 
                new XmlAttributes { XmlElements = { new XmlElementAttribute("state") } });

            overrides.Add(typeof(Country), "Name", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("name")});

            overrides.Add(typeof(Country), "Capital", 
                new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("capital")});

            return overrides;
        }

        [Fact]
        public void HelperOverrideBehavesIdenticallyToManualOverride()
        {
            //lazy deep equality comparison
            var printer = new Stateprinter();
            Assert.Equal(printer.PrintObject(GetOverrides()), printer.PrintObject(GetOverridesRaw()));

        }
    }
}
