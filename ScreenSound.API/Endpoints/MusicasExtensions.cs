using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;

public static class MusicasExtensions
{
    public static void AddEndpointMusicas(this WebApplication app)
    {
        
        app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
        { 
            return Results.Ok(dal.Listar());
        });

        app.MapPost("/AdicionarMusicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody]MusicaRequest musicaRequest) =>
        {
            var musica = new Musica(musicaRequest.Nome)
            {
                AnoLancamento = musicaRequest.AnoLancamento,
                ArtistaId = musicaRequest.ArtistaId,
                Generos = musicaRequest.Generos is not null ? GeneroRequestConverter(musicaRequest.Generos, dalGenero) : new List<Genero>()
            };
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
        
        app.MapPut("/Musicas/{id}", ([FromServices] DAL<Musica> dal,[FromBody] MusicaRequest artistaRequest, int id) =>
        {
            var musicaRecuperado = dal.RecuperarPor(a => a.Id.Equals(id));
            if (musicaRecuperado is null)
            {
                return Results.NotFound();
            }
            musicaRecuperado.Nome = artistaRequest.Nome;
            musicaRecuperado.AnoLancamento = artistaRequest.AnoLancamento;
            dal.Atualizar(musicaRecuperado);
            return Results.Ok();
        });
    }

    private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {
        var generosList = new List<Genero>();
        foreach (var item in generos)
        {
            var entity = RequestToEntity(item);
            var genero = dalGenero.RecuperarPor(x=>x.Nome.ToLower().Equals(entity.Nome.ToLower()));
            if (genero is not null)
            {
                generosList.Add(genero);
            }
            else
            {
                generosList.Add(entity);
            }
        }

        return generosList;
        //return generos.Select(a => RequestToEntity(a)).ToList();
    }

    private static Genero RequestToEntity(GeneroRequest generoRequest)
    {
        return new Genero() {Nome = generoRequest.Nome, Descricao = generoRequest.Descricao};
    }
}