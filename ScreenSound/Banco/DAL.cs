namespace ScreenSound.Banco;

internal class DAL<T> where T : class
{
    protected readonly ScreenSoundContext _context;

    public DAL(ScreenSoundContext context)
    {
        _context = context;
    }

    public IEnumerable<T> Listar()
    {
        return _context.Set<T>().ToList();
    }
    public void Adicionar(T t)
    {
        _context.Set<T>().Add(t);
        _context.SaveChanges();
    }

    public void Atualizar(T t)
    {
        _context.Set<T>().Update(t);
        _context.SaveChanges();
    }
    
    public void Deletar(T t){
        _context.Set<T>().Remove(t);
        _context.SaveChanges();
    }
    
    public T? RecuperarPor(Func<T, bool> condicao)
    {
        return _context.Set<T>().FirstOrDefault(condicao);
    }
    
    public IEnumerable<T> ListarPor(Func<T, bool> condicao)
    {
        return _context.Set<T>().Where(condicao);
    }
}