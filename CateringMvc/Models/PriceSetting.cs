using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringMvc.Models;

public class PriceSetting
{
    public int Id { get; set; }

    // ----- Biryani: rice ke hisab se (per kg rice) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniRicePerKg { get; set; } = 400;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniSabzMasalaPerKg { get; set; } = 100;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniGaramMasalaPerKg { get; set; } = 80;

    // ----- Biryani: meat (per kg meat) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniMeatPerKg { get; set; } = 1800;

    // Purana field: ab istemal nahi hota. Purani database ke column ki wajah se yahan rakha hai.
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniCookingPerKg { get; set; } = 0;

    // ----- Cooking charges (Biryani, Zarda, Haleem, Chana Daig: har item par; order mein badli ja sakti hain) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BiryaniCookingCharge { get; set; } = 2000;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaCookingCharge { get; set; } = 1000;

    // ----- Haleem -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HaleemPerKg { get; set; } = 1200;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HaleemCookingCharge { get; set; } = 1000;

    // ----- Chana Daig (chana wali daig): rice ke hisab se, teeno rates per kg rice -----
    // (ChanaDaigPerKg = chana daig mein rice ka rate per kg rice)
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigPerKg { get; set; } = 600;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigSabzMasalaPerKg { get; set; } = 100;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigGaramMasalaPerKg { get; set; } = 80;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ChanaDaigCookingCharge { get; set; } = 1000;

    // ----- White Rice (sada chawal) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRicePerKg { get; set; } = 350;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal WhiteRiceCookingCharge { get; set; } = 500;

    // ----- Simple Rice (rice per kg + cooking per item) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRicePerKg { get; set; } = 300;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SimpleRiceCookingCharge { get; set; } = 500;

    // ----- Gor Wala Chawal (Zarda ki tarah: rice + gor per kg rice, cooking per item) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalRicePerKg { get; set; } = 400;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalGurPerKg { get; set; } = 150;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GurChawalCookingCharge { get; set; } = 1000;

    // ----- Zarda (per kg rice) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaRicePerKg { get; set; } = 400;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ZardaSweetsPerKg { get; set; } = 150;

    // ----- Korma / Beef Korma: meat aur cooking dono per kg -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal KormaMeatPerKg { get; set; } = 1800;

    // Korma ki cooking charges per kg meat.
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal KormaCookingPerKg { get; set; } = 250;

    // ----- Beef Korma (per kg meat) -----
    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BeefKormaMeatPerKg { get; set; } = 2200;

    [Range(0, 10000000, ErrorMessage = "Sahi price likhein.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BeefKormaCookingPerKg { get; set; } = 250;
}
