/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>base class for IoAdapters that delegate to other IoAdapters (decorator pattern)
	/// 	</summary>
	public abstract class VanillaIoAdapter : IoAdapter
	{
		protected IoAdapter _delegate;

		public VanillaIoAdapter(IoAdapter delegateAdapter)
		{
			_delegate = delegateAdapter;
		}

		protected VanillaIoAdapter(IoAdapter delegateAdapter, string path, bool lockFile, 
			long initialLength)
		{
			_delegate = delegateAdapter.Open(path, lockFile, initialLength);
		}

		public override void Close()
		{
			_delegate.Close();
		}

		public override void Delete(string path)
		{
			_delegate.Delete(path);
		}

		public override bool Exists(string path)
		{
			return _delegate.Exists(path);
		}

		public override long GetLength()
		{
			return _delegate.GetLength();
		}

		public override int Read(byte[] bytes, int length)
		{
			return _delegate.Read(bytes, length);
		}

		public override void Seek(long pos)
		{
			_delegate.Seek(pos);
		}

		public override void Sync()
		{
			_delegate.Sync();
		}

		public override void Write(byte[] buffer, int length)
		{
			_delegate.Write(buffer, length);
		}
	}
}
