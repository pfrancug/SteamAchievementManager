namespace SAM.Backend.Steam;

public interface INativeWrapper
{
    void SetupFunctions(IntPtr objectAddress);
}
