using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();
builder.Services.AddTransient<DAL<Musica>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
{ 
    return Results.Ok(dal.Listar());
});

app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
{
    if (string.IsNullOrEmpty(nome))
    {
        return Results.BadRequest("O nome do artista não pode ser nulo ou vazio.");
    }
    
    try
    { 
        var artista = dal.RecuperarPor(a => a.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (artista is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(artista);
    }
    catch (Exception ex)
    {
        // Loga o erro para debug
        return Results.Problem($"Erro ao buscar o artista: {ex.Message}");
    }
});

app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody]Artista artista) =>
{
    dal.Adicionar(artista);
    return Results.Ok(artista);
});

app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
{
    var artista = dal.RecuperarPor(a => a.Id.Equals(id));
    if (artista is null)
    {
        return Results.NotFound();
    }

    dal.Deletar(artista);
    return Results.NoContent();
});

app.MapPut("/Artistas/{id}", ([FromServices] DAL<Artista> dal,[FromBody] Artista artista, int id) =>
{
    var artistaRecuperado = dal.RecuperarPor(a => a.Id.Equals(id));
    if (artistaRecuperado is null)
    {
        return Results.NotFound();
    }
    artistaRecuperado.Nome = artista.Nome;
    artistaRecuperado.Bio = artista.Bio;
    artistaRecuperado.FotoPerfil = artista.FotoPerfil;
    dal.Atualizar(artistaRecuperado);
    return Results.Ok();
});


app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
{ 
    return Results.Ok(dal.Listar());
});

app.MapPost("/AdicionarMusicas/{artistaId}", ([FromServices] DAL<Musica> dal, [FromBody]Musica musica) =>
{
    dal.Adicionar(musica);
    return Results.Ok(musica);
});

app.MapPost("/AdicionarMusicasNoArtista/{artistaId}", ([FromServices] DAL<Artista> artistaDAL,[FromServices] DAL<Musica> dal, [FromBody]Musica musica, int ArtistaId) =>
{
    dal.Adicionar(musica);
    
    var artistaRecuperado = artistaDAL.RecuperarPor(a => a.Id.Equals(ArtistaId));
    artistaRecuperado.AdicionarMusica(new Musica(musica.Nome) { AnoLancamento = Convert.ToInt32(musica.AnoLancamento) });
    artistaDAL.Atualizar(artistaRecuperado);
    return Results.Ok(musica);
});

app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
{
    var musica = dal.RecuperarPor(a => a.Id.Equals(id));
    if (musica is null)
    {
        return Results.NotFound();
    }

    dal.Deletar(musica);
    return Results.NoContent();
});
app.Run();