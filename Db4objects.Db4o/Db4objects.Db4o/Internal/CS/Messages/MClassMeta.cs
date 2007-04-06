using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MClassMeta : MsgObject, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			ObjectContainerBase stream = Stream();
			Unmarshall();
			try
			{
				ClassInfo classMeta = (ClassInfo)ReadObjectFromPayLoad();
				GenericClass genericClass = stream.GetClassMetaHelper().ClassMetaToGenericClass(Stream
					().Reflector(), classMeta);
				if (genericClass != null)
				{
					lock (StreamLock())
					{
						Transaction trans = stream.SystemTransaction();
						ClassMetadata yapClass = stream.ProduceClassMetadata(genericClass);
						if (yapClass != null)
						{
							stream.CheckStillToSet();
							yapClass.SetStateDirty();
							yapClass.Write(trans);
							trans.Commit();
							StatefulBuffer returnBytes = stream.ReadWriterByID(trans, yapClass.GetID());
							Write(Msg.OBJECT_TO_CLIENT.GetWriter(returnBytes));
							return true;
						}
					}
				}
			}
			catch (Exception e)
			{
			}
			Write(Msg.FAILED);
			return true;
		}
	}
}
