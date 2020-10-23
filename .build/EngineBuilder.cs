using Wyam.Core.Execution;

public static class EngineBuilder
{
    internal static Engine Create() => new Engine();

    internal static Engine WithSetting<T>(this Engine engine, string setting, T value)
    {
        engine.Settings[setting] = value;
        return engine;
    }

    internal static Engine WithConfiguration(this Engine engine, Build build)
    {
        new WyamConfiguration(engine, build);
        return engine;
    }
}