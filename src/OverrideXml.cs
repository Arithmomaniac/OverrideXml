/*
 * Copyright (C) 2014 Ivan Krivyakov, http://www.ikriv.com/
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
using System.Reflection;
using System.Xml.Serialization;

namespace Ikriv.Xml
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

    public class OverrideRootXml<T> : OverrideXmlSpec
    {
        
        internal OverrideRootXml() : base(typeof(T)) { }

        protected override string PropertyName => null;
    }

    public class OverrideMemberXml<T> : OverrideXmlSpec
    {
        protected override string PropertyName { get; }

        internal OverrideMemberXml(Expression<Func<T, object>> propertyLambda) : base(typeof(T))
        {
            //http://stackoverflow.com/a/672212/
            var propInfo = (propertyLambda.Body as MemberExpression)?.Member as PropertyInfo;
            if (    propInfo == null ||
                    (typeof(T) != propInfo.ReflectedType && !typeof(T).IsSubclassOf(propInfo.ReflectedType))
            )
            {
                throw new ArgumentException("A property expression was not provided", nameof(propertyLambda));
            }

            PropertyName = propInfo.Name;
        }
    }

    public class OverrideXml2
    {
        private List<OverrideXmlSpec> _overrides = new List<OverrideXmlSpec>();

        public OverrideRootXml<T> ForRoot<T>() {
            var ovride = new OverrideRootXml<T>();
            _overrides.Add(ovride);
            return ovride;
        }
        public OverrideMemberXml<T> ForMember<T>(Expression<Func<T, object>> propertyLambda)
        {
            var ovride = new OverrideMemberXml<T>(propertyLambda);
            _overrides.Add(ovride);
            return ovride;
            
        }
             
        public XmlAttributeOverrides Commit()
        {
            var overrides = new XmlAttributeOverrides();
            foreach (var ovride in _overrides.Select(x => x.Compile()))
            {
                overrides.Add(ovride.Type, ovride.Member, ovride.Attributes);
            }
            return overrides;
        }
    }



    /// <summary>
    /// Creates XmlAttributeOverrides instance using an easy-to-use fluent interface
    /// </summary>
    public class OverrideXml
    {
        private XmlAttributes _attributes;
        

        /// <summary>
        /// Adds [XmlRoot(elementName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlRoot(string elementName)
        {
            
            _attributes.XmlRoot = new XmlRootAttribute(elementName);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlRootAttribute for current type or member
        /// </summary>
        public OverrideXml Attr(XmlRootAttribute xmlRoot)
        {
            
            _attributes.XmlRoot = xmlRoot;
            return this;
        }

        /// <summary>
        /// Adds [XmlAttribute] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAttribute()
        {
            
            _attributes.XmlAttribute = new XmlAttributeAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlAttribute(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAttribute(string name)
        {
            
            _attributes.XmlAttribute = new XmlAttributeAttribute(name);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlAttributeAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlAttributeAttribute attribute)
        {
            
            _attributes.XmlAttribute = attribute;
            return this;
        }

        /// <summary>
        /// Adds [XmlElement] attribute to current type or member
        /// </summary>
        public OverrideXml XmlElement()
        {
            
            _attributes.XmlElements.Add(new XmlElementAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlElement(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlElement(string name)
        {
            
            _attributes.XmlElements.Add(new XmlElementAttribute(name));
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlElementAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlElementAttribute attribute)
        {
            
            _attributes.XmlElements.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds [XmlIgnore] attribute to current type or member
        /// </summary>
        /// <param name="bIgnore"></param>
        public OverrideXml XmlIgnore(bool bIgnore=true)
        {
            
            _attributes.XmlIgnore = bIgnore;
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyAttribute] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyAttribute()
        {
            
            _attributes.XmlAnyAttribute = new XmlAnyAttributeAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement()
        {
            
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement(name)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement(string name)
        {
            
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute(name));
            return this;
        }

        /// <summary>
        /// Adds [XmlAnyElement(name,ns)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlAnyElement(string name, string ns)
        {
            
            _attributes.XmlAnyElements.Add(new XmlAnyElementAttribute(name, ns));
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlAnyElementAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlAnyElementAttribute attribute)
        {
            
            _attributes.XmlAnyElements.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds [XmlArray] attribute to current type or memeber
        /// </summary>
        public OverrideXml XmlArray()
        {
            
            _attributes.XmlArray = new XmlArrayAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlArray(elementName)] attribute to current type or memeber
        /// </summary>
        public OverrideXml XmlArray(string elementName)
        {
            
            _attributes.XmlArray = new XmlArrayAttribute(elementName);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlArrayAttribute to current type or memeber
        /// </summary>
        public OverrideXml Attr(XmlArrayAttribute attribute)
        {
            
            _attributes.XmlArray = attribute;
            return this;
        }

        /// <summary>
        /// Adds [XmlArrayItem] attribute to current type or member
        /// </summary>
        public OverrideXml XmlArrayItem()
        {
            
            _attributes.XmlArrayItems.Add(new XmlArrayItemAttribute());
            return this;
        }

        /// <summary>
        /// Adds [XmlArrayItem(elementName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlArrayItem(string elementName)
        {
            
            _attributes.XmlArrayItems.Add(new XmlArrayItemAttribute(elementName));
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlArrayItemAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlArrayItemAttribute attribute)
        {
            
            _attributes.XmlArrayItems.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds [XmlDefault(value)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlDefaultValue(object value)
        {
            
            _attributes.XmlDefaultValue = value;
            return this;
        }

        /// <summary>
        /// Applies or removes [XmlNamespaceDeclarations] attribute from current type or member
        /// </summary>
        public OverrideXml Xmlns(bool value)
        {
            
            _attributes.Xmlns = value;
            return this;
        }

        /// <summary>
        /// Adds [XmlText] attribute to current type or member
        /// </summary>
        public OverrideXml XmlText()
        {
            
            _attributes.XmlText = new XmlTextAttribute();
            return this;
        }

        /// <summary>
        /// Adds [XmlType(typeName)] attribute to current type or member
        /// </summary>
        public OverrideXml XmlType(string typeName)
        {
            
            _attributes.XmlType = new XmlTypeAttribute(typeName);
            return this;
        }

        /// <summary>
        /// Adds specified instance of XmlTypeAttribute to current type or member
        /// </summary>
        public OverrideXml Attr(XmlTypeAttribute attribute)
        {
            
            _attributes.XmlType = attribute;
            return this;
        }

    }


}
