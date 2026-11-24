namespace Bas.Thoth.Topdown.Factory
{
    public class PowerSystem : Component
    {
        private FactoryContext _context;
        private GridPositionComponent _positions;
        private SolarComponent _solars;
        private MachineComponent _machines;
        private InserterComponent _inserters;

        public override void Init()
        {
            _context = Singletons.Get<FactoryContext>();
            _positions = Singletons.Get<GridPositionComponent>();
            _solars = Singletons.Get<SolarComponent>();
            _machines = Singletons.Get<MachineComponent>();
            _inserters = Singletons.Get<InserterComponent>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            _context.Power.Recalculate(_context.Grid, _positions, _solars, _machines, _inserters);
        }
    }
}
