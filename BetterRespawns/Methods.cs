using System.Collections.Generic;
using EXILED.Extensions;
using MEC;

namespace BetterRespawns
{
    public class Methods
    {
        public Plugin plugin;
        public Methods(Plugin plugin) => this.plugin = plugin;

		public static bool BlackoutMode;

		public static IEnumerator<float> RespawnDelay(bool IsChaos, List<ReferenceHub> ToRespawn)
		{
			yield return Timing.WaitForSeconds(.5f);

			if (IsChaos)
			{
				if (CheckChaosCommander())
					yield break;

				ReferenceHub LeadChaos = new ReferenceHub();
				for (int i = 0; i < 1; i++)
				{
					ToRespawn[i].gameObject.AddComponent<ChaosComponent>();
					ToRespawn[i].Broadcast(10, "<color=red>You are the Chaos commander! Press ` for more info.</color>", false);
					ToRespawn[i].SendConsoleMessage(
						"\n.help: Shows all commands and delays " +
						"\n.emp: Blacksout the facility, disables tesla gates, and makes any door openable - Can only be used once per round" +
						"\n.respawn: Forces a Chaos respawn - Cooldown: 10 minutes" +
						"\n.hack (scan/teslas): Attempts to mimick the NTF commanders ability's - Cooldowns: (scan 10 min/teslas 2 min)", "white");
					ToRespawn[i].SetMaxHealth(150);
					ToRespawn[i].SetHealth(150);
					ToRespawn[i].AddItem(ItemType.GrenadeFrag);
					LeadChaos = ToRespawn[i];
				}
				foreach (ReferenceHub hub in ToRespawn)
				{
					LeadChaos.RefreshTag();
					LeadChaos.SetRank("Chaos Commander", "green");
					LeadChaos.characterClassManager.CallCmdRequestHideTag();
					LeadChaos.serverRoles.TargetSetHiddenRole(hub.serverRoles.connectionToClient, "Chaos Commander");
				}
			}
			else
				foreach (ReferenceHub hub in ToRespawn)
					if (hub.GetRole() == RoleType.NtfCommander && CheckCommander())
					{
						hub.SetRole(RoleType.NtfLieutenant);
					}
					else if (hub.GetRole() == RoleType.NtfCommander && !CheckCommander())
					{
						hub.gameObject.AddComponent<CommanderComponent>();
						hub.Broadcast(10, "<color=red>You are the MTF commander! Press ` for more info.</color>", false);
						hub.SendConsoleMessage(
							"\n.help: Shows all commands and delays" +
							"\n.teslas: Toggles the activation of teslas - Cooldown: 2 minutes" +
							"\n.respawn: Forces a MTF respawn - Cooldown: 10 minutes" +
							//"\n.icom: Allows you to speak on the intercom at will - Cooldown: 5 minutes" +
							"\n.scan: Scans the facility for all alive people - Cooldown: 10 minutes" +
							"\n.lockdown: Locks both Gate A and Gate B closed for one minute - Can be used once per round", "white");
					}
		}

		public static bool CheckCommander()
		{
			int CommanderCount = 0;

			foreach (ReferenceHub hub in Player.GetHubs())
				if (hub.GetRole() == RoleType.NtfCommander)
					CommanderCount++;

			if (CommanderCount != 1)
				return true;

			return false;
		}

		public static bool CheckChaosCommander()
		{
			int i = 0;
			foreach (ReferenceHub hub in Player.GetHubs())
				if (hub.GetComponent<ChaosComponent>() != null)
					i++;
					
			if(i > 0)
				return true;

			return false;
		}

		public static IEnumerator<float> DoScan(string team)
		{
			int ClassDCount = 0;
			int ScienceCount = 0;
			int ScpCount = 0;
			int MtfCount = 0;
			int ChaosCount = 0;
			int TutCount = 0;

			string ClassD = string.Empty;
			string Science = string.Empty;
			string Scp = string.Empty;
			string Mtf = string.Empty;
			string Chaos = string.Empty;
			string Tut = string.Empty;

			if (team == "mtf")
				DoAnnouncement("Mtfunit Commander has Executed a facility wide scan . . results in tminus 1 minute");
			else
				DoAnnouncement(". override . successful . .g3 . Mtfunit  jam_012_3 Commander . has  jam_012_4 Executed . .g4  a facility .g2 wide scan . . results .g1 in tminus .g4 .g5 .  jam_001_2 1 jam_001_2 minute");

			yield return Timing.WaitForSeconds(60);

			foreach (ReferenceHub hub in Player.GetHubs())
			{
				if (hub.GetRole() == RoleType.ClassD)
					ClassDCount++;
				else if (hub.GetTeam() == Team.RSC)
					ScienceCount++;
				else if (hub.GetTeam() == Team.SCP)
					ScpCount++;
				else if (hub.GetTeam() == Team.MTF)
					MtfCount++;
				else if (hub.GetTeam() == Team.CHI)
					ChaosCount++;
				else if (hub.GetTeam() == Team.TUT)
					TutCount++;
			}

			if (ClassDCount != 0)
				ClassD = ClassDCount + " Classd . ";
			if (ScienceCount != 0)
				Science = ScienceCount + " Scientist . ";
			if (ScpCount != 0)
				Scp = ScpCount + " ScpSubjects . ";
			if (MtfCount != 0)
				Mtf = MtfCount + " Task force . ";
			if (ChaosCount != 0)
				Chaos = ChaosCount + " ChaosInsurgency  . .g6 ";
			if (TutCount != 0)
				Tut = TutCount + " Unspecified Personnel";

			if (team == "mtf")
				DoAnnouncement($"Facility wide scan completed . Personnel in facility  . {ClassD} {Science} {Scp} {Mtf} {Chaos} {Tut}");
			else
				DoAnnouncement($". jam_001_3  Facility wide scan jam_001_3  completed . Personnel .g4 in facility  . {ClassD} {Science} {Scp} {Mtf} {Chaos} {Tut}");
		}

		public static IEnumerator<float> DoBlackout()
		{
			ReferenceHub scp079 = new ReferenceHub();
			int lvl = new int();
			float maxEnergy = new float();

			foreach (ReferenceHub hub in Player.GetHubs())
				if(hub.GetRole() == RoleType.Scp079)
				{
					scp079 = hub;
					lvl = hub.GetLevel();
					hub.SetMaxEnergy(0);
					hub.SetEnergy(0);
					hub.Broadcast(10, "<color=red>The blackout has cut off all power for 60 seconds!</color>", false);
				}
			BlackoutMode = true;
			DoAnnouncement("pitch_0.6 jam_001_3 Facility . Power .g4 . systems Malfunction .g2 detected . . Initializing .g5 . jam_001_3 back  . up Systems in tminus 1 minute .g6");
			Map.TurnOffAllLights(60f, false);
			EventHandlers.TeslaDisabled = true;
			yield return Timing.WaitForSeconds(60f);
			DoAnnouncement(". Facility back up  systems initiated .  . Door systems online . automatic security systems  Activated  . Containment chambers Electromagnetism Successfully Initiated . . Facility back in operation mode . . Source of Outage detected . .   .g5 Unspecified device of the ChaosInsurgency. ");
			EventHandlers.TeslaDisabled = false;
			BlackoutMode = false;

			foreach (Door door in Map.Doors)		
				door.NetworkisOpen = false;
			
			if(lvl < 5)
				lvl++;

			scp079.SetLevel(lvl, false);

			if (lvl == 2)
				maxEnergy = 110;
			else if (lvl == 3)
				maxEnergy = 125;
			else if (lvl == 4)
				maxEnergy = 150;
			else if (lvl == 5)
				maxEnergy = 200;

			scp079.SetMaxEnergy(maxEnergy);
			scp079.Broadcast(10, "<color=green>Power has been restored! You have levelled up.</color>", false);
		}

		public static IEnumerator<float> DoGateLock()
		{
			foreach (Door door in Map.Doors)
				if (door.DoorName.ToUpper() == "gate_a" || door.DoorName.ToUpper() == "gate_b")
				{
					door.NetworkisOpen = false;
					door.Networklocked = true;
				}
			yield return Timing.WaitForSeconds(60f);
			foreach (Door door in Map.Doors)
				if (door.DoorName.ToUpper() == "gate_a" || door.DoorName.ToUpper() == "gate_b")
				{
					door.NetworkisOpen = true;
					door.Networklocked = false;
				}
		}
		
		public static bool HackChance()
		{
			System.Random rnd = new System.Random();
			if (rnd.Next(100) < 45)			
				return true;
			
			return false;
		}

		public static IEnumerator<float> HackSequence(ReferenceHub sender, string cmd)
		{
			DoAnnouncement("Unauthorized access detected  .  .  . jam_012_2  Override .g1 . attempt .  Initiated");
			yield return Timing.WaitForSeconds(30f);
			if (!HackChance())
			{
				DoAnnouncement("Hack attempt . Deactivated . facility Software Systems . Repair . initiated");
				sender.SendConsoleMessage("The hack has failed. You may retry once the cooldown wears off.", "red");
				yield break;
			}
			if(cmd == "teslas")
			{
				if (EventHandlers.TeslaDisabled)
				{
					EventHandlers.TeslaDisabled = false;
					DoAnnouncement(". override . successful . MtfUnit .g4 jam_001_3 Commander has activated .g3 .g5 .g1 . Facility automatic jam_001_3 . automatic .g6 security .g3 jam_001_3 automatic systems");
				}
				else
				{
					EventHandlers.TeslaDisabled = true;
					DoAnnouncement(". override . successful . MtfUnit .g4 jam_001_3 Commander has Deactivated .g3 .g5 .g1 . Facility automatic jam_001_3 . automatic .g6 security .g3 jam_001_3 automatic systems");
				}
			}
			if(cmd == "scan")
			{
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(DoScan("chaos")));
			}
		}

		public static void DoAnnouncement(string msg)
		{
			PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement(msg, false, true);
		}
	}
}
