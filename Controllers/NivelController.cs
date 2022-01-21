
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDM.Models
{
    [Authorize( Roles = "administrador")]
    public class NivelController : Controller
    {
        private readonly IDMdbContext _context;

        public NivelController(IDMdbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Niveis.OrderBy(x => x.Classificacao).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int? id)
        {
            if (id.HasValue)
            {
                var Nivel = await _context.Niveis.FindAsync(id);
                if (Nivel == null)
                {
                    return NotFound();
                }
                return View(Nivel);
            }
            return View(new NivelModel());
        }

        private bool NivelExiste(int id)
        {
            return _context.Niveis.Any(x => x.IdNivel == id);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(int? id, [FromForm] NivelModel nivel)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (NivelExiste(id.Value))
                    {
                        _context.Niveis.Update(nivel);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Nivel editado.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao editar nivel.", TypeMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Nivel não encontrado.", TypeMensagem.Erro);
                    }
                }
                else
                {
                    _context.Niveis.Add(nivel);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Novo nivel criado.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao criar nivel.", TypeMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(nivel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nivel não informado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var nivel = await _context.Niveis.FindAsync(id);
            if (nivel == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nivel não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(nivel);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var nivel = await _context.Niveis.FindAsync(id);
            if (nivel != null)
            {
                _context.Niveis.Remove(nivel);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Nivel excluído.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o nivel.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nivel não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

    }
}