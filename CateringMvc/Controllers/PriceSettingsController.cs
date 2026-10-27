using CateringMvc.Data;
using CateringMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringMvc.Controllers;

public class PriceSettingsController : Controller
{
    private readonly AppDbContext _db;

    public PriceSettingsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _db.PriceSettings.AsNoTracking().FirstOrDefaultAsync() ?? new PriceSetting();
        return View(settings);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PriceSetting model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var s = await _db.PriceSettings.FirstOrDefaultAsync();
        if (s == null)
        {
            s = new PriceSetting();
            _db.PriceSettings.Add(s);
        }

        s.BiryaniRicePerKg = model.BiryaniRicePerKg;
        s.BiryaniSabzMasalaPerKg = model.BiryaniSabzMasalaPerKg;
        s.BiryaniGaramMasalaPerKg = model.BiryaniGaramMasalaPerKg;
        s.BiryaniMeatPerKg = model.BiryaniMeatPerKg;
        s.WhiteRicePerKg = model.WhiteRicePerKg;
        s.WhiteRiceCookingCharge = model.WhiteRiceCookingCharge;
        s.SimpleRicePerKg = model.SimpleRicePerKg;
        s.SimpleRiceCookingCharge = model.SimpleRiceCookingCharge;
        s.GurChawalRicePerKg = model.GurChawalRicePerKg;
        s.GurChawalGurPerKg = model.GurChawalGurPerKg;
        s.GurChawalCookingCharge = model.GurChawalCookingCharge;
        s.ZardaRicePerKg = model.ZardaRicePerKg;
        s.ZardaSweetsPerKg = model.ZardaSweetsPerKg;
        s.KormaMeatPerKg = model.KormaMeatPerKg;
        s.HaleemPerKg = model.HaleemPerKg;
        s.HaleemCookingCharge = model.HaleemCookingCharge;
        s.ChanaDaigPerKg = model.ChanaDaigPerKg;
        s.ChanaDaigCookingCharge = model.ChanaDaigCookingCharge;
        s.BiryaniCookingCharge = model.BiryaniCookingCharge;
        s.ZardaCookingCharge = model.ZardaCookingCharge;
        s.KormaCookingPerKg = model.KormaCookingPerKg;
        s.BeefKormaMeatPerKg = model.BeefKormaMeatPerKg;
        s.BeefKormaCookingPerKg = model.BeefKormaCookingPerKg;
        s.ChanaDaigSabzMasalaPerKg = model.ChanaDaigSabzMasalaPerKg;
        s.ChanaDaigGaramMasalaPerKg = model.ChanaDaigGaramMasalaPerKg;

        await _db.SaveChangesAsync();
        TempData["Saved"] = "Prices save ho gaye.";
        return RedirectToAction(nameof(Index));
    }
}
