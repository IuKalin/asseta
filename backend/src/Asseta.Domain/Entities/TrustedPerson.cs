using Asseta.Domain.Common;

namespace Asseta.Domain.Entities;

public class TrustedPerson : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public int TrustLevel { get; private set; } = 1;
    public Guid OwnerId { get; private set; }

    private TrustedPerson() { }

    public TrustedPerson(string fullName, string email, string phoneNumber, string relationship, int trustLevel, Guid ownerId)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Relationship = relationship;
        TrustLevel = trustLevel;
        OwnerId = ownerId;
    }
}
