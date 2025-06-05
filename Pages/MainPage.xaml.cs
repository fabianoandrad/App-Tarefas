using Tarefas.Constants;
using Tarefas.Servicos;
using Tarefas.Models;
using System.Windows.Input;

namespace Tarefas.Pages;

public partial class MainPage : ContentPage
{
	DatabaseServico<Tarefa> _tarefaServico;
	public ICommand NavigateToDetailsCommand { get; private set; }
	public ICommand NavigateToChangeCommand { get; private set; }
	public ICommand DeleteCommand { get; private set; }

	public MainPage()
	{
		InitializeComponent();
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);

		NavigateToDetailsCommand = new Command<Tarefa>(async (tarefa) => await NavigateToDetails(tarefa));
		NavigateToChangeCommand = new Command<Tarefa>(async (tarefa) => await NavigateToChange(tarefa));
		DeleteCommand = new Command<Tarefa>(ExecuteDeleteCommand);
		//CardBacklog.BindingContext = this;

		CarregarTarefas();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		CarregarTarefas();
    }

	private async void ExecuteDeleteCommand(Tarefa tarefa)
	{
		bool confirm = await DisplayAlert("Confirmação", "Deseja excuir esta tarefa?", "Sim", "Não");
		if (confirm)
		{
			await _tarefaServico.DeleteAsync(tarefa);
			CarregarTarefas();
		}
	}

	private async Task NavigateToChange(Tarefa tarefa)
	{
		await Navigation.PushAsync(new NewTaskPage(tarefa));
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
	}

	private async void NewClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new NewTaskPage());
	}

	
}

