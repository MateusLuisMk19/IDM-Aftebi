using System.Linq;
using System.Threading.Tasks;
using App.Extensions;
using IDM.Models;
using IDM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IDM.Controllers
{

    public class UsuarioController : Controller
    {
        private readonly UserManager<ColaboradorModel> _userManager;
        private readonly SignInManager<ColaboradorModel> _signInManager;
        private readonly IDMdbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UsuarioController(UserManager<ColaboradorModel> userManager,
            SignInManager<ColaboradorModel> signInManager, IDMdbContext context, RoleManager<IdentityRole<int>> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._context = context;
            this._roleManager = roleManager;
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<IActionResult> Cadastrar(string id)
        {/* 
            var departamentos = _context.Departamentos.OrderBy(x => x.Descricao).AsNoTracking().ToList();
            var cargos = _context.Cargos.OrderBy(x => x.Descricao).AsNoTracking().ToList();

            var departamentoselectList = new SelectList(departamentos, 
                nameof(DepartamentoModel.IdDepartamento), nameof(DepartamentoModel.Descricao));
            var cargosSelectList = new SelectList(cargos, 
                nameof(CargoModel.IdCargo), nameof(CargoModel.Descricao));
            
            ViewBag.departamentos = departamentoselectList;
            ViewBag.cargos = cargosSelectList; */


            if (!string.IsNullOrEmpty(id))
            {
                var usuarioBD = await _userManager.FindByIdAsync(id);
                if (usuarioBD == null)
                {
                    this.MostrarMensagem("Perfil não encontrado.", true);
                    return RedirectToAction("Index", "Home");
                }
                var usuarioVM = new CadastrarUsuarioViewModel
                {
                    Id = usuarioBD.Id,
                    NomeCompleto = usuarioBD.NomeCompleto,
                    Email = usuarioBD.Email,
                    Telefone = usuarioBD.PhoneNumber,
                    DataNascimento = usuarioBD.DataNascimento
                };
                return View(usuarioVM);
            }
            return View(new CadastrarUsuarioViewModel());
        }

        private bool EntidadeExiste(int id)
        {
            return (_userManager.Users.AsNoTracking().Any(u => u.Id == id));
        }

        private static void MapearCadastrarUsuarioViewModel(CadastrarUsuarioViewModel entidadeOrigem, ColaboradorModel entidadeDestino)
        {
            entidadeDestino.NomeCompleto = entidadeOrigem.NomeCompleto;
            entidadeDestino.DataNascimento = entidadeOrigem.DataNascimento;
            entidadeDestino.NormalizedUserName = entidadeOrigem.Email.ToUpper().Trim();
            entidadeDestino.UserName = entidadeOrigem.Email;
            entidadeDestino.Email = entidadeOrigem.Email;
            entidadeDestino.NormalizedEmail = entidadeOrigem.Email.ToUpper().Trim();
            entidadeDestino.PhoneNumber = entidadeOrigem.Telefone;
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
        [FromForm] CadastrarUsuarioViewModel usuarioVM)
        {

            //se for alteração, não tem senha e confirmação de senha
            if (usuarioVM.Id > 0)
            {
                ModelState.Remove("Senha");
                ModelState.Remove("ConfSenha");
            }

            if (ModelState.IsValid)
            {
                if (EntidadeExiste(usuarioVM.Id))
                {
                    var usuarioBD = await _userManager.FindByIdAsync(usuarioVM.Id.ToString());
                    if ((usuarioVM.Email != usuarioBD.Email) &&
                        (_userManager.Users.Any(u => u.NormalizedEmail == usuarioVM.Email.ToUpper().Trim())))
                    {
                        ModelState.AddModelError("Email",
                            "Já existe um perfil cadastrado com este e-mail.");
                        return View(usuarioVM);
                    }
                    MapearCadastrarUsuarioViewModel(usuarioVM, usuarioBD);

                    var resultado = await _userManager.UpdateAsync(usuarioBD);
                    if (resultado.Succeeded)
                    {
                        this.MostrarMensagem("Perfil alterado com sucesso.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.MostrarMensagem("Não foi possível alterar o Perfil.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(usuarioVM);
                    }
                }
                else
                {
                    var usuarioBD = await _userManager.FindByEmailAsync(usuarioVM.Email);
                    if (usuarioBD != null)
                    {
                        ModelState.AddModelError("Email",
                            "Já existe um Perfil cadastrado com este e-mail.");
                        return View(usuarioBD);
                    }

                    usuarioBD = new ColaboradorModel();
                    MapearCadastrarUsuarioViewModel(usuarioVM, usuarioBD);

                    var resultado = await _userManager.CreateAsync(usuarioBD, usuarioVM.Senha);
                    if (resultado.Succeeded)
                    {
                        var result = await _userManager.AddToRoleAsync(usuarioBD, "colaborador");
                        if (result.Succeeded)
                        {
                            this.MostrarMensagem("Perfil cadastrado com sucesso. Use suas credenciais para entrar no sistema.");
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        this.MostrarMensagem("Erro ao cadastrar Perfil.", true);
                        foreach (var error in resultado.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(usuarioVM);
                    }
                }
            }
            else
            {
                return View(usuarioVM);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel login)
        {
            var admins = (await _userManager.GetUsersInRoleAsync("administrador"))
                .Select(u => u.UserName);
            var coord = (await _userManager.GetUsersInRoleAsync("coordenador"))
                .Select(u => u.UserName);

            ViewData["Coordenadores"] = coord;
            ViewData["Administradores"] = admins;

            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(login.Usuario, login.Senha, login.Lembrar, false);
                if (resultado.Succeeded)
                {
                    login.ReturnUrl = login.ReturnUrl ?? "~/";
                    return LocalRedirect(login.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        "Tentativa de login inválida. Reveja seus dados de acesso e tente novamente.");
                    return View(login);
                }
            }
            else
            {
                return View(login);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.Where(x => x.Id != 1).AsNoTracking().ToListAsync();
            /* var admins = (await _userManager.GetUsersInRoleAsync("administrador"))
                .Select(u => u.UserName);
            var coord = (await _userManager.GetUsersInRoleAsync("coordenador"))
                .Select(u => u.UserName);

            ViewBag.Coordenadores = coord;
            ViewBag.Administradores = admins; */
            return View(usuarios);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                this.MostrarMensagem("Perfil não informado.", true);
                return RedirectToAction(nameof(Index));
            }

            if (!EntidadeExiste(id.Value))
            {
                this.MostrarMensagem("Perfil não encontrado.", true);
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id.ToString());

            return View(usuario);
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<IActionResult> ExcluirPost(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                var resultado = await _userManager.DeleteAsync(usuario);
                if (resultado.Succeeded)
                {
                    this.MostrarMensagem("Perfil excluído com sucesso.");
                }
                else
                {
                    this.MostrarMensagem("Não foi possível excluir o perfil.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Perfil não encontrado.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult AcessoRestrito([FromQuery] string returnUrl)
        {
            return View(model: returnUrl);
        }

        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> AddCoordenador(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, "coordenador");
                if (resultado.Succeeded)
                {
                    var result = await _userManager.RemoveFromRoleAsync(usuario, "colaborador");

                    this.MostrarMensagem(
                        $"<b>{usuario.NomeCompleto}</b> agora é um Coordenador.");
                }
                else
                {
                    this.MostrarMensagem(
                        $"Não foi possível remover <b>{usuario.UserName}</b> de Coordenador.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Perfil não encontrado.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> RemCoordenador(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, "coordenador");
                if (resultado.Succeeded)
                {
                    var result = await _userManager.AddToRoleAsync(usuario, "colaborador");
                    if (result.Succeeded)
                    {
                        this.MostrarMensagem(
                    $"<b>{usuario.NomeCompleto}</b> agora é um Colaborador.");
                    }
                }
                else
                {
                    this.MostrarMensagem(
                        $"Não foi possível remover <b>{usuario.UserName}</b> de Coordenador.", true);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                this.MostrarMensagem("Perfil não encontrado.", true);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Perfil(int id)
        {
            var utilizador = await _context.Colaboradores.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }
    }
}