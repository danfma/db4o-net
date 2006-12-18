namespace Db4objects.Db4o.Tutorial.F1.Chapter21
{
	public class Pilot
	{
		string _name;
		int _points;
        
		public Pilot(string name, int points)
		{
			_name = name;
			_points = points;
		}
        
		public int Points
		{
			get
			{
				return _points;
			}
		}
        
		public void AddPoints(int points)
		{
			_points += points;
		}
        
		public string Name
		{
			get
			{
				return _name;
			}
		}
        
		override public string ToString()
		{
			return string.Format("{0}/{1}", _name, _points);
		}
	}
}