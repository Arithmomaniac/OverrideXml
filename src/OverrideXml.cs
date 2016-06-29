/*
 * Copyright (C) 2014 Ivan Krivyakov, http://www.ikriv.com/
 * And (C) 2014 Avi Levin
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

namespace Arithmomaniac.OverrideXml
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


        protected OverrideRootXml<T> Xmlns(bool value)
        {

            Attributes.Xmlns = value;
            return this;
        }

        public OverrideRootXml<T> XmlRoot(string elementName) => Attr(new XmlRootAttribute(elementName));
        public OverrideRootXml<T> Attr(XmlRootAttribute xmlRoot)
        {

            Attributes.XmlRoot = xmlRoot;
            return this;
        }

        
        public OverrideRootXml<T> XmlType(string typeName) => Attr(new XmlTypeAttribute(typeName));

        public OverrideRootXml<T> Attr(XmlTypeAttribute attribute)
        {

            Attributes.XmlType = attribute;
            return this;
        }

        public OverrideRootXml<T> XmlDefaultValue(object value)
        {

            Attributes.XmlDefaultValue = value;
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

        public OverrideMemberXml<T> XmlAnyAttribute()
        {

            Attributes.XmlAnyAttribute = new XmlAnyAttributeAttribute();
            return this;
        }


        public OverrideMemberXml<T> XmlText()
        {

            Attributes.XmlText = new XmlTextAttribute();
            return this;
        }

        public OverrideMemberXml<T> XmlIgnore(bool bIgnore = true)
        {

            Attributes.XmlIgnore = bIgnore;
            return this;
        }

        public OverrideMemberXml<T> XmlArray() => Attr(new XmlArrayAttribute());


        public OverrideMemberXml<T> XmlArray(string elementName) => Attr(new XmlArrayAttribute(elementName));


        public OverrideMemberXml<T> Attr(XmlArrayAttribute attribute)
        {

            Attributes.XmlArray = attribute;
            return this;
        }

        public OverrideMemberXml<T> XmlArrayItem() => Attr(new XmlArrayItemAttribute());

        public OverrideMemberXml<T> XmlArrayItem(string elementName) => Attr(new XmlArrayItemAttribute(elementName));

        public OverrideMemberXml<T> Attr(XmlArrayItemAttribute attribute)
        {

            Attributes.XmlArrayItems.Add(attribute);
            return this;
        }

        public OverrideMemberXml<T> XmlAttribute() => Attr(new XmlAttributeAttribute());

        public OverrideMemberXml<T> XmlAttribute(string name) => Attr(new XmlAttributeAttribute(name));

        public OverrideMemberXml<T> Attr(XmlAttributeAttribute attribute)
        {

            Attributes.XmlAttribute = attribute;
            return this;
        }

        public OverrideMemberXml<T> XmlElement() => Attr(new XmlElementAttribute());
        

        public OverrideMemberXml<T> XmlElement(string name) => Attr(new XmlElementAttribute(name));
        

        public OverrideMemberXml<T> Attr(XmlElementAttribute attribute)
        {

            Attributes.XmlElements.Add(attribute);
            return this;
        }

        public OverrideMemberXml<T> XmlAnyElement() => Attr(new XmlAnyElementAttribute());

        public OverrideMemberXml<T> XmlAnyElement(string name) => Attr(new XmlAnyElementAttribute(name));

        public OverrideMemberXml<T> XmlAnyElement(string name, string ns) => Attr(new XmlAnyElementAttribute(name, ns));

        public OverrideMemberXml<T> Attr(XmlAnyElementAttribute attribute)
        {

            Attributes.XmlAnyElements.Add(attribute);
            return this;
        }

    }

    public class OverrideXml
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

}
