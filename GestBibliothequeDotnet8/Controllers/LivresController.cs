using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestBibliothequeDotnet8.Utilitaires;
using GestBibliothequeDotnet8.Repositories;

namespace GestBibliothequeDotnet8.Controllers
{
    public class LivresController : Controller
    {
        private readonly ILivres _livresService;
        private readonly ICategories _categoriesService;

        public LivresController(ILivres livresService, ICategories categoriesService)
        {
            _livresService = livresService;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var livres = await _livresService.ObtenirLivresAvecCategories();
            return View(livres);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var livresAvecCategories = await _livresService.ObtenirLivresAvecCategories(id);
            var livre = livresAvecCategories.FirstOrDefault(l => l.ID == id);
            if (livre == null) return NotFound(); 
            return View(livre);
        }
        public async Task<IActionResult> AjouterAsync()
        {
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Livres livre)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _livresService.AddAsync(livre);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                   GestionErreurs.GererErreur(ex, this);
                }
            }
            ViewBag.Categories = await _categoriesService.GetAllAsync();
            return View(livre);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var livre = await _livresService.GetByIdAsync(id);
            if (livre == null) return NotFound();


            var categories = await _categoriesService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "ID", "Libelle", livre.IDCategorie);

            return View(livre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Livres livre)
        {            
            if (ModelState.IsValid)
            {
                try
                {
                    await _livresService.UpdateAsync(livre);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
        
            var categories = await _categoriesService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "ID", "Libelle", livre.IDCategorie);
            return View(livre);
        }

        public async Task<IActionResult> Supprimer(Guid id)
        {
            var livre = await _livresService.GetByIdAsync(id);
            if (livre == null) return NotFound();
            return View(livre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirmation(Guid id)
        {
            try
            {
                await _livresService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
                var livre = await _livresService.GetByIdAsync(id);
                return View("Supprimer", livre);
            }
        }

    }

}
