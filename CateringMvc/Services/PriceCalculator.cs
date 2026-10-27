using CateringMvc.Models;

namespace CateringMvc.Services;

public static class PriceCalculator
{
    /// <summary>
    /// Ek item ka amount (cooking charges Price Settings se pehle se bhari hoti hain, order mein badli ja sakti hain):
    /// Biryani:   rice kg x (rice + sabz masalay + garam masalay) + meat kg x meat price + cooking
    /// WhiteRice: rice kg x rice price + cooking
    /// Zarda:     rice kg x (rice + sweets) + cooking
    /// Korma:     meat kg x meat price + cooking   (cooking = meat kg x cooking per kg, controller nikalta hai)
    /// BeefKorma: meat kg x beef meat price + cooking (cooking = meat kg x cooking per kg)
    /// Haleem:    kg x haleem price + cooking
    /// ChanaDaig: rice kg x (rice + sabz masalay + garam masalay) + cooking
    /// </summary>
    public static decimal LineAmount(PriceSetting p, OrderType type, decimal riceKg, decimal meatKg, decimal quantityKg, decimal cookingCharge)
    {
        decimal amount = type switch
        {
            OrderType.Korma => meatKg * p.KormaMeatPerKg + cookingCharge,
            OrderType.BeefKorma => meatKg * p.BeefKormaMeatPerKg + cookingCharge,
            OrderType.WhiteRice => riceKg * p.WhiteRicePerKg + cookingCharge,
            OrderType.SimpleRice => riceKg * p.SimpleRicePerKg + cookingCharge,
            OrderType.GurChawal => riceKg * (p.GurChawalRicePerKg + p.GurChawalGurPerKg) + cookingCharge,
            OrderType.Zarda => riceKg * (p.ZardaRicePerKg + p.ZardaSweetsPerKg) + cookingCharge,
            OrderType.Haleem => quantityKg * p.HaleemPerKg + cookingCharge,
            OrderType.ChanaDaig => riceKg * (p.ChanaDaigPerKg + p.ChanaDaigSabzMasalaPerKg + p.ChanaDaigGaramMasalaPerKg) + cookingCharge,
            _ => riceKg * (p.BiryaniRicePerKg + p.BiryaniSabzMasalaPerKg + p.BiryaniGaramMasalaPerKg)
                 + meatKg * p.BiryaniMeatPerKg + cookingCharge
        };
        return Math.Round(amount, 2);
    }
}
