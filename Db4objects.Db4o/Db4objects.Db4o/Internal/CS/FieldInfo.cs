namespace Db4objects.Db4o.Internal.CS
{
	public class FieldInfo
	{
		public string _fieldName;

		public Db4objects.Db4o.Internal.CS.ClassInfo _fieldClass;

		public bool _isPrimitive;

		public bool _isArray;

		public bool _isNArray;

		public FieldInfo()
		{
		}

		public FieldInfo(string fieldName, Db4objects.Db4o.Internal.CS.ClassInfo fieldClass
			, bool isPrimitive, bool isArray, bool isNArray)
		{
			_fieldName = fieldName;
			_fieldClass = fieldClass;
			_isPrimitive = isPrimitive;
			_isArray = isArray;
			_isNArray = isNArray;
		}

		public virtual Db4objects.Db4o.Internal.CS.ClassInfo GetFieldClass()
		{
			return _fieldClass;
		}

		public virtual string GetFieldName()
		{
			return _fieldName;
		}
	}
}