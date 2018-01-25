using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

using SvenCombo.Features;

namespace SvenCombo
{
    internal class Config : IDisposable
    {
        private MenuFactory Factory { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<AbilityToggler> ItemsToggler { get; }

        public MenuItem<bool> AutoComboItem { get; }

        public MenuItem<AbilityToggler> AutoAbilitiesToggler { get; }

        public MenuItem<AbilityToggler> AutoItemsToggler { get; }

        public MenuItem<bool> AutoDisableItem { get; }

        public MenuItem<AbilityToggler> AutoDisableToggler { get; }

        public MenuItem<AbilityToggler> LinkenBreakerToggler { get; }

        public MenuItem<PriorityChanger> LinkenBreakerChanger { get; }

		public MenuItem<PriorityChanger> ComboPriority { get; }


        public MenuItem<bool> TextItem { get; }



        public MenuItem<KeyBind> ComboKeyItem { get; }




        public MenuItem<Slider> MinDisInOrbwalk { get; }

        public MenuItem<StringList> TargetItem { get; }

        public MenuItem<AbilityToggler> AntimageBreakerToggler { get; }

        public MenuItem<PriorityChanger> AntimageBreakerChanger { get; }

        public MenuItem<bool> BladeMailItem { get; }


		public SvenCombo SvenCombo { get; }

        public Mode Mode { get; }

        public Data Data { get; }

        public LinkenBreaker LinkenBreaker { get; }

     

        private AutoCombo AutoCombo { get; }

        private AutoDisable AutoDisable { get; }

        public UpdateMode UpdateMode { get; }


        private AutoUsage AutoUsage { get; }

        private Renderer Renderer { get; }

        private bool Disposed { get; set; }

		public Config(SvenCombo svenCombo)
        {
			SvenCombo = svenCombo;

            Factory = MenuFactory.CreateWithTexture("SvenCombo", "npc_dota_hero_sven");
			Factory.Target.SetFontColor(Color.Red);
            var AbilitiesMenu = Factory.Menu("Abilities");
            AbilityToggler = AbilitiesMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "sven_storm_bolt", true },
					{ "sven_gods_strength", true },
					{ "sven_warcry", true }
            }));

            var ItemsMenu = Factory.Menu("Items");
            ItemsToggler = ItemsMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "item_blink", true },
					{ "item_black_king_bar", true },
					{ "item_satanic", true },
					{ "item_bloodthorn", true }
            }));

            var AutoComboMenu = Factory.Menu("Auto Combo");
            AutoComboItem = AutoComboMenu.Item("Use Auto Combo", true);
            AutoAbilitiesToggler = AutoComboMenu.Item("Abilities: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "sven_storm_bolt", true },
					{ "sven_gods_strength", true },
					{ "sven_warcry", true }
            }));

            AutoItemsToggler = AutoComboMenu.Item("Items: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "item_blink", true },
					{ "item_black_king_bar", true },
					{ "item_satanic", true },
					{ "item_bloodthorn", true }
            }));



	
            var AutoDisableMenu = Factory.MenuWithTexture("Auto Disable", "item_sheepstick");
            AutoDisableItem = AutoDisableMenu.Item("Use Auto Disable", true);
            AutoDisableToggler = AutoDisableMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
                { "item_bloodthorn", true }
            }));

            var LinkenBreakerMenu = Factory.MenuWithTexture("Linken Breaker", "item_sphere");
            LinkenBreakerToggler = LinkenBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "item_bloodthorn", true },
					{ "item_orchid", true }
            }));

            LinkenBreakerChanger = LinkenBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
                { "item_bloodthorn" },
                { "item_orchid" }
            }));

		

            var AntimageBreakerMenu = Factory.MenuWithTexture("Antimage Breaker", "antimage_spell_shield");
            AntimageBreakerToggler = AntimageBreakerMenu.Item("Use: ", new AbilityToggler(new Dictionary<string, bool>
            {
					{ "item_bloodthorn", true },
					{ "item_orchid", true }
            }));

            AntimageBreakerChanger = AntimageBreakerMenu.Item("Priority: ", new PriorityChanger(new List<string>
            {
					{ "item_bloodthorn" },
					{ "item_orchid" }
            }));

            var BladeMailMenu = Factory.MenuWithTexture("Blade Mail", "item_blade_mail");
            BladeMailItem = BladeMailMenu.Item("Cancel Combo", true);
            BladeMailItem.Item.SetTooltip("Cancel Combo if there is enemy Blade Mail");
         

          

            var DrawingMenu = Factory.Menu("Drawing");
            TextItem = DrawingMenu.Item("Text", true);
           

            ComboKeyItem = Factory.Item("Combo Key", new KeyBind('D'));
         
  
            MinDisInOrbwalk = Factory.Item("Min Distance in OrbWalk", new Slider(0, 0, 600));
            TargetItem = Factory.Item("Target", new StringList("Lock", "Default"));

            ComboKeyItem.Item.ValueChanged += HotkeyChanged;

            var Key = KeyInterop.KeyFromVirtualKey((int)ComboKeyItem.Value.Key);

			Mode = new Mode(SvenCombo.Context, Key, this);
			SvenCombo.Context.Orbwalker.RegisterMode(Mode);

            Data = new Data();
            LinkenBreaker = new LinkenBreaker(this);
            AutoCombo = new AutoCombo(this);
            AutoDisable = new AutoDisable(this);
            UpdateMode = new UpdateMode(this);
            AutoUsage = new AutoUsage(this);
            Renderer = new Renderer(this);
        }

        private void HotkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var KeyCode = e.GetNewValue<KeyBind>().Key;

            if (KeyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var Key = KeyInterop.KeyFromVirtualKey((int)KeyCode);
            Mode.Key = Key;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                Renderer.Dispose();
                AutoUsage.Dispose();
                UpdateMode.Dispose();
                AutoDisable.Dispose();
                AutoCombo.Dispose();
				SvenCombo.Context.Orbwalker.UnregisterMode(Mode);
                Mode.Deactivate();
				SvenCombo.Context.Particle.Dispose();
                ComboKeyItem.Item.ValueChanged -= HotkeyChanged;
                Factory.Dispose();
            }

            Disposed = true;
        }
    }
}
