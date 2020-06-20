using System.Collections.Generic;
using MEC;

namespace BetterRespawns
{
    public class Cooldowns
    {
		public Plugin plugin;
		public Cooldowns(Plugin plugin) => this.plugin = plugin;

		public static int MtfCmdCool = 0;
		public static int ChaosCmdCool = 0;		
		public static int MtfRespawnTime = 0;
		public static int ChaosRespawnTime = 0;
		public static int TeslasTime = 0;
		public static int IcomTime = 0;
		public static int ScanTime = 0;
		public static int HackTime = 0;
		
		public static bool CheckCooldown(int time)
		{
			if (time <= 0)
				return false;

			return true;
		}

		public static IEnumerator<float> DoMtfCommandCooldown()
		{
			MtfCmdCool = 60;
			for (int i = 0; i < 60; i++)
			{
				MtfCmdCool--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoChsCommandCooldown()
		{
			ChaosCmdCool = 60;
			for (int i = 0; i < 60; i++)
			{
				ChaosCmdCool--;
				yield return Timing.WaitForSeconds(1f);
			}
		}	

		public static IEnumerator<float> DoMTFRespawnCooldown()
		{
			MtfRespawnTime = 600;
			for (int i = 0; i < 600; i++)
			{
				MtfRespawnTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoChaosRespawnCooldown()
		{
			ChaosRespawnTime = 600;
			for (int i = 0; i < 600; i++)
			{
				ChaosRespawnTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoTeslaCooldown()
		{
			TeslasTime = 120;
			for (int i = 0; i < 120; i++)
			{
				TeslasTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoIComCooldown()
		{
			IcomTime = 300;
			for (int i = 0; i < 300; i++)
			{
				IcomTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoScanCooldown()
		{
			ScanTime = 600;
			for (int i = 0; i < 600; i++)
			{
				ScanTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static IEnumerator<float> DoHackCooldown(int time)
		{
			HackTime = time;
			for (int i = 0; i < time; i++)
			{
				HackTime--;
				yield return Timing.WaitForSeconds(1f);
			}
		}
	}
}
