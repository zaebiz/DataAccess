﻿using DataAccess.Core.Specification.Filter;
using DataAccess.Core.Specification.Join;
using DataAccess.Core.Specification.Order;
using DataAccess.Core.Specification.Paging;

namespace DataAccess.Core.Specification
{
    /// <summary>
    /// Набор параметров, кастомизирующих запрос к БД
    /// </summary>
    public class QuerySpec<TEntity> where TEntity : class, IDbEntity
    {
        /// <summary>
        /// объект, содержащий параметры для фильтрации сущностей
        /// </summary>
        public IQueryFilter<TEntity> Filter { get; set; }

        /// <summary>
        /// объект, содержащий информацию о включении в результаты запроса доп. таблиц
        /// </summary>
        public IQueryJoin<TEntity> Join { get; set; }

        /// <summary>
        /// пагинация
        /// </summary>
        public IQueryPaging Paging { get; set; }

        /// <summary>
        /// лямбда для сортировки
        /// </summary>
        public IQueryOrder<TEntity> Order { get; set; }

        /// <summary>
        /// убирать ли полученную сущность из контекста после получения
        /// </summary>
        public bool AsNoTracking { get; set; } = true;
    }
}
