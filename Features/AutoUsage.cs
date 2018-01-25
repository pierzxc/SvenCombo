using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;

namespace SvenCombo.Features
{
    internal class AutoUsage
    {
        private Config Config { get; }

		private SvenCombo Main { get; }

        private TaskHandler Handler { get; }

        public AutoUsage(Config config)
        {
            Config = config;
			Main = config.SvenCombo;

            Handler = UpdateManager.Run(ExecuteAsync, true, true);
        }

        public void Dispose()
        {
            Handler?.Cancel();
        }

        private async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                

                // ArcaneBolt
                if (!Config.ComboKeyItem)
                {
                    var Target =
                        EntityManager<Hero>.Entities.OrderBy(
                            order => order.Health).FirstOrDefault(
                            x => !x.IsIllusion &&
                            x.IsAlive &&
                            x.IsVisible &&
                            x.IsValid &&
                            x.Team != Main.Context.Owner.Team &&
								Main.StormBolt.CanHit(x));


                    if (Target != null
						&& Main.StormBolt != null
						&& Main.StormBolt.CanBeCasted)
                    {
					//	Main.StormBolt.UseAbility(Target);
						await Await.Delay(Main.StormBolt.GetCastDelay(Target), token);
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
