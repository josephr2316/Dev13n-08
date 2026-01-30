namespace Okane.Application;

public interface IRepository<T, in TUpdateReq> where T : IEntity
{
    void Add(T entity);
    T? ById(int id);
    IEnumerable<T> All();
    void Remove(int id);
    bool Exists(int id);
    T Update(int id, TUpdateReq request);
}
