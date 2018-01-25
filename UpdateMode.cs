using System.ComponentModel;
using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;
using Ensage.SDK.Renderer.Particle;

using SharpDX;

namespace SvenCombo
{
    internal class UpdateMode
    {
        private Config Config { get; }

		private SvenCombo Main { get; }

        private IServiceContext Context { get; }

        public Hero WShow { get; set; }

        private Hero OffTarget { get; set; }

        private bool WRadius { get; set; }

        private int Count { get; set; }

		private float EnemyRadius { get; set; }

        public UpdateMode(Config config)
        {
            Config = config;
			Main = config.SvenCombo;
			Context = config.SvenCombo.Context;

            UpdateManager.Subscribe(OnUpdate, 25);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void CShotRadiusChanged(object sender, PropertyChangedEventArgs e)
        {
            WRadius = true;
            Count = 0;
        }

        private void OnUpdate()
        {
         

           

            WShow = EntityManager<Hero>.Entities.OrderBy(
                x => x.Distance2D(Context.Owner)).FirstOrDefault(
                x => !x.IsIllusion &&
                x.IsAlive &&
                x.IsVisible &&
                x.IsValid &&
                x.Team != Context.Owner.Team &&
					x.Distance2D(Context.Owner) <= Main.StormBolt.Radius - 25);

            

            if (Context.TargetSelector.IsActive 
                || OffTarget == null || !OffTarget.IsValid || !OffTarget.IsAlive)
            {
                OffTarget = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
            }

            if (OffTarget != null)
            {
                Context.Particle.DrawTargetLine(
                    Context.Owner,
                    "Target",
                    OffTarget.Position,
                    Config.Mode.CanExecute ? Color.Red : Color.Aqua);
            }
            else
            {
                Context.Particle.Remove("Target");
            }

            if (!Config.Mode.CanExecute && Config.Mode.Target != null)
            {
                if (!Context.TargetSelector.IsActive)
                {
                    Context.TargetSelector.Activate();
                }

                Config.Mode.Target = null;
            }
        }
    }
}
