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
	DatabaseServico<Anexo> _anexoServico;
	public TaskDetailsPage(Tarefa tarefa)
	{
		InitializeComponent();

		Tarefa = tarefa;
		BindingContext = this;
		_tarefaServico = new DatabaseServico<Tarefa>(Db.DB_PATH);
		_comentarioServico = new DatabaseServico<Comentario>(Db.DB_PATH);
		_anexoServico = new DatabaseServico<Anexo>(Db.DB_PATH);
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
		CarregarImagens();
		CarregarLocalizacoes();
	}

	private async void CarregarComentarios()
	{
		ComentariosCollection.ItemsSource = await _comentarioServico.Query().Where(c => c.TarefaId == Tarefa.Id).ToListAsync();
	}

	private async void CarregarImagens()
	{
		var fotos = await _anexoServico.Query().Where(a => a.TarefaId == Tarefa.Id && !string.IsNullOrEmpty(a.Arquivo)).ToListAsync();
		if (fotos.Count > 0)
		{
			FotosFrame.IsVisible = true;
			FotosCollection.ItemsSource = fotos;
		}
		else
		{
			FotosFrame.IsVisible = false;
		}
	}

		private async void CarregarLocalizacoes()
	{
		var localizacoes = await _anexoServico.Query().Where(a => a.TarefaId == Tarefa.Id && string.IsNullOrEmpty(a.Arquivo)).ToListAsync();
		if (localizacoes.Count > 0)
		{
			LocalizacaoFrame.IsVisible = true;
			LocalizacaoCollection.ItemsSource = localizacoes;
		}
		else
		{
			LocalizacaoFrame.IsVisible = false;
		}
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

	private async void TirarFotoClicked(object sender, EventArgs e)
	{
		try
		{
			//verfica se a camera está disponivel no dispositivo
			if (MediaPicker.Default.IsCaptureSupported)
			{
				// Tira uma foto e obtem o arquivo resultante
				FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

				if (photo != null)
				{
					// cria uma stream a partir do arquivo da foto
					using Stream stream = await photo.OpenReadAsync();

					// define o diretorio e o nome do arquivo onde a foto será salva
					string directory = FileSystem.AppDataDirectory;
					string fileName = Path.Combine(directory, $"{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.jpg");

					using FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
					await stream.CopyToAsync(fileStream);

					await _anexoServico.IncluirAsync(new Anexo
					{
						Arquivo = fileName,
						TarefaId = Tarefa.Id
					});
					CarregarImagens();
				}
				else
				{
					await DisplayAlert("Erro", "Não foi possível tirar a foto", "Fechar");
				}
			}
		}
		catch (FeatureNotSupportedException fnsEx)
		{
			await DisplayAlert("Erro", "A captura de fotos não é suportada neste dispositivo. - " + fnsEx.Message, "Ok");
		}
		catch (PermissionException pEx)
		{
			await DisplayAlert("Erro", "Permissão para acessar a camera não concedida. - " + pEx.Message, "Ok");
		}
		catch (Exception ex) {
			await DisplayAlert("Erro", "Ocorreu um erro inesperado. - " + ex.Message, "Ok");
		}
	}

	private async void LabelLinkGoogleMaps_Tapped(object sender, EventArgs e)
	{
		var label = sender as Label;
		if (label != null)
		{
			var url = label.Text.Split('-')[1].Trim();
			if (!string.IsNullOrEmpty(url)) {
				await Launcher.OpenAsync(new Uri(url));
			}
		}
	}

	private async void GPSClicked(object sender, EventArgs e)
	{
		var confirmado = await DisplayAlert("Localização", $"Confirma a captura da sua localização?", "Localizar", "Cancelar");

		if (confirmado)
		{
			LocalizacaoButton.Text = "Carregando...";
			LocalizacaoButton.IsEnabled = false;

			try
			{
				var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
				if (status != PermissionStatus.Granted)
				{
					status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
					if (status != PermissionStatus.Granted)
					{
						await DisplayAlert("Permissão de localização", "Permisssão de acesso à localização não concedida.", "Ok");
						return;
					}
				}

				var request = new GeolocationRequest(GeolocationAccuracy.Medium);
				var location = await Geolocation.GetLocationAsync(request);

				if (location != null)
				{
					await _anexoServico.IncluirAsync(new Anexo
					{
						Latitude = location.Latitude,
						Longitude = location.Longitude,
						TarefaId = Tarefa.Id
					});

					CarregarLocalizacoes();
					await DisplayAlert("Localização", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}", "Ok");
				}
			}
			catch (FeatureNotSupportedException fnsEx)
			{
				await DisplayAlert("Erro", "GPS não suportado neste dispositivo. - " + fnsEx.Message, "Ok");
			}
			catch (FeatureNotEnabledException fneEx)
			{
				await DisplayAlert("Erro", "GPS não esta habilitado. - " + fneEx.Message, "Ok");
			}
			catch (PermissionException pEx)
			{
				await DisplayAlert("Erro", "Permissão de GPS negada. - " + pEx.Message, "Ok");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Erro", "Não foi possivel obter a localização. - " + ex.Message, "Ok");
			}
			finally
			{
				LocalizacaoButton.Text = "Pegar coordenadas do GPS";
				LocalizacaoButton.IsEnabled = true;
			}
		}
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

}

