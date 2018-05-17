namespace Mvp.Xml.Tests.UpperLowerTests
{
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml-customer")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="mvp-xml-customer", IsNullable=false)]
    public class Customer
    {
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public CustomerOrder Order;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml-customer")]
    public class CustomerOrder
    {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Id;
    }
}
