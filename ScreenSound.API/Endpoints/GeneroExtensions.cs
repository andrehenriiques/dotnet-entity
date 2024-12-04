using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;

public static class GeneroExtensions
{
    public static void AddEndpointGeneros(this WebApplication app)
    {

        app.MapGet("/Genero", ([FromServices] DAL<Genero> dal) =>
        {
            var genero = EntityListToResponseItemList(dal.Listar()); 
            return Results.Ok(genero);
        });

        app.MapPost("/Genero", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
        {
            var genero = new Genero(){Nome = generoRequest.Nome, Descricao = generoRequest.Descricao};
            dal.Adicionar(genero);
            return Results.Ok(genero);
        });

        app.MapDelete("/Genero/{id}", ([FromServices] DAL<Genero> dal, int id) =>
        {
            var genero = dal.RecuperarPor(a => a.Id.Equals(id));
            if (genero is null)
            {
                return Results.NotFound();
            }

            dal.Deletar(genero);
            return Results.NoContent();
        });
        
        app.MapPut("/Genero/{id}", ([FromServices] DAL<Genero> dal,[FromBody] GeneroRequest generoRequest, int id) =>
        {
            var generoRecuperado = dal.RecuperarPor(a => a.Id.Equals(id));
            if (generoRecuperado is null)
            {
                return Results.NotFound();
            }
            generoRecuperado.Nome = generoRequest.Nome;
            generoRecuperado.Descricao = generoRequest.Descricao;
            dal.Atualizar(generoRecuperado);
            return Results.Ok();
        });
    }

    private static ICollection<GeneroResponse> EntityListToResponseItemList(IEnumerable<Genero> generos)
    {
        return generos.Select(a => EntityToResponseItem(a)).ToList();
    }

    private static GeneroResponse EntityToResponseItem(Genero genero)
    {
        return new GeneroResponse(genero.Id, genero.Nome);
    }

    private static ICollection<GeneroRequest> EntityListToResponseList(IEnumerable<Genero> generos)
    {
        return generos.Select(a => EntityToResponse(a)).ToList();
    }

    private static GeneroRequest EntityToResponse(Genero genero)
    {
        return new GeneroRequest(genero.Nome!, genero.Descricao!);
    }
}