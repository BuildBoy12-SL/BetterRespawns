using System;
using System.Collections.Generic;
using EXILED;
using MEC;

namespace BetterRespawns 
{
	public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public static List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public static bool TeslaDisabled = false;
		public static bool ChaosDevice = true;
		public static bool MTFLock = true;

		public void OnRoundStart()
		{
			try
			{
				ChaosDevice = true;
				MTFLock = true;
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawns exception OnRoundStart: {e}");
			}			
		}

		public void OnRoundEnd()
		{
			foreach (CoroutineHandle handle in Coroutines)
				Timing.KillCoroutines(handle);
		}

		public void OnRespawn(ref TeamRespawnEvent ev)
		{
			try
			{
				Coroutines.Add(Timing.RunCoroutine(Methods.RespawnDelay(ev.IsChaos, ev.ToRespawn)));
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawn exception OnRespawn: {e}");
			}
		}

		public void OnTriggerTesla(ref TriggerTeslaEvent ev)
		{
			try
			{
				if (TeslaDisabled)
					ev.Triggerable = false;
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawns exception OnTriggerTesla: {e}");
			}		
		}

		public void PlayerDeath(ref PlayerDeathEvent ev)
		{	
			try
			{
				if (ev.Player.GetComponent<CommanderComponent>() != null)
				{
					UnityEngine.Object.Destroy(ev.Player.GetComponent<CommanderComponent>());

					if (TeslaDisabled)
					{
						Methods.DoAnnouncement("Mtfunit Commander has been .g2 . Redacted .g2 .  . facility will Return to Previous security protocols . Initializing Facility automatic security systems");
						TeslaDisabled = false;
					}
				}
				else if (ev.Player.GetComponent<ChaosComponent>() != null)
				{
					UnityEngine.Object.Destroy(ev.Player.GetComponent<ChaosComponent>());
					ev.Player.RefreshTag();
					ev.Player.SetRank("", "default");
				}
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawns exception during PlayerDeath: {e}");
			}
		}

		public void SetClass(SetClassEvent ev)
		{
			try
			{
				if (ev.Player.GetComponent<CommanderComponent>() != null)
					UnityEngine.Object.Destroy(ev.Player.GetComponent<CommanderComponent>());


				else if (ev.Player.GetComponent<ChaosComponent>() != null)
				{
					UnityEngine.Object.Destroy(ev.Player.GetComponent<ChaosComponent>());
					ev.Player.RefreshTag();
					ev.Player.SetRank("", "default");
				}
			}
			catch(Exception e)
			{
				Log.Error($"BetterRespawns exception during SetClass: {e}");
			}
		}		
	}
}