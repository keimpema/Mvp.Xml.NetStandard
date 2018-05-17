namespace Mvp.Xml.Tests
{
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="mvp-xml", IsNullable=false)]
    public class dsPubs
    {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("publishers")]
        public dsPubsPublishers[] publishers;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml")]
    public class dsPubsPublishers
    {
        
        /// <remarks/>
        public System.UInt16 pub_id;
        
        /// <remarks/>
        public string pub_name;
        
        /// <remarks/>
        public string city;
        
        /// <remarks/>
        public string state;
        
        /// <remarks/>
        public string country;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("titles")]
        public dsPubsPublishersTitles[] titles;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml")]
    public class dsPubsPublishersTitles
    {
        
        /// <remarks/>
        public string title_id;
        
        /// <remarks/>
        public string title;
        
        /// <remarks/>
        public string type;
        
        /// <remarks/>
        public System.UInt16 pub_id;
        
        /// <remarks/>
        public System.Decimal price;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool priceSpecified;
        
        /// <remarks/>
        public System.UInt16 advance;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool advanceSpecified;
        
        /// <remarks/>
        public System.Byte royalty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool royaltySpecified;
        
        /// <remarks/>
        public System.UInt16 ytd_sales;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ytd_salesSpecified;
        
        /// <remarks/>
        public string notes;
        
        /// <remarks/>
        public System.DateTime pubdate;
    }
}
