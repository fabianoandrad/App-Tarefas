using Tarefas.Constants;
using Tarefas.Servicos;
using Tarefas.Models;
using System.Windows.Input;

namespace Tarefas;

public partial class MainPage : ContentPage
{
	DatabaseServico<Tarefa> _tarefaServico;
	public ICommand AlertCommand { get; set; }

	public MainPage()
	{
		InitializeComponent();
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);

		AlertCommand = new Command<Tarefa>(ExecuteAlertCommand);
		TarefasCollectionTable.BindingContext = this;

		CarregarTarefas();
	}

	private void ExecuteAlertCommand(Tarefa tarefa)
	{
		DisplayAlert("Alerta", $"Tarefa: {tarefa.Titulo}", "Ok");
	}

	private async void CarregarTarefas()
	{
		var tarefas = await _tarefaServico.TodosAsync();
		TarefasCollectionTable.ItemsSource = tarefas;
	}

	
}

