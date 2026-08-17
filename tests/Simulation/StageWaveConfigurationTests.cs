using System.Linq;
using OpenTD.Simulation.Configuration;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class StageWaveConfigurationTests
{
    private static readonly MapConfiguration Map = MapConfiguration.CreateDefault();

    [Theory]
    [InlineData(1, 3)]
    [InlineData(2, 4)]
    [InlineData(8, 10)]
    public void StageHasTwoMoreWavesThanItsNumber(int stageNumber, int expectedWaves)
    {
        var configuration = WaveConfiguration.CreateForStage(Map, stageNumber);

        Assert.Equal(expectedWaves, configuration.Waves.Count);
    }

    [Fact]
    public void LaterStagesCycleEstablishedCompositionsWithoutScalingStats()
    {
        var stageOne = WaveConfiguration.CreateForStage(Map, 1);
        var laterStage = WaveConfiguration.CreateForStage(Map, 7);

        for (var index = 0; index < laterStage.Waves.Count; index++)
        {
            var expected = stageOne.Waves[index % stageOne.Waves.Count];
            var actual = laterStage.Waves[index];
            Assert.Equal(expected.SpawnIntervalSeconds, actual.SpawnIntervalSeconds);
            Assert.True(expected.Enemies.SequenceEqual(actual.Enemies));
        }
    }
}
