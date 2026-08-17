using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class CurrencyView : Label
{
    private SimulationWorld? _world;
    private Entity _currencyEntity;

    public void Initialize(SimulationWorld world, Entity currencyEntity)
    {
        _world = world;
        _currencyEntity = currencyEntity;
        UpdateLabel();
    }

    public override void _Process(double delta) => UpdateLabel();

    private void UpdateLabel()
    {
        if (_world is not null &&
            _world.TryGetComponent<Currency>(_currencyEntity, out var currency))
        {
            Text = $"Gold: {currency.Amount}";
        }
    }
}
