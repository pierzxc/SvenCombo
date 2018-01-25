using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Service;

namespace SvenCombo.Features
{
    internal class AutoCombo
    {
        private Config Config { get; }

        public Mode Mode { get; }

		public SvenCombo Main { get; }

        private IServiceContext Context { get; }

        private IPredictionManager Prediction { get; }

        private TaskHandler Handler { get; }

        public AutoCombo(Config config)
        {
            Config = config;
			Context = config.SvenCombo.Context;
			Main = config.SvenCombo;
            Mode = config.Mode;
			Prediction = config.SvenCombo.Context.Prediction;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.AutoComboItem)
            {
                Handler.RunAsync();
            }
            
            config.AutoComboItem.PropertyChanged += AutoComboChanged;
        }

        public void Dispose()
        {
            Config.AutoComboItem.PropertyChanged -= AutoComboChanged;

            if (Config.AutoComboItem)
            {
                Handler?.Cancel();
            }
        }

        private void AutoComboChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.AutoComboItem)
            {
                Handler.RunAsync();
            }
            else
            {
                Handler?.Cancel();
            }
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Config.ComboKeyItem)
                {
                    return;
                }

                var Hero = EntityManager<Hero>.Entities.Where(
                    x => !x.IsIllusion &&
                    x.IsAlive &&
                    x.IsVisible &&
                    x.IsValid &&
                    x.Team != Context.Owner.Team).ToList();

                foreach (var Target in Hero)
                {
                    var IsStun = Target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);
                    var IsDebuff = Target.Modifiers.FirstOrDefault(x => x.IsDebuff && x.Name == "modifier_rod_of_atos_debuff");
                    if (Config.Data.Active(Target, IsStun) && !Config.Data.CancelCombo(Target))
                    {
                        if (!Target.IsMagicImmune() && !Target.IsLinkensProtected() && !Config.Data.AntimageShield(Target))
                        {
                            var QueenofPainBlink = Target.GetAbilityById(AbilityId.queenofpain_blink);
                            var AntiMageBlink = Target.GetAbilityById(AbilityId.antimage_blink);


							// Orchid
							if (Main.Orchid != null
								&& Config.AutoItemsToggler.Value.IsEnabled(Main.Orchid.Item.Name)
								&& Main.Orchid.CanBeCasted
								&& Main.Orchid.CanHit(Target))
							{
								Main.Orchid.UseAbility(Target);
								await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
							}

							// Bloodthorn
							if (Main.Bloodthorn != null
								&& Config.AutoItemsToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
								&& Main.Bloodthorn.CanBeCasted
								&& Main.Bloodthorn.CanHit(Target))
							{
								Main.Bloodthorn.UseAbility(Target);
								await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
							}




							// ArcaneBolt
							if (Main.StormBolt != null
								&& Config.AutoAbilitiesToggler.Value.IsEnabled(Main.StormBolt.Ability.Name)
								&& Main.StormBolt.CanBeCasted
								&& Main.StormBolt.CanHit(Target))
							{
								Main.StormBolt.UseAbility(Target);
								await Await.Delay(Main.StormBolt.GetCastDelay(Target), token);
							}


							// Blink
							if (Main.BlinkDagger != null
								&& Config.AutoItemsToggler.Value.IsEnabled(Main.BlinkDagger.Item.Name)
								&& Main.BlinkDagger.CanBeCasted
								&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange
								&& Target.Distance2D(Context.Owner) >= Context.Owner.AttackRange)
							{
								Main.BlinkDagger.UseAbility(Target);
								await Await.Delay(Main.BlinkDagger.GetCastDelay(Target), token);
							}

							// Godstrength
							if (Main.GodsStrength != null
								&& Config.AutoAbilitiesToggler.Value.IsEnabled(Main.GodsStrength.Ability.Name)
								&& Main.GodsStrength.CanBeCasted
								&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange)
							{
								Main.GodsStrength.UseAbility();
								await Await.Delay(Main.GodsStrength.GetCastDelay(Target), token);
							}

							// Warcru
							if (Main.Warcry != null
								&& Config.AutoAbilitiesToggler.Value.IsEnabled(Main.Warcry.Ability.Name)
								&& Main.Warcry.CanBeCasted
								&& Target.Distance2D(Context.Owner) <= Main.BlinkDagger.CastRange)
							{
								Main.Warcry.UseAbility();
								await Await.Delay(Main.Warcry.GetCastDelay(Target), token);
							}

							// Bkb
							if (Main.BlackKingBar != null
								&& Config.AutoItemsToggler.Value.IsEnabled(Main.BlackKingBar.Ability.Name)
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
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Main.Log.Error(e);
            }
        }
    }
}
