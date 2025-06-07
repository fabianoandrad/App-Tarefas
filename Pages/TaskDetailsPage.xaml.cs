using Tarefas.Constants;
using Tarefas.Enums;
using Tarefas.Models;
using Tarefas.Servicos;

namespace Tarefas.Pages;

public partial class TaskDetailsPage : ContentPage
{
	public Tarefa Tarefa { get; set; }
	DatabaseServico<Tarefa> _databaseServico;
	public TaskDetailsPage(Tarefa tarefa)
	{
		InitializeComponent();

		Tarefa = tarefa;
		BindingContext = this;
		_databaseServico = new DatabaseServico<Tarefa>(Db.DB_PATH);
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
			await _databaseServico.DeleteAsync(Tarefa);
			await Navigation.PopAsync();
		}
	}

		private async void GoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
	
}

