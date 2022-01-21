
using System;
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IDM.Models
{
    [Authorize]
    public class RequisicaoController : Controller
    {
        private readonly UserManager<ColaboradorModel> _userManager;
        private readonly SignInManager<ColaboradorModel> _signInManager;
        private readonly IDMdbContext _context;

        public RequisicaoController(UserManager<ColaboradorModel> userManager,
            SignInManager<ColaboradorModel> signInManager, IDMdbContext context)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var colaborador = await _userManager.FindByIdAsync(id.ToString());
                if (colaborador != null)
                {
                    var requisicoes = await _context.Requisicoes
                        .Where(p => p.IdColaborador == id)
                        .OrderByDescending(x => x.IdRequisicao)
                        .AsNoTracking().ToListAsync();

                    ViewBag.Colaborador = colaborador;
                    return View("IndexRel", requisicoes);
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Colaborador não encontrado.", TypeMensagem.Erro);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(await _context.Requisicoes.OrderByDescending(x => x.IdRequisicao).AsNoTracking().ToListAsync());
            }
        }

        [Authorize(Roles = "coordenador,colaborador")]
        [HttpGet]
        public async Task<IActionResult> Criar(int? id)
        {
            var produtos = _context.Produtos.OrderBy(x => x.Descricao).AsNoTracking().ToList();

            var produtoSelectList = new SelectList(produtos,
                nameof(ProdutoModel.IdProduto),
                nameof(ProdutoModel.Descricao));

            ViewBag.Produto = produtoSelectList;

            if (id.HasValue)
            {
                var requisicao = await _context.Requisicoes.FindAsync(id);
                if (requisicao == null)
                {
                    return NotFound();
                }
                return View(requisicao);
            }
            else
            {

                return View(new RequisicaoModel());
            }
        }

        private bool RequisicaoExiste(int id)
        {
            return _context.Requisicoes.Any(x => x.IdRequisicao == id);
        }

        [Authorize(Roles = "coordenador, colaborador")]
        [HttpPost]
        public async Task<IActionResult> Criar(int? id, [FromForm] RequisicaoModel req)
        {

            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (RequisicaoExiste(id.Value))
                    {
                        _context.Requisicoes.Update(req);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Requisição alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar requisição.", TypeMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Requisição não encontrada.", TypeMensagem.Erro);
                    }
                }
                else //criação
                {
                    //var UserLogado = _userManager.FindByLoginAsync(u => u);
                    RequisicaoModel requis = new RequisicaoModel();

                    requis.IdProduto = req.IdProduto;
                    requis.Quantidade = req.Quantidade;
                    requis.IdColaborador = req.IdColaborador;

                    requis.Produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == requis.IdProduto);
                    requis.Colaborador = _context.Colaboradores.FirstOrDefault(c => c.Id == requis.IdColaborador);
                    requis.Produto.Nivel = _context.Niveis.FirstOrDefault(n => n.IdNivel == requis.Produto.IdNivel);

                    requis.ColabName = requis.Colaborador.NomeCompleto;
                    requis.ProdutoName = requis.Produto.Descricao;

                    //ViewBag.Niveis = niveisSelectList;

                    /* var total = ; */
                    requis.ValorTotal = (double)req.Quantidade * requis.Produto.Valor;

                    EstoqueModel stock = _context.Estoques.FirstOrDefault(s => s.IdProduto == requis.IdProduto);

                    if (requis.Quantidade > stock.Quantidade)
                    {
                        requis.EstadoReq = Estado.Sem_Estoque;
                        TempData["mensagem"] = MensagemModel.Serializar("Não temos essa quantidade em estoque.", TypeMensagem.Erro);
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        if (requis.Produto.Nivel.Classificacao == 0)
                        {
                            requis.EstadoReq = Estado.Aprovado;
                            stock.Quantidade = stock.Quantidade - requis.Quantidade;
                            TempData["mensagem"] = MensagemModel.Serializar("A sua requisição foi enviada e aprovada.", TypeMensagem.Info);
                        }
                        else if (requis.Produto.Nivel.Classificacao == 1)
                        {
                            if (User.IsInRole("coordenador"))
                            {
                                requis.EstadoReq = Estado.Aprovado;
                                stock.Quantidade = stock.Quantidade - requis.Quantidade;
                                TempData["mensagem"] = MensagemModel.Serializar("A sua requisição foi enviada e aprovada.", TypeMensagem.Info);

                            }
                            else if (User.IsInRole("colaborador"))
                            {
                                requis.EstadoReq = Estado.Pendente;
                                stock.Quantidade = stock.Quantidade - requis.Quantidade;
                                TempData["mensagem"] = MensagemModel.Serializar("A sua requisição foi enviada e aguarda aprovação.", TypeMensagem.Info);

                            }
                        }
                        else if (requis.Produto.Nivel.Classificacao == 2)
                        {
                            requis.EstadoReq = Estado.Pendente;
                            stock.Quantidade = stock.Quantidade - requis.Quantidade;
                            TempData["mensagem"] = MensagemModel.Serializar("A sua requisição foi enviada e aguarda aprovação.", TypeMensagem.Info);

                        }
                    }

                    _context.Requisicoes.Add(requis);
                    _context.Produtos.Update(requis.Produto);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                var prod = _context.Produtos.FirstOrDefault(p => p.IdProduto == req.IdProduto);

                RequisicaoModel requi = new RequisicaoModel()
                {
                    IdProduto = prod.IdProduto
                };
                return View(requi);
            }
            else
            {
                return View(req);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Requisição não informada.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var requisicao = await _context.Requisicoes.FindAsync(id);
            if (requisicao == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Requisição não encontrada.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(requisicao);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var requisicao = await _context.Requisicoes.FindAsync(id);
            if (requisicao != null)
            {
                EstoqueModel stock = _context.Estoques.FirstOrDefault(s => s.IdProduto == requisicao.IdProduto);

                stock.Quantidade = stock.Quantidade + requisicao.Quantidade;

                _context.Requisicoes.Remove(requisicao);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Requisição excluída.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a requisição.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Requisição não encontrada.", TypeMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var Requisicao = await _context.Requisicoes.FindAsync(id);
            if (Requisicao == null)
            {
                return NotFound();
            }
            return View(Requisicao);
        }

        [Authorize(Roles = "coordenador, administrador")]
        public async Task<IActionResult> Pedido(int? id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            return View(await _context.Requisicoes.OrderByDescending(x => x.IdRequisicao).Where(x => x.IdColaborador != usuario.Id).AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "coordenador, administrador")]
        public async Task<IActionResult> Aprovar(int? id)
        {
            if (id.HasValue) //aprovar especifico
            {
                var REQ = await _context.Requisicoes.FindAsync(id);

                var aprovacao = _context.Aprovacoes.FirstOrDefault(a => a.IdRequisicao == REQ.IdRequisicao);
                if (aprovacao == null)
                {
                    aprovacao = new AprovacaoModel();
                }

                /* aprovacao. ; */

                REQ.Produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == REQ.IdProduto);
                REQ.Produto.Nivel = _context.Niveis.FirstOrDefault(n => n.IdNivel == REQ.Produto.IdNivel);

                var aprovador = _context.Colaboradores.FirstOrDefault(c => c.Id == REQ.IdColaborador);

                aprovacao.NumerodeAprovacao++;
                aprovacao.IdRequisicao = REQ.IdRequisicao;

                var requerente = _context.Colaboradores.FirstOrDefault(c => c.Id == REQ.IdColaborador);
                var role = _userManager.GetRolesAsync(requerente).Result;

                if (REQ == null)
                {
                    return NotFound();
                }

                if (REQ.Produto.Nivel.Classificacao == 1)
                {
                    aprovacao.isAprovado = true;
                    /*  */
                    TempData["mensagem"] = MensagemModel.Serializar("Requisição aprovada.", TypeMensagem.Info);
                }
                else if (REQ.Produto.Nivel.Classificacao == 2)
                {
                    if (role.Contains("coordenador"))
                    {
                        aprovacao.isAprovado = true;
                        TempData["mensagem"] = MensagemModel.Serializar("Requisição aprovada.", TypeMensagem.Info);

                    }
                    else if (role.Contains("colaborador"))
                    {
                        if (User.IsInRole("coordenador"))
                        {
                            if (aprovacao.NumerodeAprovacao == 2)
                            {
                                aprovacao.isAprovado = true;
                                TempData["mensagem"] = MensagemModel.Serializar("Requisição aprovada.", TypeMensagem.Info);

                            }
                            else
                            {
                                aprovacao.isAprovado = false;
                                TempData["mensagem"] = MensagemModel.Serializar("Requisição aprovada, precisa ser aprovada por mais um coordenador.", TypeMensagem.Info);

                            }
                        }
                        else if (User.IsInRole("administrador"))
                        {
                            aprovacao.isAprovado = true;
                            TempData["mensagem"] = MensagemModel.Serializar("Requisição aprovada.", TypeMensagem.Info);

                        }

                    }
                }

                if (aprovacao.isAprovado)
                {
                    REQ.EstadoReq = Estado.Aprovado;
                    //stock.Quantidade = stock.Quantidade - REQ.Quantidade;
                }

                if (aprovacao.IdAprovacao == 0)
                {
                    _context.Aprovacoes.Add(aprovacao);
                }
                else
                {
                    _context.Aprovacoes.Update(aprovacao);
                }
                _context.Requisicoes.Update(REQ);
                _context.Colaboradores.Update(aprovador);
                _context.SaveChanges();

            }

            return RedirectToAction("Pedido");

        }

        [Authorize(Roles = "coordenador, administrador")]
        public async Task<IActionResult> Rejeitar(int? id)
        {
            if (id.HasValue) //aprovar especifico
            {
                var REQ = await _context.Requisicoes.FindAsync(id);

                EstoqueModel stock = _context.Estoques.FirstOrDefault(s => s.IdProduto == REQ.IdProduto);

                if (REQ == null)
                {
                    return NotFound();
                }


                REQ.EstadoReq = Estado.Rejeitado;
                stock.Quantidade = stock.Quantidade + REQ.Quantidade;

                _context.Requisicoes.Update(REQ);
                _context.SaveChanges();

                TempData["mensagem"] = MensagemModel.Serializar("Requisição Rejeitada.", TypeMensagem.Erro);

            }

            return RedirectToAction("Pedido");

        }

    }
}