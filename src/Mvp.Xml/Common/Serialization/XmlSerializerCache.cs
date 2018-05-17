using System;
using System.Collections.Generic;
using System.Xml.Serialization;
//using System.Management.Instrumentation;

namespace Mvp.Xml.Common.Serialization
{
	/// <summary>
	/// Delegete type for events to raise from the
	/// serializer cache.
	/// </summary>
	public delegate void SerializerCacheDelegate(Type type
		, XmlAttributeOverrides overrides
		, Type[] types
		, XmlRootAttribute root
		, string defaultNamespace);

	/// <summary>
	/// The XmlSerializerCache allows to work around the 
	/// assembly leak problem in the <see cref="XmlSerializer"/> 
	/// ( LINK )
	/// The cache will inspect if it contains any previously cached 
	/// instances that are compatible with the parameters passed to the
	/// various overloads to the GetSerializer method before constructing 
	/// a new XmlSerializer instance.
	/// </summary>
	/// <remarks>
	/// In contrast to the <see cref="XmlSerializer"/>, the XmlSerializerCache requires
	/// a permission set that allows reflecting over private members.
	/// </remarks>
	public class XmlSerializerCache : IDisposable
	{

		/// <summary>
		/// The NewSerializer event fires when the XmlSerializerCache
		/// receives a request for an XmlSerializer instance
		/// that is not in the cache and it needs to create a
		/// new instance.
		/// </summary>
		public event SerializerCacheDelegate NewSerializer;

		/// <summary>
		/// The CacheHit even fires when the XmlSerializerCache
		/// receives a request for a previously cached instance
		/// of an XmlSerializer
		/// </summary>
		public event SerializerCacheDelegate CacheHit;

		/// <summary>
		/// The Dictionary to store cached serializer instances.
		/// </summary>
		private readonly Dictionary<string, XmlSerializer> serializers;

		/// <summary>
		/// An object to synchonize access to the Dictionary instance.
		/// </summary>
		private readonly object syncRoot;

        //private PerfCounterManager stats;

		/// <summary>
		/// Default constructor to initialize
		/// an instance of the cache.
		/// </summary>
		public XmlSerializerCache()
		{
			syncRoot = new object();
            //stats = new PerfCounterManager();
			serializers = new Dictionary<string, XmlSerializer>();
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="defaultNamespace"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, string defaultNamespace)
		{
			return GetSerializer(type, null, new Type[0], null, defaultNamespace);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, XmlRootAttribute root)
		{
			return GetSerializer(type, null, new Type[0], root, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides)
		{
			return GetSerializer(type
				, overrides
				, new Type[0]
				, null
				, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type, Type[] types)
		{
			return GetSerializer(type
				, null
				, types
				, null
				, null);
		}

		/// <summary>
		/// Get an XmlSerializer instance for the
		/// specified parameters. The method will check if
		/// any any previously cached instances are compatible
		/// with the parameters before constructing a new  
		/// XmlSerializer instance.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrides"></param>
		/// <param name="types"></param>
		/// <param name="root"></param>
		/// <param name="defaultNamespace"></param>
		/// <returns></returns>
		public XmlSerializer GetSerializer(Type type
			, XmlAttributeOverrides overrides
			, Type[] types
			, XmlRootAttribute root
			, string defaultNamespace)
		{
			string key = CacheKeyFactory.MakeKey(type
				, overrides
				, types
				, root
				, defaultNamespace);

			XmlSerializer serializer = null;
			if (!serializers.ContainsKey(key))
			{
				lock (syncRoot)
				{
					if (!serializers.ContainsKey(key))
					{
                        //stats.IncrementInstanceCount();

						serializer = new XmlSerializer(type
							, overrides
							, types
							, root
							, defaultNamespace);
						serializers.Add(key, serializer);

					    NewSerializer?.Invoke(type
					        , overrides
					        , types
					        , root
					        , defaultNamespace);
					} // if( null == Serializers[key] )
				} // lock
			} // if( null == Serializers[key] )
			else
			{
				serializer = serializers[key];
                //stats.IncrementHitCount();
				// Tell the listeners that we already 
				// had a serializer that matched the attributes
			    CacheHit?.Invoke(type
			        , overrides
			        , types
			        , root
			        , defaultNamespace);
			}

			System.Diagnostics.Debug.Assert(null != serializer);
			return serializer;
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~XmlSerializerCache()
		{
			Dispose(false);
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
                //stats.Dispose();
			}
		}

	    /// <summary>
		/// Implementation of IDisposable.Dispose. Call to 
		/// clean up resources held by the XmlSerializerCache.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
