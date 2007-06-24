using System;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

using Db4objects.Db4o.Tutorial;

namespace Db4objects.Db4o.Tutorial.F1.Chapter1
{
    public class QueryExample : Util
    {
        public static void Main(string[] args)
        {
            IObjectContainer db = Db4oFactory.OpenFile(Util.YapFileName);
            try
            {
                StoreFirstPilot(db);
                StoreSecondPilot(db);
                RetrieveAllPilots(db);
                RetrievePilotByName(db);
                RetrievePilotByExactPoints(db);
                RetrieveByNegation(db);
                RetrieveByConjunction(db);
                RetrieveByDisjunction(db);
                RetrieveByComparison(db);
                RetrieveByDefaultFieldValue(db);
                RetrieveSorted(db); 
                ClearDatabase(db);
            }
            finally
            {
                db.Close();
            }
        }
    
        public static void StoreFirstPilot(IObjectContainer db)
        {
            Pilot pilot1 = new Pilot("Michael Schumacher", 100);
            db.Set(pilot1);
            Console.WriteLine("Stored {0}", pilot1);
        }
    
        public static void StoreSecondPilot(IObjectContainer db)
        {
            Pilot pilot2 = new Pilot("Rubens Barrichello", 99);
            db.Set(pilot2);
            Console.WriteLine("Stored {0}", pilot2);
        }
    
        public static void RetrieveAllPilots(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrievePilotByName(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_name").Constrain("Michael Schumacher");
            IObjectSet result = query.Execute();
            ListResult(result);
        }
        
        public static void RetrievePilotByExactPoints(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_points").Constrain(100);
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrieveByNegation(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_name").Constrain("Michael Schumacher").Not();
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrieveByConjunction(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            IConstraint constr = query.Descend("_name")
                    .Constrain("Michael Schumacher");
            query.Descend("_points")
                    .Constrain(99).And(constr);
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrieveByDisjunction(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            IConstraint constr = query.Descend("_name")
                    .Constrain("Michael Schumacher");
            query.Descend("_points")
                    .Constrain(99).Or(constr);
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrieveByComparison(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_points")
                    .Constrain(99).Greater();
            IObjectSet result = query.Execute();
            ListResult(result);
        }
    
        public static void RetrieveByDefaultFieldValue(IObjectContainer db)
        {
            Pilot somebody = new Pilot("Somebody else", 0);
            db.Set(somebody);
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_points").Constrain(0);
            IObjectSet result = query.Execute();
            ListResult(result);
            db.Delete(somebody);
        }
        
        public static void RetrieveSorted(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(Pilot));
            query.Descend("_name").OrderAscending();
            IObjectSet result = query.Execute();
            ListResult(result);
            query.Descend("_name").OrderDescending();
            result = query.Execute();
            ListResult(result);
        }
    
        public static void ClearDatabase(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(Pilot));
            foreach (object item in result)
            {
                db.Delete(item);
            }
        }
    }
}
