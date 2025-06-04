
using Tarefas.Models;

namespace Tarefas.Pages;

public partial class TaskDetailsPage : ContentPage
{
	public Tarefa Tarefa { get; set; }
	public TaskDetailsPage(Tarefa tarefa)
	{
		InitializeComponent();

		Tarefa = tarefa;
		BindingContext = this;
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	
}

