using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tarefas.Models;

namespace Tarefas.Servicos
{
    public class UsuarioServico
    {
        private static UsuarioServico _usuarioServico = new UsuarioServico();
        private List<Usuario> _usuarios = new List<Usuario>();

        private UsuarioServico()
        {
            _usuarios.Add(new Usuario { Id = 1, Nome = "João" });
            _usuarios.Add(new Usuario { Id = 2, Nome = "Maria" });
            _usuarios.Add(new Usuario { Id = 3, Nome = "Fabiano" });
            _usuarios.Add(new Usuario { Id = 4, Nome = "Miguel" });
        }

        public static UsuarioServico Instancia()
        {
            return _usuarioServico;
        }

        public List<Usuario> Todos()
        {
            return _usuarios;
        }
    }
}