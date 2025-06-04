
using Tarefas.Constants;
using Tarefas.Enums;
using Tarefas.Models;
using Tarefas.Servicos;

namespace Tarefas.Pages;

public partial class NewTaskPage : ContentPage
{
	DatabaseServico<Tarefa> _tarefaServico;
	public Tarefa Tarefa { get; set; }
	public NewTaskPage(Tarefa tarefa = null)
	{
		InitializeComponent();
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);

		Tarefa = tarefa;
		BindingContext = this;

		StatusPicker.ItemsSource = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();
		UsuarioPicker.ItemsSource = UsuarioServico.Instancia().Todos();
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		if (Tarefa == null) Tarefa = new Tarefa();
		
		Tarefa.Titulo = TituloEntry.Text;
		Tarefa.Descricao = DescricaoEditor.Text;
		Tarefa.Status = (Status)StatusPicker.SelectedItem;
		Tarefa.UsuarioId = ((Usuario)UsuarioPicker.SelectedItem).Id;

		if (Tarefa.Id == 0)
		{
			await _tarefaServico.IncluirAsync(Tarefa);
		}
		else
		{
			await _tarefaServico.EditAsync(Tarefa);
		}

		await Navigation.PopAsync();
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	
}

