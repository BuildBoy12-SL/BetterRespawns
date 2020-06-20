using EXILED;
using EXILED.Extensions;
using UnityEngine;
using MEC;

namespace BetterRespawns
{
	public class CommanderComponent : MonoBehaviour
	{
		public ReferenceHub hub;
		
		public void Awake()
		{
			hub = gameObject.GetPlayer();
			Events.ConsoleCommandEvent += OnConsoleCommand;
		}

		public void OnDestroy()
		{
			hub = null;
			Events.ConsoleCommandEvent -= OnConsoleCommand;
		}

		public void OnConsoleCommand(ConsoleCommandEvent ev)
		{
			if(Cooldowns.CheckCooldown(Cooldowns.MtfCmdCool))
			{
				ev.Color = "red";
				ev.ReturnMessage = $"You must wait {Cooldowns.MtfCmdCool} seconds to use abilities.";
				return;
			}

			if (ev.Player != hub)
				return;

			if(ev.Command.StartsWith("respawn"))
			{
				if (Cooldowns.CheckCooldown(Cooldowns.MtfRespawnTime))
				{
					ev.Color = "red";
					ev.ReturnMessage = $"You must wait {Cooldowns.MtfRespawnTime} seconds to respawn another wave.";
					return;
				}
				
				GameCore.Console.singleton.TypeCommand("/SERVER_EVENT FORCE_MTF_RESPAWN");
				ev.Color = "cyan";
				ev.ReturnMessage = "An MTF unit has been alerted to immediately come to your location.";
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoMTFRespawnCooldown()));
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoMtfCommandCooldown()));
			}			
			else if (ev.Command.StartsWith("teslas"))
			{
				if(Cooldowns.CheckCooldown(Cooldowns.TeslasTime))
				{
					ev.Color = "red";
					ev.ReturnMessage = $"Tesla gates cannot be toggled for {Cooldowns.TeslasTime} seconds.";
					return;
				}

				if (EventHandlers.TeslaDisabled)
				{
					EventHandlers.TeslaDisabled = false;
					Methods.DoAnnouncement("MtfUnit Commander has activated Facility automatic security systems");
				}				
				else
				{
					EventHandlers.TeslaDisabled = true;
					Methods.DoAnnouncement("MtfUnit Commander has Deactivated Facility automatic security systems ");
				}
				ev.Color = "cyan";
				ev.ReturnMessage = "Tesla gates have been successfully toggled.";
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoTeslaCooldown()));
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoMtfCommandCooldown()));
			}
			else if (ev.Command.StartsWith("icom"))
			{
				if(Cooldowns.CheckCooldown(Cooldowns.IcomTime))
				{
					ev.Color = "red";
					ev.ReturnMessage = $"You cannot use the intercom remotely for {Cooldowns.IcomTime} seconds.";
					return;
				}	
				if (ev.Player.IsIntercomMuted())
				{
					ev.Color = "red";
					ev.ReturnMessage = "Denied as you are currently intercom muted. Contact this server's staff if you believe this is in error.";
					return;
				}
				GameCore.Console.singleton.TypeCommand("INTERCOM-TIMEOUT");
				ev.Color = "cyan";
				ev.ReturnMessage = "You are now live on the intercom. Thirty seconds remaining.";
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoIComCooldown()));
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoMtfCommandCooldown()));
			}
			else if(ev.Command.StartsWith("scan"))
			{
				if(Cooldowns.CheckCooldown(Cooldowns.ScanTime))
				{
					ev.Color = "red";
					ev.ReturnMessage = $"You cannot scan the facility for {Cooldowns.ScanTime} seconds.";
					return;
				}
				ev.Color = "cyan";
				ev.ReturnMessage = "Commencing Scan.";
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Methods.DoScan("mtf")));
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoScanCooldown()));
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Cooldowns.DoMtfCommandCooldown()));
			}
			else if(ev.Command.StartsWith("lockdown"))
			{
				if(!EventHandlers.MTFLock)
				{
					ev.Color = "red";
					ev.ReturnMessage = "You must wait until another round to access this command.";
					return;
				}
				EventHandlers.Coroutines.Add(Timing.RunCoroutine(Methods.DoGateLock()));
				ev.Color = "cyan";
				ev.ReturnMessage = "Lockdown successful.";
				EventHandlers.MTFLock = false;
			}
			else if (ev.Command.StartsWith("help"))
			{
				ev.Color = "white";
				ev.ReturnMessage =
					"\n.help: Shows all commands and delays" +
					"\n.teslas: Toggles the activation of teslas - Cooldown: 2 minutes" +
					"\n.respawn: Forces a MTF respawn - Cooldown: 10 minutes" +
					//"\n.icom: Allows you to speak on the intercom at will - Cooldown: 5 minutes" + 
					"\n.scan: Scans the facility for all alive people - Cooldown: 10 minutes" +
					"\n.lockdown: Locks both Gate A and Gate B closed for one minute - Can be used once per round";				
			};
		}		
	}
}