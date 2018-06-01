using System.Collections.Generic;

namespace Mvp.Xml.XPointer
{
	/// <summary>
	/// XPointer scheme.
	/// </summary>
	internal class XPointerSchema
	{
	    public enum SchemaType
		{
			Element,
			Xmlns,
			XPath1,
			XPointer,
			Unknown
		}
		public static readonly IDictionary<string, SchemaType> Schemas = CreateSchemasTable();

		private static IDictionary<string, SchemaType> CreateSchemasTable()
		{
			IDictionary<string, SchemaType> table = new Dictionary<string, SchemaType>(4);
			//<namespace uri>:<ncname>
			table.Add(":element", SchemaType.Element);
			table.Add(":xmlns", SchemaType.Xmlns);
			table.Add(":xpath1", SchemaType.XPath1);
			table.Add(":xpointer", SchemaType.XPointer);
			return table;
		}
	}
}
