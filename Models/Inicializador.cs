using System;
using IDM.Models;
using Microsoft.AspNetCore.Identity;


namespace IDM.Models
{
    public static class Inicializador
    {
        private static void InicializarPerfis(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!roleManager.RoleExistsAsync("administrador").Result)
            {
                var perfil1 = new IdentityRole<int>();
                perfil1.Name = "administrador";
                roleManager.CreateAsync(perfil1).Wait();
            }

            if (!roleManager.RoleExistsAsync("coordenador").Result)
            {
                var perfil2 = new IdentityRole<int>();
                perfil2.Name = "coordenador";
                roleManager.CreateAsync(perfil2).Wait();
            }

            if (!roleManager.RoleExistsAsync("colaborador").Result)
            {
                var perfil3 = new IdentityRole<int>();
                perfil3.Name = "colaborador";
                roleManager.CreateAsync(perfil3).Wait();
            }
        }


        private static void InicializarDados(UserManager<ColaboradorModel> userManager, IDMdbContext _context)
        {
            if (userManager.FindByNameAsync("admin@email.com").Result == null)
            {
                //Perfis
                var adm = new ColaboradorModel();
                adm.Id = 1;
                adm.UserName = "admin@email.com";
                adm.Email = "admin@email.com";
                adm.NomeCompleto = "Administrador do Sistema";
                adm.DataNascimento = new DateTime(1980, 1, 1);
                adm.PhoneNumber = "999999991";
                var resultado = userManager.CreateAsync(adm, "Admin@123").Result;
                if (resultado.Succeeded)
                {
                    userManager.AddToRoleAsync(adm, "administrador").Wait();
                }

                var coord = new ColaboradorModel();
                coord.Id = 2;
                coord.UserName = "coordenador@email.com";
                coord.Email = "coordenador@email.com";
                coord.NomeCompleto = "Coordenador Emp";
                coord.DataNascimento = new DateTime(1980, 2, 1);
                coord.PhoneNumber = "999999992";
                resultado = userManager.CreateAsync(coord, "Coord@123").Result;
                if (resultado.Succeeded)
                {
                    userManager.AddToRoleAsync(coord, "coordenador").Wait();
                }

                var coord2 = new ColaboradorModel();
                coord2.Id = 3;
                coord2.UserName = "coordenador2@email.com";
                coord2.Email = "coordenador2@email.com";
                coord2.NomeCompleto = "Coordenador2 Emp";
                coord2.DataNascimento = new DateTime(1980, 3, 1);
                coord2.PhoneNumber = "999999993";
                resultado = userManager.CreateAsync(coord2, "Coord2@123").Result;
                if (resultado.Succeeded)
                {
                    userManager.AddToRoleAsync(coord2, "coordenador").Wait();
                }

                var colab = new ColaboradorModel();
                colab.Id = 4;
                colab.UserName = "colaborador@email.com";
                colab.Email = "colaborador@email.com";
                colab.NomeCompleto = "Colaborador Emp";
                colab.DataNascimento = new DateTime(1980, 4, 1);
                colab.PhoneNumber = "999999994";
                resultado = userManager.CreateAsync(colab, "Colab@123").Result;
                if (resultado.Succeeded)
                {
                    userManager.AddToRoleAsync(colab, "colaborador").Wait();
                }

                //Niveis
                var nivel0 = new NivelModel { Classificacao = 0, Descricao = "Consumíveis" };
                _context.Niveis.Add(nivel0);

                var nivel1 = new NivelModel { Classificacao = 1, Descricao = "Eletrónicos" };
                _context.Niveis.Add(nivel1);

                var nivel2 = new NivelModel { Classificacao = 2, Descricao = "Localização físicas ou Veículos" };
                _context.Niveis.Add(nivel2);

                //Fornecedor
                var fornecedor = new FornecedorModel { Nome = "Fornecedor Exemplo",
                    Email = "fornecedor@email.com",
                    Endereco = "Endereço de fornecedor",
                    Telefone = "+999999999"};
                    
                _context.Fornecedores.Add(fornecedor);

                _context.SaveChanges();
            }
        }

        public static void InicializarIdentity(UserManager<ColaboradorModel> userManager,
            RoleManager<IdentityRole<int>> roleManager, IDMdbContext _context)
        {
            InicializarPerfis(roleManager);
            InicializarDados(userManager, _context);
        }
    }
}