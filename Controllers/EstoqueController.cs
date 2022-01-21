
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IDM.Models
{
    [Authorize]
    public class EstoqueController : Controller
    {
        private readonly IDMdbContext _context;

        public EstoqueController(IDMdbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Estoques.OrderBy(x => x.Produto.Descricao)
            .Include(x => x.Produto).Where(x => x.Quantidade > 0).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int? id)
        {
            var produtos = _context.Produtos.OrderBy(x => x.Descricao).AsNoTracking().ToList();

            var produtosSelectList = new SelectList(produtos, 
                nameof(ProdutoModel.IdProduto), nameof(ProdutoModel.Descricao));
           
            ViewBag.Produtos = produtosSelectList;
            
            if (id.HasValue)
            {
                var Estoque = await _context.Estoques.FindAsync(id);
                if (Estoque == null)
                {
                    return NotFound();
                }
                return View(Estoque);
            }

            return View(new EstoqueModel());
        }

        private bool EstoqueExiste(int id)
        {
            return _context.Estoques.Any(x => x.IdEstoque == id);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(int? id, [FromForm] EstoqueModel estoque)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (EstoqueExiste(id.Value))
                    {
                        _context.Estoques.Update(estoque);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Estoque editado.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao editar estoque.", TypeMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Estoque não encontrado.", TypeMensagem.Erro);
                    }
                }
                else
                {
                    _context.Estoques.Add(estoque);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Novo item adicionado ao estoque.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao criar estoque.", TypeMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(estoque);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Estoque não informado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var estoque = await _context.Estoques.FindAsync(id);
            if (estoque == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Estoque não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(estoque);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var estoque = await _context.Estoques.FindAsync(id);
            if (estoque != null)
            {
                _context.Estoques.Remove(estoque);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Item excluído do estoque.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o estoque.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Estoque não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

    }
}