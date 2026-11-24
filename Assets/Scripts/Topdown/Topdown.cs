using Bas.Thoth.Topdown.Factory;
using System.Collections.Generic;

namespace Bas.Thoth.Topdown
{
    public class Topdown : ThothManager
    {
        private TopdownFactory _factory;

        protected override void PostStart()
        {
            int playerId = _factory.CreatePlayer();
            _factory.CreateBackground();
            FactoryGame.Start(playerId);
        }

        protected override IEnumerable<Bas.Thoth.Factory> GetFactories()
        {
            return new[] { _factory = Singletons.Get<TopdownFactory>() };
        }
    }
}
