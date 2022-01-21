
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IDM.Models
{
    [Authorize( Roles = "administrador")]
    public class ProdutoController : Controller
    {
        private readonly IDMdbContext _context;

        public ProdutoController(IDMdbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Produtos.OrderBy(x => x.IdNivel)
            .Include(x => x.Fornecedor).Include(x => x.Nivel).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int? id)
        {
            var fornecedores = _context.Fornecedores.OrderBy(x => x.Nome).AsNoTracking().ToList();
            var niveis = _context.Niveis.OrderBy(x => x.Classificacao).AsNoTracking().ToList();

            var fornecedoreSelectList = new SelectList(fornecedores, 
                nameof(FornecedorModel.IdFornecedor), nameof(FornecedorModel.Nome));
            var niveisSelectList = new SelectList(niveis, 
                nameof(NivelModel.IdNivel), nameof(NivelModel.Classificacao));
            
            ViewBag.Fornecedores = fornecedoreSelectList;
            ViewBag.Niveis = niveisSelectList;
            
            if (id.HasValue)
            {
                var Produto = await _context.Produtos.FindAsync(id);
                if (Produto == null)
                {
                    return NotFound();
                }
                return View(Produto);
            }

            return View(new ProdutoModel());
        }

        private bool ProdutoExiste(int id)
        {
            return _context.Produtos.Any(x => x.IdProduto == id);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(int? id, [FromForm] ProdutoModel produto)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (ProdutoExiste(id.Value))
                    {
                        _context.Produtos.Update(produto);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Produto editado.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao editar produto.", TypeMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TypeMensagem.Erro);
                    }
                }
                else
                {
                    _context.Produtos.Add(produto);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Novo produto criado.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao criar produto.", TypeMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(produto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não informado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Produto excluído.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o produto.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

    }
}