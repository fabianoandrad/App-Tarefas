using Tarefas.Constants;
using Tarefas.Enums;
using Tarefas.Models;
using Tarefas.Servicos;

namespace Tarefas.Pages;

public partial class TaskDetailsPage : ContentPage
{
	public Tarefa Tarefa { get; set; }
	DatabaseServico<Tarefa> _tarefaServico;
	DatabaseServico<Comentario> _comentarioServico;
	public TaskDetailsPage(Tarefa tarefa)
	{
		InitializeComponent();

		Tarefa = tarefa;
		BindingContext = this;
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);
		_comentarioServico = new DatabaseServico<Comentario>(Db.DB_PATH);
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LabelTitulo.Text = Tarefa.Titulo;
		LabelNomeUsuario.Text = Tarefa.NomeUsuario;
		LabelDataCriacao.Text = Tarefa.DataCriacao.ToString();
		LabelDataAtualizacao.Text = Tarefa.DataAtualizacao.ToString();
		LabelStatus.Text = Tarefa.Status.ToString();
		LabelDescricao.Text = Tarefa.Descricao;
		UsuarioPicker.ItemsSource = UsuarioServico.Instancia().Todos();

		CarregarComentarios();
    }

	private async void CarregarComentarios()
	{
		ComentariosCollection.ItemsSource = await _comentarioServico.Query().Where(c => c.TarefaId == Tarefa.Id).ToListAsync();
	}

	private async void AdicionarComentarioClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(NovoComentarioEditor.Text) || UsuarioPicker.SelectedIndex == -1)
		{
			await DisplayAlert("Erro", "Digite o comentario e selecione o usuario", "Fechar");
			return;
		}

		var usuario = (Usuario)UsuarioPicker.SelectedItem;

		await _comentarioServico.IncluirAsync(new Comentario
		{
			UsuarioId = usuario.Id,
			TarefaId = Tarefa.Id,
			Texto = NovoComentarioEditor.Text
		});

		NovoComentarioEditor.Text = string.Empty;
		UsuarioPicker.SelectedIndex = -1;

		CarregarComentarios();
	}

	private async void editClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new NewTaskPage(Tarefa));
	}

	private async void deleteClicked(object sender, EventArgs e)
	{
		bool confirm = await DisplayAlert("Confirmação", "Deseja excluir esta tarefa?", "Sim", "Não");
		if (confirm)
		{
			await _tarefaServico.DeleteAsync(Tarefa);
			await Navigation.PopAsync();
		}
	}

		private async void GoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
	
}

