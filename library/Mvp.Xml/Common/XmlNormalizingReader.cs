using System.Xml;

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Reader that only exposes xmlns attribute declarations if they 
	/// have not been previously declared by a parent element, 
	/// normalizing the output XML so that no duplicate namespace 
	/// declarations exist.
	/// </summary>
	public class XmlNormalizingReader : XmlWrappingReader
	{
	    private readonly XmlNamespaceManager nsManager;
	    private readonly string xmlNsNamespace;

		/// <summary>
		/// Initializes the normalizing reader with the 
		/// underlying reader to use.
		/// </summary>
		/// <param name="baseReader">Underlying reader to normalize.</param>
		public XmlNormalizingReader(XmlReader baseReader)
			: base(baseReader)
		{
			nsManager = new XmlNamespaceManager(baseReader.NameTable);
			xmlNsNamespace = nsManager.LookupNamespace("xmlns");
		}

		/// <summary>
		/// See <see cref="XmlReader.Read"/>.
		/// </summary>
		public override bool Read()
		{
			bool read = base.Read();

			if (NodeType == XmlNodeType.Element)
			{
				nsManager.PushScope();
				for (bool go = BaseReader.MoveToFirstAttribute(); go; go = BaseReader.MoveToNextAttribute())
				{
				    if (BaseReader.NamespaceURI != xmlNsNamespace)
				    {
				        continue;
				    }

				    string prefix = GetNamespacePrefix();

				    // Only push if it's not already defined.
				    if (nsManager.LookupNamespace(prefix) == null)
				    {
				        nsManager.AddNamespace(prefix, Value);
				    }
				}

				// If it had attributes, we surely moved through all of them searching for namespaces
				if (BaseReader.HasAttributes)
				{
					BaseReader.MoveToElement();
				}
			}
			else if (NodeType == XmlNodeType.EndElement)
			{
				nsManager.PopScope();
			}

			return read;
		}

		/// <summary>
		/// See <see cref="XmlReader.AttributeCount"/>.
		/// </summary>
		public override int AttributeCount
		{
			get
			{
				int count = 0;
				for (bool go = MoveToFirstAttribute(); go; go = MoveToNextAttribute())
				{
					count++;
				}

				return count;
			}
		}

		/// <summary>
		/// See <see cref="XmlReader.MoveToFirstAttribute"/>.
		/// </summary>
		public override bool MoveToFirstAttribute()
		{
			bool moved = base.MoveToFirstAttribute();
			while (moved && IsXmlNs && !IsLocalXmlNs)
			{
				moved = MoveToNextAttribute();
			}

			if (!moved)
			{
			    MoveToElement();
			}

		    return moved;
		}

		/// <summary>
		/// See <see cref="XmlReader.MoveToNextAttribute"/>.
		/// </summary>
		public override bool MoveToNextAttribute()
		{
			bool moved = base.MoveToNextAttribute();
			while (moved && IsXmlNs && !IsLocalXmlNs)
			{
				moved = MoveToNextAttribute();
			}

			return moved;
		}

		private bool IsXmlNs => NamespaceURI == xmlNsNamespace;

	    private bool IsLocalXmlNs => nsManager.GetNamespacesInScope(XmlNamespaceScope.Local).ContainsKey(GetNamespacePrefix());

	    private string GetNamespacePrefix()
		{
			// This is not very intuitive, but it's how it works.
			// In the first case, a non-empty prefix is represented 
			// as an xmlns:foo="bar" declaration, where xmlns is the 
			// actual attribute prefix, and where the real prefix 
			// being declared is the reader localname (foo in this case).
			// If no prefix is being declared for the namespace, 
			// it's an xmlns="foo" declaration, therefore we pass empty string.
			return Prefix.Length > 0 ? LocalName : string.Empty;
		}
	}
}
