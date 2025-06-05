
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

		Tarefa = tarefa ?? new Tarefa();
		BindingContext = tarefa;

		StatusPicker.ItemsSource = Enum.GetValues(typeof(Status)).Cast<Status>().ToList();
		UsuarioPicker.ItemsSource = UsuarioServico.Instancia().Todos();

		StatusPicker.SelectedItem = Tarefa.Status ?? Status.Backlog;
		UsuarioPicker.SelectedItem = Tarefa.Usuario;
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(TituloEntry.Text))
		{
			await DisplayAlert("Atenção", "O título não pode ser vazio!", "OK");
			TituloEntry.Focus();
			return;
		}

		Tarefa.Titulo = TituloEntry.Text;
		Tarefa.Descricao = DescricaoEditor.Text;
		if (StatusPicker.SelectedItem != null)
		{
			Tarefa.Status = (Status)StatusPicker.SelectedItem;
		}
		else
		{
			Tarefa.Status = Status.Backlog;
		}

		if (UsuarioPicker.SelectedItem != null)
		{
			Tarefa.UsuarioId = ((Usuario)UsuarioPicker.SelectedItem).Id;
		}

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

