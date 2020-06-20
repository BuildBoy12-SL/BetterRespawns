using EXILED;

namespace BetterRespawns 
{
	public class Plugin : EXILED.Plugin
	{
		public EventHandlers EventHandlers;
		public Cooldowns Cooldowns;
		public Methods Methods;

		internal bool Enabled;
		
		public override void OnEnable()
		{
			Configs();
			if (!Enabled)
				return;

			EventHandlers = new EventHandlers(this);
			Cooldowns = new Cooldowns(this);
			Methods = new Methods(this);

			Events.RoundEndEvent += EventHandlers.OnRoundEnd;
			Events.TeamRespawnEvent += EventHandlers.OnRespawn;
			Events.TriggerTeslaEvent += EventHandlers.OnTriggerTesla;
			Events.PlayerDeathEvent += EventHandlers.PlayerDeath;
			Events.SetClassEvent += EventHandlers.SetClass;
			Events.RoundStartEvent += EventHandlers.OnRoundStart;

			Log.Info($"BetterRespawns loaded.");
		}

		public override void OnDisable()
		{
			Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
			Events.TeamRespawnEvent -= EventHandlers.OnRespawn;
			Events.TriggerTeslaEvent -= EventHandlers.OnTriggerTesla;
			Events.PlayerDeathEvent -= EventHandlers.PlayerDeath;
			Events.SetClassEvent -= EventHandlers.SetClass;
			Events.RoundStartEvent -= EventHandlers.OnRoundStart;

			EventHandlers = null;
			Cooldowns = null;
			Methods = null;
			Log.Info("BetterRespawns disabled.");
		}

		public override void OnReload()
		{
			Log.Info("Reloading BetterRespawns.");
		}

		public override string getName => "BetterRespawns";

		public void Configs()
		{ 
			Enabled = Config.GetBool("br_enable", true); 		
		}
	}
}