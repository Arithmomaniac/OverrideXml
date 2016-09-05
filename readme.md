XmlOverrideBuilder
===

&copy; 2016 Avi Levin
Based on work &copy; 2014 Ivan Krivyakov

This project is an XML serialization helper for .NET. It creates an `XmlAttributeOverrides` via a fluent interface.

This project is heavily based on [OverrideXml](https://github.com/ikriv/OverrideXml), and the basic idea of how to use it is explained in [Ivan's blog](http://www.ikriv.com/dev/dotnet/OverrideXml.shtml). I essentially just refactored the code to match more normal fluid pattterns, as I explain [here](https://medium.com/@Arithmomaniac/fluid-interfaces-considered-harmful-6abc7cbc58d3). Here is how you would write the example he gives on his blog using this package:

```csharp
XmlAttributeOverrides GetOverrides()
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
```