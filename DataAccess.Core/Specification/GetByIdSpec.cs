﻿using DataAccess.Core.Specification.Join;

namespace DataAccess.Core.Specification
{
    public class GetByIdSpec<TEntity>
        where TEntity : IDbEntity
    {
        /// <summary>
        /// Id искомой сущности
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// убирать ли полученную сущность из контекста после получения
        /// </summary>
        public bool AsNoTracking { get; set; } = false;

        /// <summary>
        /// объект, содержащий информацию о включении в результаты запроса доп. таблиц
        /// </summary>
        public IQueryJoin<TEntity> Join;
    }
}
