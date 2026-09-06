namespace Asseta.Application.Common.Interfaces;

public interface IPairingCodeHasher
{
    string GeneratePairingCode();
    (string Hash, string Salt) HashPairingCode(string code);
    bool VerifyPairingCode(string code, string hash, string salt);
}
