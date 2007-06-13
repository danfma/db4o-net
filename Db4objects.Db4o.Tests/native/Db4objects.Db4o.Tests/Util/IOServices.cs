/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Db4objects.Db4o.Tests.Util
{
	class IOServices
	{
		public static string FindParentDirectory(string path)
		{
#if !CF_1_0 && !CF_2_0
			string parent = Path.GetFullPath("..");
			while (true)
			{
				if (Directory.Exists(Path.Combine(parent, path))) return parent;
				string oldParent = parent;
				parent = Path.GetDirectoryName(parent);
				if (parent == oldParent || parent == null) break;
			}
#endif
			return null;
		}

		public static void WriteFile(string fname, string contents)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fname));
			using (StreamWriter writer = new StreamWriter(fname))
			{
				writer.Write(contents);
			}
		}

        public static string JoinQuotedArgs(string[] args)
        {
            return JoinQuotedArgs(' ', args);
        }

        public static string JoinQuotedArgs(char separator, params string[] args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string arg in args)
            {
                if (builder.Length > 0) builder.Append(separator);
                builder.Append(Quote(arg));
            }
            return builder.ToString();
        }

        public static string Quote(string s)
        {
            if (s.StartsWith("\"")) return s;
            return "\"" + s + "\"";
        }
		
#if !CF_1_0 && !CF_2_0
		public static string Exec(string program, params string[] arguments)
		{
			return Exec(program, JoinQuotedArgs(arguments));
		}

		private static string Exec(string program, string arguments)
		{
			ProcessStartInfo psi = new ProcessStartInfo(program);
			psi.UseShellExecute = false;
			psi.Arguments = arguments;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.WorkingDirectory = Path.GetTempPath();
			psi.CreateNoWindow = true;

			Process p = Process.Start(psi);
			string stdout = p.StandardOutput.ReadToEnd();
			string stderr = p.StandardError.ReadToEnd();
			p.WaitForExit();
            if (p.ExitCode != 0) throw new ApplicationException(stdout + stderr);
			return stdout + stderr;
		}
#endif

		public static string BuildTempPath(string fname)
		{
			return Path.Combine(Path.GetTempPath(), fname);
		}
	}
}
