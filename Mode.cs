using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Orbwalker.Modes;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

namespace SvenCombo
{
    internal class Mode : KeyPressOrbwalkingModeAsync
    {
        private Config Config { get; }

		private SvenCombo Main { get; }

        private ITargetSelectorManager TargetSelector { get; }

        private IPredictionManager Prediction { get; }

        public Hero Target { get; set;}


        public Mode(
            IServiceContext context, 
            Key key,
            Config config) : base(context, key)
        {
            Config = config;
			Main = config.SvenCombo;
            TargetSelector = context.TargetSelector;
            Prediction = context.Prediction;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (Config.TargetItem.Value.SelectedValue.Contains("Lock") 
                && (Target == null || !Target.IsValid || !Target.IsAlive))
            {
                if (!TargetSelector.IsActive)
                {
                    TargetSelector.Activate();
                }

                if (TargetSelector.IsActive)
                {
                    Target = TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }

                if (Target != null)
                {
                    if (TargetSelector.IsActive)
                    {
                        TargetSelector.Deactivate();
                    }
                }
            }
            else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && TargetSelector.IsActive)
            {
                Target = TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (Target != null
                && (!Config.BladeMailItem || !Target.HasModifier("modifier_item_blade_mail_reflect")) 
                && !Config.Data.CancelCombo(Target))
            {
                var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                var IsDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");

                if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !Config.Data.AntimageShield(Target))
                {
                    

                    // Orchid
                    if (Main.Orchid != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                        && Main.Orchid.CanBeCasted
                        && Main.Orchid.CanHit(Target))
                    {
                        Main.Orchid.UseAbility(Target);
                        await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                    }

                    // Bloodthorn
                    if (Main.Bloodthorn != null
                        && Config.ItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                        && Main.Bloodthorn.CanBeCasted
                        && Main.Bloodthorn.CanHit(Target))
                    {
                        Main.Bloodthorn.UseAbility(Target);
                        await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                    }
                    
                   

                 
                    // ArcaneBolt
					if (Main.StormBolt != null
						&& Config.AbilityToggler.Value.IsEnabled(Main.StormBolt.Ability.Name)
						&& Main.StormBolt.CanBeCasted
						&& Main.StormBolt.CanHit(Target))
                    {
						Main.StormBolt.UseAbility(Target);
						await Await.Delay(Main.StormBolt.GetCastDelay(Target), token);
                    }

                  	
					// Blink
					if (Main.BlinkDagger != null
						&& Config.ItemsToggler.Value.IsEnabled(Main.BlinkDagger.Item.Name)
						&& Main.BlinkDagger.CanBeCasted
						&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange
						&& Target.Distance2D(Context.Owner) >= Context.Owner.AttackRange)
					{
						Main.BlinkDagger.UseAbility(Target);
						await Await.Delay(Main.BlinkDagger.GetCastDelay(Target), token);
					}
                 
					// Godstrength
					if (Main.GodsStrength != null
						&& Config.AbilityToggler.Value.IsEnabled(Main.GodsStrength.Ability.Name)
						&& Main.GodsStrength.CanBeCasted
						&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange)
					{
						Main.GodsStrength.UseAbility();
						await Await.Delay(Main.GodsStrength.GetCastDelay(Target), token);
					}

					// Warcru
					if (Main.Warcry != null
						&& Config.AbilityToggler.Value.IsEnabled(Main.Warcry.Ability.Name)
						&& Main.Warcry.CanBeCasted
						&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange)
					{
						Main.Warcry.UseAbility();
						await Await.Delay(Main.Warcry.GetCastDelay(Target), token);
					}

					// Bkb
					if (Main.BlackKingBar != null
						&& Config.ItemsToggler.Value.IsEnabled(Main.BlackKingBar.Ability.Name)
						&& Main.BlackKingBar.CanBeCasted
						&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange)
					{
						Main.BlackKingBar.UseAbility();
						await Await.Delay(Main.Warcry.GetCastDelay(Target), token);
					}

				
                }
                else
                {
                    await Config.LinkenBreaker.Breaker(token, Target);
                }
                
                if (Target == null || Target.IsAttackImmune() || Target.IsInvulnerable())
                {
                    if (!Orbwalker.Settings.Move)
                    {
                        Orbwalker.Settings.Move.Item.SetValue(true);
                    }

                    Orbwalker.Move(Game.MousePosition);
                }
                else if (Target != null)
                {
                    if (Owner.Distance2D(Target) <= Config.MinDisInOrbwalk
                        && Target.Distance2D(Game.MousePosition) <= Config.MinDisInOrbwalk)
                    {
                        if (Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(false);
                        }

                        Orbwalker.OrbwalkTo(Target);
                    }
                    else
                    {
                        if (!Orbwalker.Settings.Move)
                        {
                            Orbwalker.Settings.Move.Item.SetValue(true);
                        }

                        Orbwalker.OrbwalkTo(Target);
                    }
                }
            }
            else
            {
                if (!Orbwalker.Settings.Move)
                {
                    Orbwalker.Settings.Move.Item.SetValue(true);
                }
                
                Orbwalker.Move(Game.MousePosition);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }
    }
}
