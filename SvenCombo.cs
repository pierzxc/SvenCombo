using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_sven;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

using log4net;

using PlaySharp.Toolkit.Logging;

namespace SvenCombo
{
	[ExportPlugin(
		name: "SvenCombo",
		mode: StartupMode.Auto,
		author: "PIER", 
		version: "1.2.1.1",
		units: HeroId.npc_dota_hero_sven)]
	internal class SvenCombo : Plugin
	{
		private Config Config { get; set; }

		public IServiceContext Context { get; }

		public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ImportingConstructor]
		public SvenCombo([Import] IServiceContext context)
		{
			Context = context;
			AbilityFactory = context.AbilityFactory;
		}

		private AbilityFactory AbilityFactory { get; }

		public sven_storm_bolt StormBolt { get; set; }

		public sven_warcry Warcry { get; set; }

		public sven_gods_strength GodsStrength { get; set; }

		public sven_great_cleave GreatCleave { get; set; }

		public Dagon Dagon
		{
			get
			{
				return Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;
			}
		}

		[ItemBinding]
		public item_blink BlinkDagger { get; set; }

		[ItemBinding]
		public item_mask_of_madness MaskOfMadness { get; set; }

		[ItemBinding]
		public item_sheepstick Hex { get; set; }

		[ItemBinding]
		public item_orchid Orchid { get; set; }

		[ItemBinding]
		public item_bloodthorn Bloodthorn { get; set; }

		[ItemBinding]
		public item_rod_of_atos RodofAtos { get; set; }

		[ItemBinding]
		public item_veil_of_discord Veil { get; set; }

		[ItemBinding]
		public item_ethereal_blade Ethereal { get; set; }

		[ItemBinding]
		public item_black_king_bar BlackKingBar { get; set; }

		[ItemBinding]
		public item_dagon Dagon1 { get; set; }

		[ItemBinding]
		public item_dagon_2 Dagon2 { get; set; }

		[ItemBinding]
		public item_dagon_3 Dagon3 { get; set; }

		[ItemBinding]
		public item_dagon_4 Dagon4 { get; set; }

		[ItemBinding]
		public item_dagon_5 Dagon5 { get; set; }

		[ItemBinding]
		public item_force_staff ForceStaff { get; set; }

		[ItemBinding]
		public item_cyclone Eul { get; set; }

		protected override void OnActivate()
		{
			Config = new Config(this);

			StormBolt = AbilityFactory.GetAbility<sven_storm_bolt>();
			Warcry = AbilityFactory.GetAbility<sven_warcry>();
			GodsStrength = AbilityFactory.GetAbility<sven_gods_strength>();
			GreatCleave = AbilityFactory.GetAbility<sven_great_cleave>();

			Context.Inventory.Attach(this);
		}

		protected override void OnDeactivate()
		{
			Context.Inventory.Detach(this);

			Config?.Dispose();
		}
	}
}
