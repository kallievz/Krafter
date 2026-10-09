using AditiKraft.Krafter.Backend.Features.Localization.Common;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static class LocalizationLanguageSeedData
{
    public static IEnumerable<Language> GetSeeds()
    {
        return
        [
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "af", Name = "Afrikaans", IsActive = true },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "en", Name = "English", IsActive = true },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "nr", Name = "isiNdebele", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "xh", Name = "isiXhosa", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "zu", Name = "isiZulu", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "ns", Name = "Sepedi", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "st", Name = "Sesotho", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "tn", Name = "Setswana", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "ss", Name = "Siswati", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "ts", Name = "Xitsonga", IsActive = false },
            new Language { Id = Guid.CreateVersion7().ToString(), Code = "ve", Name = "Tshivenda", IsActive = false }
        ];
    }
}
