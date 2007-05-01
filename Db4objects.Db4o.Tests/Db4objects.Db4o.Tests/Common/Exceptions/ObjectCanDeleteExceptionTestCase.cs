using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ObjectCanDeleteExceptionTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public static void Main(string[] args)
		{
			new ObjectCanDeleteExceptionTestCase().RunSolo();
		}

		public class Item
		{
			public virtual bool ObjectCanDelete(IObjectContainer container)
			{
				throw new ItemException();
			}
		}

		public virtual void Test()
		{
			ObjectCanDeleteExceptionTestCase.Item item = new ObjectCanDeleteExceptionTestCase.Item
				();
			Store(item);
			Assert.Expect(typeof(ReflectException), typeof(ItemException), new _AnonymousInnerClass27
				(this, item));
		}

		private sealed class _AnonymousInnerClass27 : ICodeBlock
		{
			public _AnonymousInnerClass27(ObjectCanDeleteExceptionTestCase _enclosing, ObjectCanDeleteExceptionTestCase.Item
				 item)
			{
				this._enclosing = _enclosing;
				this.item = item;
			}

			public void Run()
			{
				this._enclosing.Db().Delete(item);
				this._enclosing.Db().Commit();
			}

			private readonly ObjectCanDeleteExceptionTestCase _enclosing;

			private readonly ObjectCanDeleteExceptionTestCase.Item item;
		}
	}
}