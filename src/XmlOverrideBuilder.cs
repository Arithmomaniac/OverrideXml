/*
 * Copyright (C) 2014 Ivan Krivyakov, http://www.ikriv.com/
 * And (C) 2016 Avi Levin
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Arithmomaniac.XmlOverride
{
    internal class XmlAttributeOverride
    {
        public XmlAttributeOverride(Type type, string member, XmlAttributes attributes)
        {
            Type = type;
            Member = member;
            Attributes = attributes;
        }
        public Type Type { get; }
        public string Member { get; }
        public XmlAttributes Attributes { get; set; }
    }

    public abstract class OverrideXmlSpec
    {
        protected OverrideXmlSpec(Type type) { Class = type; }

        private Type Class;
        protected abstract string PropertyName { get; }
        protected XmlAttributes Attributes { get; } = new XmlAttributes();
        internal XmlAttributeOverride Compile() => new XmlAttributeOverride(Class, PropertyName, Attributes);
    }

    /// <summary>
    /// A fluent overrider for class-level xml elements.
    /// </summary>
    public class OverrideRootXml<T> : OverrideXmlSpec
    {
        
        internal OverrideRootXml() : base(typeof(T)) { }

        protected override string PropertyName => null;

        /// <summary>
        /// Specifies whether to keep all namespace declarations
        ///  when an object containing a member that returns an <see cref="XmlSerializerNamespaces"/>
        ///  object is overridden.
        /// </summary>
        protected OverrideRootXml<T> Xmlns(bool value)
        {
            Attributes.Xmlns = value;
            return this;
        }

        /// <summary>
        /// Specifies the name of the XML root element via a <see cref="XmlRootAttribute"/>.
        /// </summary>
        /// <param name="elementName">
        /// The name of the XML root element.
        /// </param>
        public OverrideRootXml<T> XmlRoot(string elementName) => Attr(new XmlRootAttribute(elementName));
        /// <summary>
        /// Sets the <see cref="XmlRootAttribute"/> for the class.
        /// </summary>
        public OverrideRootXml<T> Attr(XmlRootAttribute xmlRoot)
        {
            Attributes.XmlRoot = xmlRoot;
            return this;
        }

        /// <summary>
        /// specifies the name of the XML type via a <see cref="XmlTypeAttribute"/>
        /// </summary>
        /// <param name="typeName">
        /// The name of the XML type generated when the class instance is serialized
        ///  (and recognized when deserialized).
        ///</param>
        public OverrideRootXml<T> XmlType(string typeName) => Attr(new XmlTypeAttribute(typeName));

        /// <summary>
        /// Sets the <see cref="XmlTypeAttribute"/> for the class.
        /// </summary>
        public OverrideRootXml<T> Attr(XmlTypeAttribute attribute)
        {
            Attributes.XmlType = attribute;
            return this;
        }
    }

    public class OverrideMemberXml<T> : OverrideXmlSpec
    {
        protected override string PropertyName { get; }

        internal OverrideMemberXml(Expression<Func<T, object>> propertyLambda) : base(typeof(T))
        {
            //http://stackoverflow.com/a/672212/
            var propInfo = (propertyLambda.Body as MemberExpression)?.Member;
            if (    propInfo == null ||
                    (typeof(T) != propInfo.ReflectedType && !typeof(T).IsSubclassOf(propInfo.ReflectedType))
            )
            {
                throw new ArgumentException("A member expression was not provided", nameof(propertyLambda));
            }

            PropertyName = propInfo.Name;
        }

        /// <summary>
        /// Specifies that the member (a field that returns an array of <see cref="XmlAttribute"/>
        /// objects) can contain any XML attributes.
        /// </summary>
        public OverrideMemberXml<T> XmlAny()
        {
            Attributes.XmlAnyAttribute = new XmlAnyAttributeAttribute();
            return this;
        }

        /// <summary>
        /// Specifies that the member must be treated as XML text 
        /// when the class that contains it is serialized or deserialized.
        /// </summary>
        public OverrideMemberXml<T> XmlText()
        {
            Attributes.XmlText = new XmlTextAttribute();
            return this;
        }

        /// <summary>
        /// Specifies whether or not the property is serialized.
        /// </summary>
        public OverrideMemberXml<T> XmlIgnore(bool bIgnore = true)
        {

            Attributes.XmlIgnore = bIgnore;
            return this;
        }


        /// <summary>
        /// Specifies that the class member must be serialized as an array of XML elements,
        /// via a <see cref="XmlArrayAttribute"/>.
        /// </summary>
        public OverrideMemberXml<T> XmlArray() => Attr(new XmlArrayAttribute());

        /// <summary>
        /// Specifies that the class member must be serialized as an array of XML elements,
        /// via a <see cref="XmlArrayAttribute"/>.
        /// </summary>
        /// <param name="elementName">The name of the XML element generated.</param>
        public OverrideMemberXml<T> XmlArray(string elementName) => Attr(new XmlArrayAttribute(elementName));

        /// <summary>
        /// Sets the <see cref="XmlArrayAttribute"/> for the property.
        /// </summary>
        public OverrideMemberXml<T> Attr(XmlArrayAttribute attribute)
        {
            Attributes.XmlArray = attribute;
            return this;
        }

        /// <summary>
        /// Specifies the derived types placed in a serialized array,
        /// via a <see cref="XmlArrayItemAttribute"/>.
        /// </summary>
        public OverrideMemberXml<T> XmlArrayItem() => Attr(new XmlArrayItemAttribute());

        /// <summary>
        /// Specifies the derived types placed in a serialized array,
        /// via a <see cref="XmlArrayItemAttribute"/>.
        /// </summary>
        /// <param name="elementName">The name of the XML element generated.</param>
        public OverrideMemberXml<T> XmlArrayItem(string elementName) => Attr(new XmlArrayItemAttribute(elementName));

        /// <summary>
        /// Sets the <see cref="XmlArrayItemAttribute"/> for the property.
        /// </summary>
        public OverrideMemberXml<T> Attr(XmlArrayItemAttribute attribute)
        {
            Attributes.XmlArrayItems.Add(attribute);
            return this;
        }

        /// <summary>
        /// Specifies that the property is serialized/deserialized as an attribute, 
        /// via the <see cref="XmlAttributeAttribute"/> class.
        /// </summary>
        public OverrideMemberXml<T> XmlAttribute() => Attr(new XmlAttributeAttribute());
        /// <summary>
        /// Specifies that the property is serialized/deserialized as an attribute, 
        /// via the <see cref="XmlAttributeAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the XML attribute generated.</param>
        public OverrideMemberXml<T> XmlAttribute(string name) => Attr(new XmlAttributeAttribute(name));

        /// <summary>
        /// Sets the <see cref="XmlAttributeAttribute"/> for the property.
        /// </summary>
        public OverrideMemberXml<T> Attr(XmlAttributeAttribute attribute)
        {
            Attributes.XmlAttribute = attribute;
            return this;
        }

        /// <summary>
        /// Specifies that the property is serialized/deserialized as an element, 
        /// via the <see cref="XmlElementAttribute"/> class.
        /// </summary>
        public OverrideMemberXml<T> XmlElement() => Attr(new XmlElementAttribute());

        /// <summary>
        /// Specifies that the property is serialized/deserialized as an element, 
        /// via the <see cref="XmlElementAttribute"/> class.
        /// </summary>
        /// /// <param name="name">The name of the XML element generated.</param>
        public OverrideMemberXml<T> XmlElement(string name) => Attr(new XmlElementAttribute(name));


        /// <summary>
        /// Sets the <see cref="XmlElementAttribute"/> for the property.
        /// </summary>
        public OverrideMemberXml<T> Attr(XmlElementAttribute attribute)
        {
            Attributes.XmlElements.Add(attribute);
            return this;
        }
        ///<summary>
        /// Specifies that the member contains objects that represent any XML element
        /// that has no corresponding member in the object being serialized or deserialized, via
        /// The <see cref="XmlAnyElementAttribute"/> class.
        /// </summary>
        public OverrideMemberXml<T> XmlAnyElement() => Attr(new XmlAnyElementAttribute());
        ///<summary>
        /// Specifies that the member contains objects that represent any XML element
        /// that has no corresponding member in the object being serialized or deserialized, via
        /// The <see cref="XmlAnyElementAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the XML element generated.</param>
        public OverrideMemberXml<T> XmlAnyElement(string name) => Attr(new XmlAnyElementAttribute(name));
        ///<summary>
        /// Specifies that the member contains objects that represent any XML element
        /// that has no corresponding member in the object being serialized or deserialized, via
        /// The <see cref="XmlAnyElementAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the XML element generated.</param>
        /// <param name="ns">The name of the XML namespace generated.</param>
        public OverrideMemberXml<T> XmlAnyElement(string name, string ns) => Attr(new XmlAnyElementAttribute(name, ns));

        /// <summary>
        /// Sets the <see cref="XmlAnyElementAttribute"/> for the property.
        /// </summary>
        public OverrideMemberXml<T> Attr(XmlAnyElementAttribute attribute)
        {
            Attributes.XmlAnyElements.Add(attribute);
            return this;
        }
        /// <summary>
        /// Gets or sets the default value of an XML element or attribute.
        /// </summary>
        public OverrideMemberXml<T> XmlDefaultValue(object value)
        {
            Attributes.XmlDefaultValue = value;
            return this;
        }
    }

    /// <summary>
    /// A builder class for creating an <see cref="XmlAttributeOverrides"/> object.
    /// </summary>
    public class XmlOverrideBuilder
    {
        private List<OverrideXmlSpec> _overrides = new List<OverrideXmlSpec>();
        
        /// <summary>
        /// Fluidly configures the xml overrides for a given class.
        /// </summary>
        /// <typeparam name="T">The class to override</typeparam>
        /// <param name="configurator">The way the given class is to be configured</param>
        public XmlOverrideBuilder Configure<T>(Action<OverrideXmlClass<T>> configurator)
        {
            var classOverrides = new OverrideXmlClass<T>();
            configurator(classOverrides);
            _overrides.AddRange(classOverrides.Overrides);
            return this;
        }

        /// <summary>
        /// Instantiates the builder as an <see cref="XmlAttributeOverrides"/> object.
        /// </summary>
        public XmlAttributeOverrides Commit()
        {
            var overrides = new XmlAttributeOverrides();
            foreach (var ovride in _overrides.Select(x => x.Compile()))
            {
                if (ovride.Member == null)
                    overrides.Add(ovride.Type, ovride.Attributes);
                else
                    overrides.Add(ovride.Type, ovride.Member, ovride.Attributes);
            }
            return overrides;
        }
    }

    /// <summary>
    /// Contains an XML override configuration for a given class.
    /// </summary>
    /// <typeparam name="T">The class to override</typeparam>
    public class OverrideXmlClass<T>
    {
        private List<OverrideXmlSpec> _overrides = new List<OverrideXmlSpec>();
        internal IReadOnlyCollection<OverrideXmlSpec> Overrides => _overrides;

        /// <summary>
        /// Instantiates a fluent overrider for class-level xml elements.
        /// </summary>
        public OverrideRootXml<T> ForRoot()
        {
            var ovride = new OverrideRootXml<T>();
            _overrides.Add(ovride);
            return ovride;
        }

        /// <summary>
        /// Instantiates a fluent overrider for a given property's xml elements.
        /// </summary>
        /// <param name="propertyLambda"></param>
        public OverrideMemberXml<T> ForMember(Expression<Func<T, object>> propertyLambda)
        {
            var ovride = new OverrideMemberXml<T>(propertyLambda);
            _overrides.Add(ovride);
            return ovride;
        }
    }
}
