using System.Collections;
using System.IO;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestFailureCollection : Printable
	{
		internal ArrayList _failures = new ArrayList();

		public virtual IEnumerator Iterator()
		{
			return _failures.GetEnumerator();
		}

		public virtual int Size()
		{
			return _failures.Count;
		}

		public virtual void Add(TestFailure failure)
		{
			_failures.Add(failure);
		}

		public override void Print(TextWriter writer)
		{
			PrintSummary(writer);
			PrintDetails(writer);
		}

		private void PrintSummary(TextWriter writer)
		{
			int index = 1;
			IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write(index.ToString());
				writer.Write(") ");
				writer.Write(((TestFailure)e.Current).GetTest().GetLabel());
				writer.Write("\n");
				++index;
			}
		}

		private void PrintDetails(TextWriter writer)
		{
			int index = 1;
			IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write("\n");
				writer.Write(index.ToString());
				writer.Write(") ");
				((Printable)e.Current).Print(writer);
				writer.Write("\n");
				++index;
			}
		}
	}
}
