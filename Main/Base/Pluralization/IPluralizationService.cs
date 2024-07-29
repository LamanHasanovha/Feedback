namespace Main.Base.Pluralization;

public interface IPluralizationService
{
    string Pluralize(string singular);

    string Singularize(string plural);
}
