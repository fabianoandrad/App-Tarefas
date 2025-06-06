using Tarefas.Constants;
using Tarefas.Servicos;
using Tarefas.Models;
using System.Windows.Input;

namespace Tarefas.Pages;

public partial class MainPage : ContentPage
{
	DatabaseServico<Tarefa> _tarefaServico;
	public ICommand NavigateToDetailsCommand { get; private set; }

	public MainPage()
	{
		InitializeComponent();
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);

		NavigateToDetailsCommand = new Command<Tarefa>(async (tarefa) => await NavigateToDetails(tarefa));

		//CardBacklog.BindingContext = this;

		CarregarTarefas();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		CarregarTarefas();
    }

	private async Task NavigateToDetails(Tarefa tarefa)
	{
		await Navigation.PushAsync(new TaskDetailsPage(tarefa));
	}

	private async void CarregarTarefas()
	{
		CardBacklog.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Enums.Status.Backlog).ToArrayAsync();
		CardAnalise.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Enums.Status.Analise).ToArrayAsync();
		CardParafazer.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Enums.Status.ParaFazer).ToArrayAsync();
		CardDesenvolvimento.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Enums.Status.Desenvolvimento).ToArrayAsync();
		CardFeito.ItemsSource = await _tarefaServico.Query().Where(t => t.Status == Enums.Status.Feito).ToArrayAsync();
	}

	private async void NewItemClicked(object sender, EventArgs e)
	{
		var btn = sender as Button;
		if (sender != null) {
			var status = (Enums.Status)btn.CommandParameter;
			await Navigation.PushAsync(new NewTaskPage(new Tarefa(){Status = status}));
			
		}
	}

	
}

