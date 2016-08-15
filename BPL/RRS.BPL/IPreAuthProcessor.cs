using RRS.BEL;

namespace RRS.BPL
{
    public interface IPreAuthProcessor
    {
        AuthResponse ProcessPreAuth(AuthRequest request);
    }
}
