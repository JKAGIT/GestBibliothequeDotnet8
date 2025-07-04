using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Mvc;

namespace GestBibliothequeDotnet8.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategories _categoriesService;
        private readonly ILivres _livresService;

        public CategoriesController(ICategories categoriesService, ILivres livresService)
        {
            _categoriesService = categoriesService;
            _livresService = livresService;
        }      

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var paginatedResult = await _categoriesService.GetPagedAsync(pageNumber, pageSize);
            return View(paginatedResult); // Passez PaginatedResult au modèle de vue
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var categories = await ObtenirCategorie(id);
            if (categories == null) return NotFound();

            var livres = await _livresService.ObtenirLivresParCategorie(id);  
            categories.Livres = livres.ToList();

            return View(categories);
        }
        public IActionResult Ajouter()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Categories categorie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriesService.AddAsync(categorie);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            return View(categorie);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var categorie = await ObtenirCategorie(id); ;
            if (categorie == null) return NotFound();
            return View(categorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Categories categorie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriesService.UpdateAsync(categorie);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            return View(categorie);
        }

        public async Task<IActionResult> Supprimer(Guid id)
        {
            var categorie = await ObtenirCategorie(id);
            if (categorie == null)
            {
                return NotFound();
            }
            return View(categorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirmation(Guid id)
        {
            try
            {
                await _categoriesService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
                var categorie = await ObtenirCategorie(id); ;
                return View("Supprimer", categorie);
            }
        }
        #region Méthodes privées
        private async Task<Categories> ObtenirCategorie(Guid id)
        {
            try
            {
                var categorie = await _categoriesService.GetByIdAsync(id);
            if (categorie == null) return null;
            return categorie;
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
                return null;
            }
        }
        #endregion
    }
}
