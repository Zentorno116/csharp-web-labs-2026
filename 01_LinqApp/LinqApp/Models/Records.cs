namespace LinqApp.Models;

public record FitnessRecord(int Hours, int Year, int Month, int ClientId);

public record ApplicantRecord(int School, int Year, string Surname);

public record DebtorRecord(string Surname, int Apartment, decimal Debt);

public record GasStationRecord(string Company, int Brand, int Price, string Street);

public record EgeRecord(int School, string Surname, string Initials, int Math, int Russian, int Informatics);

public record SchoolGradeRecord(int GradeClass, string Subject, string Surname, string Initials, int Mark);

public record ConsumerRecord(int BirthYear, string Street, int ConsumerId);

public record ProductRecord(string Category, string Article, string Country);

public record DiscountRecord(int DiscountPercent, string Store, int ConsumerId);

public record PriceRecord(string Article, string Store, int Price);

public record PurchaseRecord(string Store, int ConsumerId, string Article);
