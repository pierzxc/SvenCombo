using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;

namespace SvenCombo.Features
{
    internal class LinkenBreaker
    {
        private Config Config { get; }

		private SvenCombo Main { get; set; }

        private IOrderedEnumerable<KeyValuePair<string, uint>> BreakerChanger { get; set; }

        public LinkenBreaker(Config config)
        {
            Config = config;
			Main = config.SvenCombo;
        }

        public async Task Breaker(CancellationToken token, Hero Target)
        {
            if (Target.IsLinkensProtected())
            {
                BreakerChanger = Config.LinkenBreakerChanger.Value.Dictionary.Where(
                z => Config.LinkenBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }
            else if (Config.Data.AntimageShield(Target))
            {
                BreakerChanger = Config.AntimageBreakerChanger.Value.Dictionary.Where(
                z => Config.AntimageBreakerToggler.Value.IsEnabled(z.Key)).OrderByDescending(x => x.Value);
            }

            if (BreakerChanger == null)
            {
                return;
            }
            
            foreach (var Order in BreakerChanger.ToList())
            {
              

                
                // Orchid
                if (Main.Orchid != null
                    && Main.Orchid.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Orchid.CanBeCasted
                    && Main.Orchid.CanHit(Target))
                {
                    Main.Orchid.UseAbility(Target);
                    await Await.Delay(Main.Orchid.GetCastDelay(Target), token);
                }

                // Bloodthorn
                if (Main.Bloodthorn != null
                    && Main.Bloodthorn.Item.Name == Order.Key
                    && (Target.IsLinkensProtected() || Config.Data.AntimageShield(Target))
                    && Main.Bloodthorn.CanBeCasted
                    && Main.Bloodthorn.CanHit(Target))
                {
                    Main.Bloodthorn.UseAbility(Target);
                    await Await.Delay(Main.Bloodthorn.GetCastDelay(Target), token);
                }

                
              
              
            }
        }
    }
}
