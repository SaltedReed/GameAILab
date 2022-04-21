using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameAILab.Profile
{
    // ProfileGameAILab
    public class LabProfiler
    {
		public class ProfilePoint
		{
			//public DateTime lastRecorded;
			//public TimeSpan totalTime;
			public System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			public int totalCalls;
			public long tmpBytes;
			public long totalBytes;
		}

		static readonly Dictionary<string, ProfilePoint> profiles = new Dictionary<string, ProfilePoint>();
		static DateTime startTime = DateTime.UtcNow;

		public static ProfilePoint[] fastProfiles;
		public static string[] fastProfileNames;

		private LabProfiler()
		{
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void InitializeFastProfile(string[] profileNames)
		{
			fastProfileNames = new string[profileNames.Length + 2];
			Array.Copy(profileNames, fastProfileNames, profileNames.Length);
			fastProfileNames[fastProfileNames.Length - 2] = "__Control1__";
			fastProfileNames[fastProfileNames.Length - 1] = "__Control2__";
			fastProfiles = new ProfilePoint[fastProfileNames.Length];
			for (int i = 0; i < fastProfiles.Length; i++) fastProfiles[i] = new ProfilePoint();
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void StartFastProfile(int tag)
		{
			//profiles.TryGetValue(tag, out point);
			fastProfiles[tag].watch.Start();//lastRecorded = DateTime.UtcNow;
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void EndFastProfile(int tag)
		{
			/*if (!profiles.ContainsKey(tag))
			 * {
			 *  Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
			 *  return;
			 * }*/
			ProfilePoint point = fastProfiles[tag];

			point.totalCalls++;
			point.watch.Stop();
			//DateTime now = DateTime.UtcNow;
			//point.totalTime += now - point.lastRecorded;
			//fastProfiles[tag] = point;
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void EndProfile()
		{
			Profiler.EndSample();
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void StartProfile(string tag)
		{
#if false
			Profiler.BeginSample(tag);
#else
			//Console.WriteLine ("Profile Start - " + tag);
			ProfilePoint point;

			profiles.TryGetValue(tag, out point);
			if (point == null)
			{
				point = new ProfilePoint();
				profiles[tag] = point;
			}
			point.tmpBytes = GC.GetTotalMemory(false);
			point.watch.Start();
			//point.lastRecorded = DateTime.UtcNow;
			//Debug.Log ("Starting " + tag);
#endif
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void EndProfile(string tag)
		{
#if true
			if (!profiles.ContainsKey(tag))
			{
				Debug.LogError("Can only end profiling for a tag which has already been started (tag was " + tag + ")");
				return;
			}
			//Console.WriteLine ("Profile End - " + tag);
			//DateTime now = DateTime.UtcNow;
			ProfilePoint point = profiles[tag];
			//point.totalTime += now - point.lastRecorded;
			++point.totalCalls;
			point.watch.Stop();
			point.totalBytes += GC.GetTotalMemory(false) - point.tmpBytes;
			//profiles[tag] = point;
			//Debug.Log ("Ending " + tag);
#else
			EndProfile();
#endif
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void Reset()
		{
			profiles.Clear();
			startTime = DateTime.UtcNow;

			if (fastProfiles != null)
			{
				for (int i = 0; i < fastProfiles.Length; i++)
				{
					fastProfiles[i] = new ProfilePoint();
				}
			}
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void PrintFastResults()
		{
			if (fastProfiles == null)
				return;

			StartFastProfile(fastProfiles.Length - 2);
			for (int i = 0; i < 1000; i++)
			{
				StartFastProfile(fastProfiles.Length - 1);
				EndFastProfile(fastProfiles.Length - 1);
			}
			EndFastProfile(fastProfiles.Length - 2);

			double avgOverhead = fastProfiles[fastProfiles.Length - 2].watch.Elapsed.TotalMilliseconds / 1000.0;

			TimeSpan endTime = DateTime.UtcNow - startTime;
			var output = new System.Text.StringBuilder();
			output.Append("============================\n\t\t\t\tProfile results:\n============================\n");
			output.Append("Name		|	Total Time	|	Total Calls	|	Avg/Call	|	Bytes");
			//foreach(KeyValuePair<string, ProfilePoint> pair in profiles)
			for (int i = 0; i < fastProfiles.Length; i++)
			{
				string name = fastProfileNames[i];
				ProfilePoint value = fastProfiles[i];

				int totalCalls = value.totalCalls;
				double totalTime = value.watch.Elapsed.TotalMilliseconds - avgOverhead * totalCalls;

				if (totalCalls < 1) continue;


				output.Append("\n").Append(name.PadLeft(10)).Append("|   ");
				output.Append(totalTime.ToString("0.0 ").PadLeft(10)).Append(value.watch.Elapsed.TotalMilliseconds.ToString("(0.0)").PadLeft(10)).Append("|   ");
				output.Append(totalCalls.ToString().PadLeft(10)).Append("|   ");
				output.Append((totalTime / totalCalls).ToString("0.000").PadLeft(10));


				/* output.Append("\nProfile");
				 * output.Append(name);
				 * output.Append(" took \t");
				 * output.Append(totalTime.ToString("0.0"));
				 * output.Append(" ms to complete over ");
				 * output.Append(totalCalls);
				 * output.Append(" iteration");
				 * if (totalCalls != 1) output.Append("s");
				 * output.Append(", averaging \t");
				 * output.Append((totalTime / totalCalls).ToString("0.000"));
				 * output.Append(" ms per call"); */
			}
			output.Append("\n\n============================\n\t\tTotal runtime: ");
			output.Append(endTime.TotalSeconds.ToString("F3"));
			output.Append(" seconds\n============================");
			Debug.Log(output.ToString());
		}

		[System.Diagnostics.Conditional("ProfileGameAILab")]
		public static void PrintResults()
		{
			TimeSpan endTime = DateTime.UtcNow - startTime;
			var output = new System.Text.StringBuilder();

			output.Append("============================\n\t\t\t\tProfile results:\n============================\n");

			int maxLength = 5;
			foreach (KeyValuePair<string, ProfilePoint> pair in profiles)
			{
				maxLength = Math.Max(pair.Key.Length, maxLength);
			}

			output.Append(" Name ".PadRight(maxLength)).
			Append("|").Append(" Total Time	".PadRight(20)).
			Append("|").Append(" Total Calls ".PadRight(20)).
			Append("|").Append(" Avg/Call ".PadRight(20));



			foreach (var pair in profiles)
			{
				double totalTime = pair.Value.watch.Elapsed.TotalMilliseconds;
				int totalCalls = pair.Value.totalCalls;
				if (totalCalls < 1) continue;

				string name = pair.Key;

				output.Append("\n").Append(name.PadRight(maxLength)).Append("| ");
				output.Append(totalTime.ToString("0.0").PadRight(20)).Append("| ");
				output.Append(totalCalls.ToString().PadRight(20)).Append("| ");
				output.Append((totalTime / totalCalls).ToString("0.000").PadRight(20));
				output.Append(FormatBytesBinary((int)pair.Value.totalBytes).PadLeft(10));

				/*output.Append("\nProfile ");
				 * output.Append(pair.Key);
				 * output.Append(" took ");
				 * output.Append(totalTime.ToString("0"));
				 * output.Append(" ms to complete over ");
				 * output.Append(totalCalls);
				 * output.Append(" iteration");
				 * if (totalCalls != 1) output.Append("s");
				 * output.Append(", averaging ");
				 * output.Append((totalTime / totalCalls).ToString("0.0"));
				 * output.Append(" ms per call");*/
			}
			output.Append("\n\n============================\n\t\tTotal runtime: ");
			output.Append(endTime.TotalSeconds.ToString("F3"));
			output.Append(" seconds\n============================");
			Debug.Log(output.ToString());
		}

		public static string FormatBytesBinary(int bytes)
		{
			double sign = bytes >= 0 ? 1D : -1D;

			bytes = Mathf.Abs(bytes);

			if (bytes < 1024)
			{
				return (bytes * sign) + " bytes";
			}
			else if (bytes < 1024 * 1024)
			{
				return ((bytes / 1024D) * sign).ToString("0.0") + " KiB";
			}
			else if (bytes < 1024 * 1024 * 1024)
			{
				return ((bytes / (1024D * 1024D)) * sign).ToString("0.0") + " MiB";
			}
			return ((bytes / (1024D * 1024D * 1024D)) * sign).ToString("0.0") + " GiB";
		}
	}

}