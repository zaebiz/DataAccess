using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.Repository.Repository;
using DataAccess.Repository.Specification;
using DataAccess.Repository.Specification.Filter;

//using AutoMapper;

namespace DataAccess.Repository.DataService
{
    /// <summary>
    /// Типизированная обертка над репозиторием, обеспечивающая
    /// - набор удобных методов получения данных
    /// - доступ к самому репозиторию
    /// - доступ к контексту EF
    /// </summary>
    public class DataServiceBase<TEntity> : IDataService<TEntity>
        where TEntity: class, IDbEntity
    {
        protected readonly IRepository _repository;        

        public IRepository Repository => _repository;

        public DataServiceBase(IRepository repository)
        {
            _repository = repository;            
        }

        #region GetById Methods        
        
        public virtual async Task<TEntity> GetItem(GetByIdSpec<TEntity> spec)
        {
            if (spec.Id <= 0) throw new ArgumentOutOfRangeException(nameof(spec.Id));

            return await
                _repository
                .GetItemById(spec)
                .ConfigureAwait(false);
        }
        
        public virtual async Task<TEntity> GetItem(int itemId)
        {
            var spec = new GetByIdSpec<TEntity>()
            {
                Id = itemId
            };

            return await GetItem(spec)
            .ConfigureAwait(false);
        }
        
        public async Task<TDto> GetMappedItem<TDto>(GetByIdSpec<TEntity> spec, IMapper mapper)
        {
            var item = await GetItem(spec).ConfigureAwait(false);
            return mapper.Map<TDto>(item);
        }

        public async Task<TDto> GetMappedItem<TDto>(int itemId, IMapper mapper)
        {
            return await GetMappedItem<TDto>(new GetByIdSpec<TEntity>() { Id = itemId }, mapper)
                .ConfigureAwait(false);
        }
        
        public virtual async Task<TEntity> GetItemOrThrow(GetByIdSpec<TEntity> spec)
        {
            var item = await GetItem(spec).ConfigureAwait(false);

            if (item == null)
                throw new Exception($"Сущность не найдена");

            return item;
        }
        
        public virtual async Task<TEntity> GetItemOrThrow(int itemId)
        {
            return await GetItemOrThrow(new GetByIdSpec<TEntity>()
            {
                Id = itemId
            })
            .ConfigureAwait(false);
        }

        
        public async Task<TDto> GetMappedItemOrThrow<TDto>(int itemId, IMapper mapper)
        {
            var item = await GetItemOrThrow(new GetByIdSpec<TEntity> { Id = itemId })
                .ConfigureAwait(false);

            return mapper.Map<TDto>(item);
        }

        public virtual async Task<TDto> GetMappedItemOrThrow<TDto>(GetByIdSpec<TEntity> spec, IMapper mapper)
        {
            var item = await GetItemOrThrow(spec)
                .ConfigureAwait(false);

            return mapper.Map<TDto>(item);
        }

        public virtual async Task<bool> IsItemExists(int entityId)
        {
            var count = await _repository
                .GetQueryable<TEntity>()
                .CountAsync(c => c.Id == entityId)
                .ConfigureAwait(false);

            return count > 0;
        }

        #endregion

        #region GetList Methods

        
        public virtual IQueryable<TEntity> GetQueryableList(QuerySpec<TEntity> spec)
        {
            return _repository.GetList(spec);
        }
        
        public virtual async Task<List<TEntity>> GetItemsList(QuerySpec<TEntity> spec)
        {
            return await _repository
                .GetList(spec)
                .ToListAsync();
            //.ConfigureAwait(false);
        }

        public async Task<List<TDto>> GetItemsMappedList<TDto>(QuerySpec<TEntity> spec, IMapper mapper)
        {
            return
                (await GetItemsList(spec))
                //.ConfigureAwait(false))
                .Select(x => mapper.Map<TDto>(x))
                .ToList();
        }
        
        public virtual async Task<int> GetItemsCount(IQueryFilter<TEntity> filter)
        {
            return await _repository
                .GetFilteredQueryable(filter)
                .CountAsync()
                .ConfigureAwait(false);
        }

        #endregion

        #region Add methods        
        
        public void AddItem(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _repository.GetDbSet<TEntity>().Add(item);
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                AddItem(item);
            }
        }
        
        public void AddItem<TDto>(TDto dto, IMapper mapper)
        {
            var item = mapper.Map<TEntity>(dto);
            AddItem(item);
        }
        
        public async Task<TEntity> AddAndSave(TEntity item)
        {
            AddItem(item);

            await _repository
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return item;
        }
        
        public async Task<TEntity> AddAndSave<TDto>(TDto dto, IMapper mapper)
        {
            var item = mapper.Map<TEntity>(dto);
            return await AddAndSave(item).ConfigureAwait(false);
        }               

        #endregion

        #region Upsert methods        
        
        public void UpsertItem(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _repository.AddOrUpdate(item);
        }

        public void UpsertRange(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                UpsertItem(item);
            }
        }

        public void UpsertItem<TDto>(TDto dto, IMapper mapper)
        {
            var item = mapper.Map<TEntity>(dto);
            UpsertItem(item);
        }
        
        public virtual async Task<TEntity> UpsertAndSave(TEntity item)
        {
            UpsertItem(item);

            await _repository
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return item;
        }
        
        public async Task<TEntity> UpsertAndSave<TDto>(TDto dto, IMapper mapper)
        {
            var item = mapper.Map<TEntity>(dto);
            return await UpsertAndSave(item);
        }               

        #endregion

        #region Remove methods

        /// <summary>
        /// можно ли разрешать пользователю удалять сущность, или она используется в других таблицах в качестве ключа
        /// </summary>
        protected virtual async Task<bool> IsItemRemoveForbidden(int itemId)
        {
            return await Task.FromResult(false);
        }
        
        public void RemoveRange(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            _repository.RemoveRange(items);
        }

        public async Task RemoveItem(int itemId)
        {
            if (itemId <= default(int)) throw new ArgumentOutOfRangeException(nameof(itemId));

            if (await IsItemRemoveForbidden(itemId))
                throw new Exception("Удаление невозможно");

            _repository.Remove<TEntity>(itemId);
        }

        public void RemoveItem(TEntity item)
        {
            _repository.Remove(item);
        }

        public virtual async Task RemoveItemAndSave(TEntity item)
        {
            RemoveItem(item);
            await _repository.SaveChangesAsync();
        }

        public virtual async Task RemoveItemAndSave(int itemId)
        {
            await RemoveItem(itemId);
            await _repository.SaveChangesAsync();
        }

        #endregion
    }
}
