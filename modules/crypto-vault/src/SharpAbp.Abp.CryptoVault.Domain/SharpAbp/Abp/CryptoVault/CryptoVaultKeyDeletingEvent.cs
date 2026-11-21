using System;

namespace SharpAbp.Abp.CryptoVault;

public sealed record CryptoVaultKeyDeletingEvent(Guid KeyId);
