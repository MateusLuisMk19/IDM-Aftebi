
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDM.Models
{
    [Authorize( Roles = "administrador")]
    public class FornecedorController : Controller
    {
        
        private readonly IDMdbContext _context;

        public FornecedorController(IDMdbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Fornecedores.OrderBy(x => x.Nome).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int? id)
        {
            if (id.HasValue)
            {
                var Fornecedor = await _context.Fornecedores.FindAsync(id);
                if (Fornecedor == null)
                {
                    return NotFound();
                }
                return View(Fornecedor);
            }
            return View(new FornecedorModel());
        }

        private bool FornecedorExiste(int id)
        {
            return _context.Fornecedores.Any(x => x.IdFornecedor == id);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(int? id, [FromForm] FornecedorModel fornecedor)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (FornecedorExiste(id.Value))
                    {
                        _context.Fornecedores.Update(fornecedor);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Fornecedor editado.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao editar fornecedor.", TypeMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Fornecedor não encontrado.", TypeMensagem.Erro);
                    }
                }
                else
                {
                    _context.Fornecedores.Add(fornecedor);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Novo fornecedor criado.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao criar fornecedor.", TypeMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(fornecedor);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Fornecedor não informado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Fornecedor não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(fornecedor);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor != null)
            {
                _context.Fornecedores.Remove(fornecedor);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Fornecedor excluído.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o fornecedor.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Fornecedor não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var Fornecedor = await _context.Fornecedores.FindAsync(id);
            if (Fornecedor == null)
            {
                return NotFound();
            }
            return View(Fornecedor);
        }

    }
}