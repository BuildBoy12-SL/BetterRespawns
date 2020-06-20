using EXILED;
using EXILED.Extensions;
using UnityEngine;
using MEC;
using System;

namespace BetterRespawns
{
	public class ChaosComponent : MonoBehaviour
	{
		public ReferenceHub hub;
		
		public void Awake()
		{
			hub = gameObject.GetPlayer();
			Events.ConsoleCommandEvent += OnConsoleCommand;
			Events.DoorInteractEvent += OnDoorAccess;
		}

		public void OnDestroy()
		{
			hub = null;
			Events.ConsoleCommandEvent -= OnConsoleCommand;
			Events.DoorInteractEvent -= OnDoorAccess;
		}

		public void OnConsoleCommand(ConsoleCommandEvent ev)
		{
			try
			{
				if (Cooldowns.CheckCooldown(Cooldowns.ChaosCmdCool))
				{
					ev.ReturnMessage = $"<color=red>You must wait {Cooldowns.ChaosCmdCool} seconds to use abilities.</color>";
					return;
				}

				if (ev.Player != hub)
					return;

				if (ev.Command.StartsWith("emp"))
				{
					bool Does173Exist = new bool();
					if (!EventHandlers.ChaosDevice)
					{
						ev.Color = "red";
						ev.ReturnMessage = "You must wait until another round to access this command.";
						return;
					}
					foreach (ReferenceHub hub in Player.GetHubs())
					{
						if (hub.GetRole() == RoleType.Scp173)
							Does173Exist = true;
					}
					if (Does173Exist)
					{
						ev.Color = "red";
						ev.ReturnMessage = "Blackout cancelled. Entity in the facility has been found to directly benefit from the darkness.";
						Does173Exist = false;
						return;
					}
					ev.Color = "yellow";
					ev.ReturnMessage = "Facility blacked out for 60 seconds.";
					EventHandlers.ChaosDevice = false;
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Methods.DoBlackout()));
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoChsCommandCooldown()));
				}
				else if (ev.Command.StartsWith("respawn"))
				{
					if (Cooldowns.CheckCooldown(Cooldowns.ChaosRespawnTime))
					{
						ev.Color = "red";
						ev.ReturnMessage = $"You must wait {Cooldowns.ChaosRespawnTime} seconds to respawn another wave.";
						return;
					}
					GameCore.Console.singleton.TypeCommand("/SERVER_EVENT FORCE_CI_RESPAWN");
					ev.ReturnMessage = "<color=cyan>An Chaos unit has been alerted to immediately come to your location.</color>";
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoChaosRespawnCooldown()));
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoChsCommandCooldown()));
				}
				else if (ev.Command.StartsWith("hack"))
				{
					if(Cooldowns.CheckCooldown(Cooldowns.HackTime))
					{
						ev.Color = "red";
						ev.ReturnMessage = $"You must wait {Cooldowns.HackTime} seconds to attempt another hack.";
						return;
					}

					string Cmd = string.Empty;
					int CoolTime = new int();
					
					if (ev.Command.Contains("scan"))
					{
						Cmd = "scan";
						CoolTime = 600;
					}										
					else if (ev.Command.Contains("teslas"))
					{
						Cmd = "teslas";
						CoolTime = 120;
					}							
					else
					{
						ev.ReturnMessage = "You must specify a command to hack: .hack (scan/teslas)";
						return;
					}						
					ev.ReturnMessage = null;
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Methods.HackSequence(hub, Cmd)));
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoHackCooldown(CoolTime)));
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoChsCommandCooldown()));
				}
				else if (ev.Command.StartsWith("help"))
				{
					ev.Color = "white";
					ev.ReturnMessage =
						"\n.help: Shows all commands and delays " +
						"\n.emp: Blacksout the facility and disabled tesla gates, also lets anybody open any door - Can only be used once per round" +
						"\n.respawn: Forces a Chaos respawn - Cooldown: 10 minutes" +
						"\n.hack (scan/teslas): Attempts to mimick the NTF commanders ability's - Cooldowns: (scan 10 min/teslas 2 min)";
				}
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawns exception for ChaosCommander ConsoleCommand: {e}");
			}
		}

		public void OnDoorAccess(ref DoorInteractionEvent ev)
		{
			if (Methods.BlackoutMode)
				ev.Allow = true;
		}
	}
}