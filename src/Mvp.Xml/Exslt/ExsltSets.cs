using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml;
//using System.Web.UI;

using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	///   This class implements the EXSLT functions in the http://exslt.org/sets namespace.
	/// </summary>			
	public class ExsltSets
	{

		/// <summary>
		/// Implements the following function 
		///    node-set difference(node-set, node-set) 
		/// </summary>
		/// <param name="nodeset1">An input nodeset</param>
		/// <param name="nodeset2">Another input nodeset</param>
		/// <returns>The those nodes that are in the node set 
		/// passed as the first argument that are not in the node set 
		/// passed as the second argument.</returns>
		public XPathNodeIterator Difference(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			if (nodeset2.Count > 166)
			{
			    return Difference2(nodeset1, nodeset2);
			}

		    //else
			XPathNavigatorIterator nodelist1 = new XPathNavigatorIterator(nodeset1, true);
			XPathNavigatorIterator nodelist2 = new XPathNavigatorIterator(nodeset2);

			for (int i = 0; i < nodelist1.Count; i++)
			{
				XPathNavigator nav = nodelist1[i];

				if (nodelist2.Contains(nav))
				{
					nodelist1.RemoveAt(i);
					i--;
				}
			}

			nodelist1.Reset();
			return nodelist1;
		}

	    public XPathNodeIterator difference(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2) => Difference(nodeset1, nodeset2);

        /// <summary>
        /// Implements an optimized algorithm for the following function 
        ///    node-set difference(node-set, node-set) 
        /// Uses document identification and binary search,
        /// based on the fact that a node-set is always in document order.
        /// </summary>
        /// <param name="nodeset1">An input nodeset</param>
        /// <param name="nodeset2">Another input nodeset</param>
        /// <returns>The those nodes that are in the node set 
        /// passed as the first argument that are not in the node set 
        /// passed as the second argument.</returns>
        /// <author>Dimitre Novatchev</author>

        private XPathNodeIterator Difference2(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			List<DocPair> arDocs = new List<DocPair>();

			List<XPathNavigator> arNodes2 = new List<XPathNavigator>(nodeset2.Count);

			while (nodeset2.MoveNext())
			{
				arNodes2.Add(nodeset2.Current.Clone());
			}

			AuxExslt.FindDocs(arNodes2, arDocs);

			XPathNavigatorIterator enlResult = new XPathNavigatorIterator();

			while (nodeset1.MoveNext())
			{
				XPathNavigator currNode = nodeset1.Current;

				if (!AuxExslt.FindNode(arNodes2, arDocs, currNode))
				{
				    enlResult.Add(currNode.Clone());
				}
			}

			enlResult.Reset();
			return enlResult;
		}


		/// <summary>
		/// Implements the following function 
		///    node-set distinct(node-set)
		/// </summary>
		/// <param name="nodeset">The input nodeset</param>
		/// <returns>Returns the nodes in the nodeset whose string value is 
		/// distinct</returns>
		public XPathNodeIterator Distinct(XPathNodeIterator nodeset)
		{
			if (nodeset.Count > 15)
			{
			    return Distinct2(nodeset);
			}

		    //else
			XPathNavigatorIterator nodelist = new XPathNavigatorIterator();

			while (nodeset.MoveNext())
			{
				if (!nodelist.ContainsValue(nodeset.Current.Value))
				{
					nodelist.Add(nodeset.Current.Clone());
				}
			}
			nodelist.Reset();
			return nodelist;
		}

	    public XPathNodeIterator distinct(XPathNodeIterator nodeset) => Distinct(nodeset);

        /// <summary>
        /// Implements the following function 
        /// node-set distinct2(node-set)
        /// 
        /// This implements an optimized algorithm
        /// using a Hashtable
        ///    
        /// </summary>
        /// <param name="nodeset">The input nodeset</param>
        /// <returns>Returns the nodes in the nodeset whose string value is 
        /// distinct</returns>
        /// <author>Dimitre Novatchev</author>
        private XPathNodeIterator Distinct2(XPathNodeIterator nodeset)
		{

			XPathNavigatorIterator nodelist = new XPathNavigatorIterator();

			Dictionary<string, string> ht = new Dictionary<string, string>(nodeset.Count / 3);

			while (nodeset.MoveNext())
			{
				string strVal = nodeset.Current.Value;

				if (!ht.ContainsKey(strVal))
				{
					ht.Add(strVal, "");

					nodelist.Add(nodeset.Current.Clone());
				}
			}

			nodelist.Reset();
			return nodelist;
		}

		/// <summary>
		/// Implements 
		///    boolean hassamenode(node-set, node-set)
		/// </summary>
		/// <param name="nodeset1"></param>
		/// <param name="nodeset2"></param>
		/// <returns>true if both nodeset contain at least one of the same node</returns>
		public bool HasSameNode(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			if (nodeset1.Count >= 250 || nodeset2.Count >= 250)
			{
			    return HasSameNode2(nodeset1, nodeset2);
			}
		    //else

			XPathNavigatorIterator nodelist1 = new XPathNavigatorIterator(nodeset1, true);
			XPathNavigatorIterator nodelist2 = new XPathNavigatorIterator(nodeset2, true);

			foreach (XPathNavigator nav in nodelist1)
			{
				if (nodelist2.Contains(nav))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Implements 
		///    boolean hassamenode2(node-set, node-set)
		///    
		/// Optimized by using a document identification and
		/// binary search algorithm
		/// </summary>
		/// <param name="nodeset1">The first nodeset</param>
		/// <param name="nodeset2">The second nodeset</param>
		/// <returns>true if both nodeset contain 
		/// at least one of the same node</returns>
		/// <author>Dimitre Novatchev</author>
		private bool HasSameNode2(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			XPathNodeIterator it1 =
				(nodeset1.Count > nodeset2.Count) ? nodeset1
				: nodeset2;

			XPathNodeIterator it2 =
				(nodeset1.Count > nodeset2.Count) ? nodeset2
				: nodeset1;

			List<DocPair> arDocs = new List<DocPair>();

			List<XPathNavigator> arNodes1 = new List<XPathNavigator>(it1.Count);

			while (it1.MoveNext())
			{
				arNodes1.Add(it1.Current.Clone());
			}

			AuxExslt.FindDocs(arNodes1, arDocs);

			XPathNavigatorIterator enlResult = new XPathNavigatorIterator();

			while (it2.MoveNext())
			{
				XPathNavigator currNode = it2.Current;

				if (AuxExslt.FindNode(arNodes1, arDocs, currNode))
				{
				    return true;
				}
			}

			return false;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>        
		public bool hasSameNode_RENAME_ME(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			return HasSameNode(nodeset1, nodeset2);
		}

		/// <summary>
		/// Implements the following function 
		///   node-set intersection(node-set, node-set)
		/// </summary>
		/// <param name="nodeset1">The first node-set</param>
		/// <param name="nodeset2">The second node-set</param>
		/// <returns>The node-set, which is the intersection
		///          of nodeset1 and nodeset2
		/// </returns>
		public XPathNodeIterator Intersection(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{

			if (nodeset1.Count >= 500 || nodeset2.Count >= 500)
			{
			    return Intersection3(nodeset1, nodeset2);
			}

		    //else
			XPathNavigatorIterator nodelist1 = new XPathNavigatorIterator(nodeset1, true);
			XPathNavigatorIterator nodelist2 = new XPathNavigatorIterator(nodeset2);


			for (int i = 0; i < nodelist1.Count; i++)
			{
				XPathNavigator nav = nodelist1[i];

				if (!nodelist2.Contains(nav))
				{
					nodelist1.RemoveAt(i);
					i--;
				}
			}

			nodelist1.Reset();
			return nodelist1;
		}

	    public XPathNodeIterator intersection(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2) => Intersection(nodeset1, nodeset2);

        /// <summary>
        ///   Implements the following function 
        ///   node-set intersection3(node-set, node-set)
        ///
        ///   This is an optimisation of the initial implementation
        ///   of intersection(). It uses a document identification
        ///   and a binary search algorithm, based on the fact
        ///   that a node-set is always in document order.
        /// </summary>
        /// <param name="nodeset1">The first node-set</param>
        /// <param name="nodeset2">The second node-set</param>
        /// <returns>The node-set, which is the intersection
        ///          of nodeset1 and nodeset2
        /// </returns>
        /// <author>Dimitre Novatchev</author>				

        private XPathNodeIterator Intersection3(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			XPathNodeIterator it1 =
				(nodeset1.Count > nodeset2.Count) ? nodeset1
				: nodeset2;

			XPathNodeIterator it2 =
				(nodeset1.Count > nodeset2.Count) ? nodeset2
				: nodeset1;

			List<DocPair> arDocs = new List<DocPair>();

			List<XPathNavigator> arNodes1 = new List<XPathNavigator>(it1.Count);

			while (it1.MoveNext())
			{
				arNodes1.Add(it1.Current.Clone());
			}

			AuxExslt.FindDocs(arNodes1, arDocs);

			XPathNavigatorIterator enlResult = new XPathNavigatorIterator();

			while (it2.MoveNext())
			{
				XPathNavigator currNode = it2.Current;

				if (AuxExslt.FindNode(arNodes1, arDocs, currNode))
				{
				    enlResult.Add(currNode.Clone());
				}
			}

			enlResult.Reset();
			return enlResult;
		}


		/// <summary>
		/// Implements the following function 
		///		node-set leading(node-set, node-set)
		/// </summary>
		/// <param name="nodeset1"></param>
		/// <param name="nodeset2"></param>
		/// <returns>returns the nodes in the node set passed as the 
		/// first argument that precede, in document order, the first node 
		/// in the node set passed as the second argument</returns>
		public XPathNodeIterator Leading(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{
			XPathNavigator leader = null;

			if (nodeset2.MoveNext())
			{
				leader = nodeset2.Current;
			}
			else
			{
				return nodeset1;
			}

			XPathNavigatorIterator nodelist1 = new XPathNavigatorIterator();

			while (nodeset1.MoveNext())
			{
				if (nodeset1.Current.ComparePosition(leader) == XmlNodeOrder.Before)
				{
					nodelist1.Add(nodeset1.Current.Clone());
				}
			}

			nodelist1.Reset();
			return nodelist1;
		}

	    public XPathNodeIterator leading(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2) => Leading(nodeset1, nodeset2);

        /// <summary>
        /// Implements the following function 
        ///		node-set trailing(node-set, node-set)
        /// </summary>
        /// <param name="nodeset1"></param>
        /// <param name="nodeset2"></param>
        /// <returns>returns the nodes in the node set passed as the 
        /// first argument that follow, in document order, the first node 
        /// in the node set passed as the second argument</returns>
        public XPathNodeIterator Trailing(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2)
		{

			XPathNavigator leader = null;

			if (nodeset2.MoveNext())
			{
				leader = nodeset2.Current;
			}
			else
			{
				return nodeset1;
			}

			XPathNavigatorIterator nodelist1 = new XPathNavigatorIterator();

			while (nodeset1.MoveNext())
			{
				if (nodeset1.Current.ComparePosition(leader) == XmlNodeOrder.After)
				{
					nodelist1.Add(nodeset1.Current.Clone());
				}
			}

			nodelist1.Reset();
			return nodelist1;
		}

	    public XPathNodeIterator trailing(XPathNodeIterator nodeset1, XPathNodeIterator nodeset2) => Trailing(nodeset1, nodeset2);

	}


    /// <summary>
    ///   This class implements some auxiliary methods
    ///   which should not be exposed in any namespace
    /// </summary>	
    internal class AuxExslt
	{
		public static bool
			FindNode(List<XPathNavigator> arNodes1, List<DocPair> arDocs, XPathNavigator currNode)
		{
			// 1. First, find the document of this node, return false if not found
			// 2. If the document for the node is found and the node is not an immediate
			//    hit, then look for it using binsearch.

			int start = -1, end = -1, mid;

			foreach (DocPair p in arDocs)
			{
				XmlNodeOrder xOrder =
					arNodes1[p.First].ComparePosition(currNode);

				if (xOrder == XmlNodeOrder.Same)
				{
				    return true;
				}

			    //else
				if (xOrder != XmlNodeOrder.Unknown)
				{
					start = p.First;
					end = p.Second;
					break;
				}
			}

			if (start == -1)
			{
			    return false;
			}
		    //else perform a binary search in the range [start, end]

			while (end >= start)
			{
				mid = (start + end) / 2;

				XmlNodeOrder xOrder =
					arNodes1[mid].ComparePosition(currNode);

				if (xOrder == XmlNodeOrder.Before)
				{
					start = mid + 1;
				}
				else if (xOrder == XmlNodeOrder.After)
				{
					end = mid - 1;
				}
				else
				{
				    return true;
				}
			}

			return false;
		}

		public static void
			FindDocs(List<XPathNavigator> arNodes, List<DocPair> arDocs)
		{
			int count = arNodes.Count;
			int startDoc = 0;
			int endDoc;
			int start;
			int end;
			int mid;

			while (startDoc < count)
			{
				start = startDoc;
				endDoc = count - 1;
				end = endDoc;
				mid = (start + end) / 2;

				while (end > start)
				{
					if (
						arNodes[start].ComparePosition(arNodes[mid])
						==
						XmlNodeOrder.Unknown
						)
					{
						end = mid - 1;
						if (arNodes[start].ComparePosition(arNodes[end])
							!=
							XmlNodeOrder.Unknown)
						{
							endDoc = end;
							break;
						}
					}
					else
					{
						if (arNodes[mid].ComparePosition(arNodes[mid + 1])
							==
							XmlNodeOrder.Unknown)
						{
							endDoc = mid;
							break;
						}
						start = mid + 1;
					}
					mid = (start + end) / 2;
				}

				//here we know startDoc and endDoc
				DocPair docRange = new DocPair(startDoc, endDoc);
				arDocs.Add(docRange);
				startDoc = endDoc + 1;

			}
		}
	}
}
