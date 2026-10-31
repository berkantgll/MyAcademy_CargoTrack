using System;

namespace CargoTrack.Entity.Enums;

[Flags]
public enum ExtraServiceType
{
    None = 0,
    SpecialPackaging = 1, // Özel koruyucu paketleme
    GiftWrapping = 2,     // Hediye paketi
    Insurance = 4         // Sigorta
}