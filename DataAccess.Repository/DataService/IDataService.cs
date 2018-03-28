using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Filter;

namespace DataAccess.Repository.DataService
{
    public interface IDataService<TEntity> where TEntity : class, IDbEntity
    {
        IRepository Repository { get; }

        #region GetById methods

        /// <summary>
        /// Проверить - существует ли в таблице запись с переданным Id?
        /// </summary>
        Task<bool> IsItemExists(int entityId);

        /// <summary>
        /// получить сущность по Id
        /// </summary>
        /// <param name="itemId">Id объекта</param>
        Task<TEntity> GetItem(int itemId);

        /// <summary>
        /// получить сущность по Id
        /// </summary>
        /// <param name="spec">спецификация запроса (Id объекта, джоины)</param>
        Task<TEntity> GetItem(GetByIdSpec<TEntity> spec);

        /// <summary>
        /// получить сущность по Id  и сконвертировать ее в TDto
        /// </summary>
        /// <param name="spec">спецификация запроса (Id объекта, джоины)</param>
        /// <param name="mapper">Automapper engine</param>
        Task<TDto> GetMappedItem<TDto>(GetByIdSpec<TEntity> spec, IMapper mapper);

        /// <summary>
        /// получить сущность по Id  и сконвертировать ее в TDto
        /// </summary>
        /// <param name="mapper">Automapper engine</param>
        Task<TDto> GetMappedItem<TDto>(int itemId, IMapper mapper);

        /// <summary>
        /// получить сущность по Id и кинуть исключение, если не найдена
        /// </summary>
        /// <param name="itemId">Id объекта</param>
        Task<TEntity> GetItemOrThrow(int itemId);

        /// <summary>
        /// получить сущность по Id и кинуть исключение, если не найдена
        /// </summary>
        Task<TEntity> GetItemOrThrow(GetByIdSpec<TEntity> spec);

        /// <summary>
        /// получить сущность по Id и сконвертировать ее в TDto.
        /// Кинуть исключение, если не найдена
        /// </summary>
        Task<TDto> GetMappedItemOrThrow<TDto>(int itemId, IMapper mapper);

        /// <summary>
        /// получить сущность по Id и сконвертировать ее в TDto.
        /// Кинуть исключение, если не найдена
        /// </summary>
        Task<TDto> GetMappedItemOrThrow<TDto>(GetByIdSpec<TEntity> spec, IMapper mapper);

        #endregion

        #region GetList methods

        /// <summary>
        /// Получить объект запроса списка сущностей
        /// </summary>
        /// <param name="spec">спецификация запроса (фильтрация, пагинация, джоины)</param>
        IQueryable<TEntity> GetQueryableList(QuerySpec<TEntity> spec);

        /// <summary>
        /// получить список сущностей
        /// </summary>
        /// <param name="spec">спецификация запроса (фильтрация, пагинация, джоины)</param>
        Task<List<TEntity>> GetItemsList(QuerySpec<TEntity> spec);

        /// <summary>
        /// получить список сущностей EF, сконвертированный в TDto.  Маппинг для Automapper должен быть создан заранее
        /// </summary>
        /// <param name="spec">спецификация запроса (фильтрация, пагинация, джоины)</param>
        /// <param name="mapper">Automapper engine</param>
        Task<List<TDest>> GetItemsMappedList<TDest>(QuerySpec<TEntity> spec, IMapper mapper);

        /// <summary>
        /// получить кол-во сущностей по запросу
        /// </summary>
        /// <param name="filter">фильтр, применяемый к сущностям</param>
        Task<int> GetItemsCount(IQueryFilter<TEntity> filter);

        #endregion

        #region Add methods

        /// <summary>
        /// добавить объект EF в контекст. Изменения в БД не сохраняем
        /// </summary>
        void AddItem(TEntity item);

        /// <summary>
        /// добавить коллекцию объектов EF в контекст. Изменения в БД не сохраняем
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<TEntity> items);

        /// <summary>
        /// Смапить объект Dto в объект EF и добавить в контекст.  Изменения в БД не сохраняем
        /// </summary>
        void AddItem<TDto>(TDto dto, IMapper mapper);

        /// <summary>
        /// добавить объект EF в контекст и сохранить изменения контекста в БД .
        /// Вернуть сохраненный объект
        /// </summary>
        Task<TEntity> AddAndSave(TEntity item);

        /// <summary>
        /// сконвертироват Dto в доменный объект
        /// добавить объект EF в контекст и сохранить изменения контекста в БД.
        /// Вернуть сохраненный объект 
        /// </summary>
        Task<TEntity> AddAndSave<TDto>(TDto dto, IMapper mapper);

        #endregion

        #region Upsert methods

        /// <summary>
        /// добавить объект EF в контекст. Изменения в БД не сохраняем
        /// </summary>
        void UpsertItem(TEntity item);

        /// <summary>
        /// добавить коллекцию объектов EF в контекст. Изменения в БД не сохраняем
        /// </summary>
        void UpsertRange(IEnumerable<TEntity> items);

        /// <summary>
        /// Смапить объект Dto в объект EF и добавить в контекст. Изменения в БД не сохраняем
        /// </summary>
        void UpsertItem<TDto>(TDto dto, IMapper mapper);

        /// <summary>
        /// Добавить в контекст объект EF и сохранить в БД
        /// </summary>
        Task<TEntity> UpsertAndSave(TEntity item);

        /// <summary>
        /// Сконвертировать Dto в доменный объект
        /// Добавить в контекст объект EF и сохранить в БД. 
        /// </summary>
        Task<TEntity> UpsertAndSave<TDto>(TDto dto, IMapper mapper);

        #endregion

        #region Remove methods

        /// <summary>
        /// удалить сущность
        /// </summary>
        void RemoveItem(TEntity item);

        /// <summary>
        /// удалить коллекцию сущность
        /// </summary>
        void RemoveRange(IEnumerable<TEntity> items);

        /// <summary>
        /// удалить сущность
        /// </summary>
        Task RemoveItem(int itemId);

        /// <summary>
        /// удалить сущность и сохранить изменения в БД
        /// </summary>
        Task RemoveItemAndSave(TEntity item);

        /// <summary>
        /// удалить сущность и сохранить изменения в БД
        /// </summary>
        Task RemoveItemAndSave(int itemId);

        #endregion


    }
}