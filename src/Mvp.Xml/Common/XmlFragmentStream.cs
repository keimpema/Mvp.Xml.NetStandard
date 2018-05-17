using System;
using System.IO;
using System.Text;

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Allows streams without a root element (i.e. multiple document 
	/// fragments) to be passed to an <see cref="System.Xml.XmlReader"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is obsolete. Use <see cref="XmlFragmentReader"/> instead.
	/// </para>
	/// A faked root element is added at the stream 
	/// level to enclose the fragments, which can be customized 
	/// using the overloaded constructors.
	/// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
	/// See: http://weblogs.asp.net/cazzu/archive/2004/04/23/119263.aspx.
	/// </remarks>
	[Obsolete("Use XmlFragmentReader instead.", false)]
	public class XmlFragmentStream : Stream
	{
	    // Holds the inner stream with the XML fragments.
	    private readonly Stream stream;

	    private bool first = true;
	    private bool done;
	    private bool eof;

		// TODO: there's a potential encoding issue here.
	    private readonly byte[] rootstart = Encoding.UTF8.GetBytes("<root>");
	    private readonly byte[] rootend = Encoding.UTF8.GetBytes("</root>");
	    private int endidx = -1;

	    /// <summary>
		/// Initializes the class with the underlying stream to use, and 
		/// uses the default &lt;root&gt; container element. 
		/// </summary>
		/// <param name="innerStream">The stream to read from.</param>
		public XmlFragmentStream(Stream innerStream)
		{
		    stream = innerStream ?? throw new ArgumentNullException("innerStream");
		}

		/// <summary>
		/// Initializes the class with the underlying stream to use, with 
		/// a custom root element. 
		/// </summary>
		/// <param name="innerStream">The stream to read from.</param>
		/// <param name="rootName">Custom root element name to use.</param>
		public XmlFragmentStream(Stream innerStream, string rootName)
			: this(innerStream)
		{
			rootstart = Encoding.UTF8.GetBytes("<" + rootName + ">");
			rootend = Encoding.UTF8.GetBytes("</" + rootName + ">");
		}

		/// <summary>
		/// Initializes the class with the underlying stream to use, with 
		/// a custom root element. 
		/// </summary>
		/// <param name="innerStream">The stream to read from.</param>
		/// <param name="rootName">Custom root element name to use.</param>
		/// <param name="ns">The namespace of the root element.</param>
		public XmlFragmentStream(Stream innerStream, string rootName, string ns)
			: this(innerStream)
		{
			rootstart = Encoding.UTF8.GetBytes("<" + rootName + " xmlns=\"" + ns + "\">");
			rootend = Encoding.UTF8.GetBytes("</" + rootName + ">");
		}

	    /// <summary>See <see cref="Stream.Close"/>.</summary>
		public override void Close()
		{
			stream.Close();
			base.Close();
		}

		/// <summary>See <see cref="Stream.Flush"/>.</summary>
		public override void Flush()
		{
			stream.Flush();
		}

		/// <summary>See <see cref="Stream.Seek"/>.</summary>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		/// <summary>See <see cref="Stream.SetLength"/>.</summary>
		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		/// <summary>See <see cref="Stream.Write"/>.</summary>
		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		/// <summary>See <see cref="Stream.CanRead"/>.</summary>
		public override bool CanRead => stream.CanRead;

	    /// <summary>See <see cref="Stream.CanSeek"/>.</summary>
		public override bool CanSeek => stream.CanSeek;

	    /// <summary>See <see cref="Stream.CanWrite"/>.</summary>
		public override bool CanWrite => stream.CanWrite;

	    /// <summary>See <see cref="Stream.Length"/>.</summary>
		public override long Length => stream.Length;

	    /// <summary>See <see cref="Stream.Position"/>.</summary>
		public override long Position
		{
			get => stream.Position;
	        set => stream.Position = value;
	    }

	    /// <summary>See <see cref="Stream.Read"/>.</summary>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (done)
			{
			    if (eof)
			    {
			        throw new EndOfStreamException(Properties.Resources.XmlFragmentStream_EOF);
			    }

			    eof = true;
			    return 0;

			}

			// If this is the first one, return the wrapper root element.
			if (first)
			{
				rootstart.CopyTo(buffer, 0);

				stream.Read(buffer, rootstart.Length, count - rootstart.Length);

				first = false;
				return count;
			}

			// We have a pending closing wrapper root element.
			if (endidx != -1)
			{
				for (int i = endidx; i < rootend.Length; i++)
				{
					buffer[i] = rootend[i];
				}
				return rootend.Length - endidx;
			}

			int ret = stream.Read(buffer, offset, count);

			// Did we reached the end?
		    if (ret >= count)
		    {
		        return ret;
		    }

		    rootend.CopyTo(buffer, ret);
		    if (count - ret > rootend.Length)
		    {
		        done = true;
		        return ret + rootend.Length;
		    }

		    endidx = count - ret;
		    return count;
		}
	}
}
