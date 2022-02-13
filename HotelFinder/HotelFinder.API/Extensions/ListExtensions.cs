using HotelFinder.API.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HotelFinder.API.Extensions
{
    public static class ListExtensions
    {
        public static List<T> ToListFromQueryParams<T>(this IQueryable<T> entity, GetAllParams prm) where T : class
        {

            var props = entity
                            .GetType()
                            .GetProperties();

            var list = entity
                            .Where(x => props.Where(p => p.GetValue(x) == null ? false : p.GetValue(x).ToString().Contains(prm.SearchVal)).Count() > 0);

            var sortProp = props
                            .Where(x => x.Name == prm.SortField)
                            .FirstOrDefault();

            if (sortProp != null)
            {
                list = prm.SortType == Enums.SortType.Asc ? list.OrderBy(x => sortProp) : list.OrderByDescending(x => sortProp);
            }

            var filterFields = props.Where(x => prm.FilterFields
                                    .Contains(x.Name)).ToList();

           

            var x = Expression
                        .Parameter(typeof(T), "x");

            var body = Expression
                        .PropertyOrField(x, filterFields[0].Name);

            var lambda = Expression
                        .Lambda<Func<T, T>>(body, x);


           list = list
                    .Select(lambda);


           return list
                    .ToList();

        }

        
    }
}
