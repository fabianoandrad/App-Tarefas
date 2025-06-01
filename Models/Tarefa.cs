using Tarefas.Enums;

namespace Tarefas.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public int UsuarioId { get; set; }
    public Status Status { get; set; }

    public Tarefa()
    {
        this.DataCriacao = DateTime.Now;
        this.DataAtualizacao = DateTime.Now;
    }
}