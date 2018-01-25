using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;

namespace SvenCombo.Features
{
    internal class AutoDisable
    {
        private Config Config { get; }

        private IServiceContext Context { get; }

		private SvenCombo Main { get; }

        private TaskHandler Handler { get; }

        public AutoDisable(Config config)
        {
            Config = config;
			Context = config.SvenCombo.Context;
			Main = config.SvenCombo;

            Handler = UpdateManager.Run(ExecuteAsync, true, false);

            if (config.AutoDisableItem)
            {
                Handler.RunAsync();
            }

            config.AutoDisableItem.PropertyChanged += AutoDisableChanged;
        }

        public void Dispose()
        {
            Config.AutoDisableItem.PropertyChanged -= AutoDisableChanged;

            if (Config.AutoDisableItem)
            {
                Handler?.Cancel();
            }
        }

        private void AutoDisableChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Config.AutoDisableItem)
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
                var Hero = 
                    EntityManager<Hero>.Entities.Where(
                        x => !x.IsIllusion &&
                        x.IsAlive &&
                        x.IsVisible &&
                        x.IsValid &&
                        x.Team != Context.Owner.Team).ToList();

                foreach (var Target in Hero)
                {
                    if (Config.Data.Disable(Target))
                    {
                      

                        // Orchid
                        if (Main.Orchid != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.Orchid.Item.Name)
                            && Main.Orchid.CanBeCasted
                            && Main.Orchid.CanHit(Target))
                        {
                            Main.Orchid.UseAbility(Target);
                            await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                        }

                        // Bloodthorn
                        if (Main.Bloodthorn != null
                            && Config.AutoDisableToggler.Value.IsEnabled(Main.Bloodthorn.Item.Name)
                            && Main.Bloodthorn.CanBeCasted
                            && Main.Bloodthorn.CanHit(Target))
                        {
                            Main.Bloodthorn.UseAbility(Target);
                            await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
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
