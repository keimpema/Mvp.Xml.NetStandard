using System;
using System.Text;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization
{
	/// <summary>
	/// The CacheKeyFactory extracts a unique signature
	/// to identify each instance of an XmlSerializer
	/// in the cache.
	/// </summary>
	public static class CacheKeyFactory
	{

		/// <summary>
		/// Creates a unique signature for the passed
		/// in parameter. MakeKey normalizes array content
		/// and the content of the XmlAttributeOverrides before
		/// creating the key.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrides"></param>
		/// <param name="types"></param>
		/// <param name="root"></param>
		/// <param name="defaultNamespace"></param>
		/// <returns></returns>
		public static string MakeKey(Type type
			, XmlAttributeOverrides overrides
			, Type[] types
			, XmlRootAttribute root
			, string defaultNamespace)
		{
			var keyBuilder = new StringBuilder();
			keyBuilder.Append(type.FullName);
			keyBuilder.Append("??");
			keyBuilder.Append(SignatureExtractor.GetOverridesSignature(overrides));
			keyBuilder.Append("??");
			keyBuilder.Append(SignatureExtractor.GetTypeArraySignature(types));
			keyBuilder.Append("??");
			keyBuilder.Append(SignatureExtractor.GetXmlRootSignature(root));
			keyBuilder.Append("??");
			keyBuilder.Append(SignatureExtractor.GetDefaultNamespaceSignature(defaultNamespace));

			return keyBuilder.ToString();
		}
	}
}
