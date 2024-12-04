using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndpointsArtistas(this WebApplication app)
    {
        
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

        app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody]ArtistaRequest artistaRequest) =>
        {
            var artista = new Artista(artistaRequest.Nome, artistaRequest.Bio);
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

        app.MapPut("/Artistas/{id}", ([FromServices] DAL<Artista> dal,[FromBody] ArtistaRequest artistaRequest, int id) =>
        {
            var artistaRecuperado = dal.RecuperarPor(a => a.Id.Equals(id));
            if (artistaRecuperado is null)
            {
                return Results.NotFound();
            }
            artistaRecuperado.Nome = artistaRequest.Nome;
            artistaRecuperado.Bio = artistaRequest.Bio;
            if (artistaRequest.FotoPerfil is not null)
            {
                artistaRecuperado.FotoPerfil = artistaRequest.FotoPerfil;
            }
            dal.Atualizar(artistaRecuperado);
            return Results.Ok();
        });

    }
}