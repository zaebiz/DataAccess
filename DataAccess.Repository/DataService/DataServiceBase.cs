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
        protected readonly IMapper _mapper;

        public IRepository Repository => _repository;

        public DataServiceBase(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
        
        public async Task<TDto> GetMappedItem<TDto>(GetByIdSpec<TEntity> spec)
        {
            var item = await GetItem(spec).ConfigureAwait(false);

            return _mapper.Map<TDto>(item);
        }

        public async Task<TDto> GetMappedItem<TDto>(int itemId)
        {
            return await GetMappedItem<TDto>(new GetByIdSpec<TEntity>()
            {
                Id = itemId
            })
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

        
        public async Task<TDto> GetMappedItemOrThrow<TDto>(int itemId)
        {
            var item = await GetItemOrThrow(new GetByIdSpec<TEntity>()
            {
                Id = itemId
            })
            .ConfigureAwait(false);

            return _mapper.Map<TDto>(item);
        }

        public virtual async Task<TDto> GetMappedItemOrThrow<TDto>(GetByIdSpec<TEntity> spec)
        {
            var item = await GetItemOrThrow(spec).ConfigureAwait(false);

            return _mapper.Map<TDto>(item);
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

        public async Task<List<TDto>> GetItemsMappedList<TDto>(QuerySpec<TEntity> spec)
        {
            return
                (await GetItemsList(spec))
                //.ConfigureAwait(false))
                .Select(x => _mapper.Map<TDto>(x))
                .ToList();
        }
        
        public virtual IQueryable<TEntity> GetQueryableOrderedList<TSortKey>(OrderedQuerySpec<TEntity, TSortKey> spec)
        {
            return _repository.GetOrderedList(spec);
        }
        
        public virtual async Task<List<TEntity>> GetItemsOrderedList<TSortKey>(OrderedQuerySpec<TEntity, TSortKey> spec)
        {
            return await _repository
                .GetOrderedList(spec)
                .ToListAsync();
                //.ConfigureAwait(false);
        }
        
        public async Task<List<TDto>> GetItemsOrderedMappedList<TSortKey, TDto>(OrderedQuerySpec<TEntity, TSortKey> spec)
        {
            return
                (await GetItemsOrderedList(spec))
                //.ConfigureAwait(false))
                .Select(x => _mapper.Map<TDto>(x))
                .ToList();
        }
        
        public virtual async Task<int> GetItemsCount(IQueryFilter<TEntity> filter)
        {
            return await _repository
                .GetFilteredQueryable(filter)
                .CountAsync();
            //.ConfigureAwait(false);
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
        
        public void AddItem<TDto>(TDto dto)
        {
            var item = _mapper.Map<TEntity>(dto);
            AddItem(item);
        }
        
        public virtual async Task<TEntity> AddAndSave(TEntity item)
        {
            AddItem(item);

            await _repository
                .SaveChangesAsync();
                //.ConfigureAwait(false);

            return item;
        }
        
        public async Task<TDto> AddAndSave<TDto>(TEntity item)
        {
            item = await AddAndSave(item);
            return _mapper.Map<TDto>(item);
        }
        
        public async Task<TDto> AddAndSave<TDto>(TDto dto)
        {
            var item = _mapper.Map<TEntity>(dto);
            return await AddAndSave<TDto>(item);
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

        public void UpsertItem<TDto>(TDto dto)
        {
            var item = _mapper.Map<TEntity>(dto);
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
        
        public async Task<TDto> UpsertAndSave<TDto>(TEntity item)
        {
            await UpsertAndSave(item);
            return _mapper.Map<TDto>(item);
        }
        
        public async Task<TDto> UpsertAndSave<TDto>(TDto dto)
        {
            var item = _mapper.Map<TEntity>(dto);
            return await UpsertAndSave<TDto>(item);
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
        
        public virtual async Task RemoveRange(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            _repository.RemoveRange(items);
        }

        public virtual async Task RemoveItem(int itemId)
        {
            if (itemId <= default(int)) throw new ArgumentOutOfRangeException(nameof(itemId));

            if (await IsItemRemoveForbidden(itemId))
                throw new Exception("Удаление невозможно");

            _repository.Remove<TEntity>(itemId);
        }

        public virtual async Task RemoveItem(TEntity item)
        {
            _repository.Remove(item);
        }

        public virtual async Task RemoveItemAndSave(TEntity item)
        {
            await RemoveItem(item);
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
