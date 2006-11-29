namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QField : Db4objects.Db4o.Foundation.IVisitor4, Db4objects.Db4o.Types.IUnversioned
	{
		[System.NonSerialized]
		internal Db4objects.Db4o.Transaction i_trans;

		public string i_name;

		[System.NonSerialized]
		internal Db4objects.Db4o.YapField i_yapField;

		public int i_yapClassID;

		public int i_index;

		public QField()
		{
		}

		internal QField(Db4objects.Db4o.Transaction a_trans, string name, Db4objects.Db4o.YapField
			 a_yapField, int a_yapClassID, int a_index)
		{
			i_trans = a_trans;
			i_name = name;
			i_yapField = a_yapField;
			i_yapClassID = a_yapClassID;
			i_index = a_index;
			if (i_yapField != null)
			{
				if (!i_yapField.Alive())
				{
					i_yapField = null;
				}
			}
		}

		internal virtual bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return i_yapField == null || i_yapField.CanHold(claxx);
		}

		internal virtual object Coerce(object a_object)
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx = null;
			Db4objects.Db4o.Reflect.IReflector reflector = i_trans.Reflector();
			if (a_object != null)
			{
				if (a_object is Db4objects.Db4o.Reflect.IReflectClass)
				{
					claxx = (Db4objects.Db4o.Reflect.IReflectClass)a_object;
				}
				else
				{
					claxx = reflector.ForObject(a_object);
				}
			}
			else
			{
				return a_object;
			}
			if (i_yapField == null)
			{
				return a_object;
			}
			return i_yapField.Coerce(claxx, a_object);
		}

		internal virtual Db4objects.Db4o.YapClass GetYapClass()
		{
			if (i_yapField != null)
			{
				return i_yapField.GetFieldYapClass(i_trans.Stream());
			}
			return null;
		}

		internal virtual Db4objects.Db4o.YapField GetYapField(Db4objects.Db4o.YapClass yc
			)
		{
			if (i_yapField != null)
			{
				return i_yapField;
			}
			Db4objects.Db4o.YapField yf = yc.GetYapField(i_name);
			if (yf != null)
			{
				yf.Alive();
			}
			return yf;
		}

		public virtual Db4objects.Db4o.YapField GetYapField()
		{
			return i_yapField;
		}

		internal virtual bool IsArray()
		{
			return i_yapField != null && i_yapField.GetHandler() is Db4objects.Db4o.YapArray;
		}

		internal virtual bool IsClass()
		{
			return i_yapField == null || i_yapField.GetHandler().GetTypeID() == Db4objects.Db4o.YapConst
				.TYPE_CLASS;
		}

		internal virtual bool IsSimple()
		{
			return i_yapField != null && i_yapField.GetHandler().GetTypeID() == Db4objects.Db4o.YapConst
				.TYPE_SIMPLE;
		}

		internal virtual Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			if (i_yapField != null)
			{
				return i_yapField.PrepareComparison(obj);
			}
			if (obj == null)
			{
				return Db4objects.Db4o.Null.INSTANCE;
			}
			Db4objects.Db4o.YapClass yc = i_trans.Stream().GetYapClass(i_trans.Reflector().ForObject
				(obj), true);
			Db4objects.Db4o.YapField yf = yc.GetYapField(i_name);
			if (yf != null)
			{
				return yf.PrepareComparison(obj);
			}
			return null;
		}

		internal virtual void Unmarshall(Db4objects.Db4o.Transaction a_trans)
		{
			if (i_yapClassID != 0)
			{
				Db4objects.Db4o.YapClass yc = a_trans.Stream().GetYapClass(i_yapClassID);
				i_yapField = yc.i_fields[i_index];
			}
		}

		public virtual void Visit(object obj)
		{
			((Db4objects.Db4o.QCandidate)obj).UseField(this);
		}
	}
}
