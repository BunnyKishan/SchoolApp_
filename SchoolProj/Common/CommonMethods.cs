using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SchoolProj.Common
{
	public class FilterInfor
	{
		[JsonProperty("sortName")]
		public string SortName { get; set; }

		[JsonProperty("sortOrder")]
		public string SortOrder { get; set; }

		[JsonProperty("pageNumber")]
		public int PageNumber { get; set; }

		[JsonProperty("pageSize")]
		public int PageSize { get; set; }

		[JsonProperty("searchText")]
		public string SearchText { get; set; }

		[JsonProperty("searchItems")]
		public List<SearchItem> SearchItems { get; set; }
	}

	public class SearchItem
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("value")]
		public object Value { get; set; }

		public Operator Operator { get; set; }
	}

	public enum Operator
	{
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual,
		Between,
		Like,
		In
	}

	public static class CommonMethods
	{
		public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, FilterInfor filter)
		{
			if (source == null && source.Count() == 0)
			{
				return source;
			}
			FilterInfor filterValid = ValidateFilter(source, filter);
			return (IQueryable<T>)ApplyFilter((IQueryable)source, filterValid);
		}

		public static FilterInfor ValidateFilter<T>(IQueryable<T> source, FilterInfor filter)
		{

			var TProperties = typeof(T).GetProperties();
			// where
			List<SearchItem> checkFilters = new List<SearchItem>();
			filter.SearchItems = new List<SearchItem>();
			if (!string.IsNullOrEmpty(filter.SearchText))
			{
				checkFilters = JsonConvert.DeserializeObject<List<SearchItem>>(filter.SearchText);
			}
			// filter.SearchItems = checkFilters;
			foreach (PropertyInfo propertyInfo in TProperties)
			{
				// validate search items
				foreach (var filterItem in checkFilters)
				{
					if (filterItem.Key == propertyInfo.Name)
					{
						if (propertyInfo.PropertyType != null && filterItem.Value != null)
						{
							IConvertible convertible = filterItem.Value as IConvertible;
							var t = propertyInfo.PropertyType;
							if (convertible != null)
							{
								if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
								{
									filterItem.Value = Convert.ChangeType(filterItem.Value, t.GetGenericArguments()[0]);
								}
								else
								{
									filterItem.Value = Convert.ChangeType(filterItem.Value, t);

                                    if (t.Name == nameof(DateTime))
                                    {
                                        filterItem.Value = ((DateTime)filterItem.Value);
                                    }
								}

								filter.SearchItems.Add(filterItem);
							}
						}
					}
				}
			}

			// order
			string defaultSortName = TProperties.FirstOrDefault().Name;
			foreach (PropertyInfo propertyInfo in TProperties)
			{
				if (filter.SortName == propertyInfo.Name)
				{
					defaultSortName = propertyInfo.Name;
					break;
				}
			}
			filter.SortName = defaultSortName;
			if (filter.SortOrder != "asc" && filter.SortOrder != "desc")
			{
				filter.SortOrder = "asc";
			}

			// paging
			filter.PageNumber = filter.PageNumber == 0 ? 1 : filter.PageNumber;
			filter.PageSize = filter.PageSize == 0 ? 10 : filter.PageSize;

			return filter;
		}

		public static IQueryable ApplyFilter(this IQueryable query, FilterInfor filter)
		{
			object[] values = new object[filter.SearchItems.Count];
			for (int i = 0; i < filter.SearchItems.Count; i++)
			{
				values[i] = filter.SearchItems[i].Value;
			}
			LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(query.ElementType, typeof(bool), GenerateWhere(filter), values);

			// apply where
			Expression expr = Expression.Call(
					typeof(Queryable), "Where",
					new Type[] { query.ElementType },
					query.Expression, Expression.Quote(lambda));

			var filtered = query.Provider.CreateQuery(expr)
					.OrderBy(GenerateOrderBy(filter));

			return filtered;
		}

		public static List<TSource> ApplyPagingToList<TSource>(this IQueryable<TSource> query, FilterInfor filter)
		{
			if (query == null || query.Count() == 0)
			{
				return new List<TSource>();
			}
			filter.PageSize = filter.PageSize == 0 ? 10 : filter.PageSize;
			filter.PageNumber = filter.PageNumber == 0 ? 1 : filter.PageNumber;
			var q = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
			return q;
		}

		public static string SerializeToJson<TSource>(this IQueryable<TSource> query, FilterInfor filter)
		{
			if (query == null || query.Count() == 0)
			{
				return JsonConvert.SerializeObject(new { total = 0, rows = query }, Formatting.Indented
				, new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});
			}
			filter.PageSize = filter.PageSize == 0 ? 10 : filter.PageSize;
			filter.PageNumber = filter.PageNumber == 0 ? 1 : filter.PageNumber;
			
			var q = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
			return JsonConvert.SerializeObject(new { total = query.Count(), rows = q }, Formatting.Indented
				, new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});
		}

		public static string SerializeListToJson<TSource>(IEnumerable<TSource> query, int total)
		{
			if (query == null || total == 0)
			{
				return JsonConvert.SerializeObject(new { total = 0, rows = query }, Formatting.Indented
				, new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				});
			}
			
			return JsonConvert.SerializeObject(new { total = total, rows = query }, Formatting.Indented
				, new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore

				});
		}

		public static string ToDescription(this Enum value)
		{
			// Get the Description attribute value for the enum value
			FieldInfo fi = value.GetType().GetField(value.ToString());
            if(fi == null)
            {
                return string.Empty;
            }

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(
					typeof(DescriptionAttribute), false);

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			return value.ToString();
		}

		public static IEnumerable<SelectListItem> EnumToSelectList<T>()
		{
			return (Enum.GetValues(typeof(T)).Cast<T>().Select(
				e => new SelectListItem() { Text = ToDescription(e as Enum), Value = (Convert.ToInt32(e)).ToString() })).ToList();
		}
		
		public static string GetDisplayName(this Enum value)
		{
			var attribute = (DisplayAttribute)value.GetType()?
				.GetField(value.ToString())?
				.GetCustomAttributes(false)?
				.Where(a => a is DisplayAttribute)
				.FirstOrDefault();

			return attribute?.Name ?? string.Empty;
		}

		public static string GenerateOrderBy(FilterInfor filter)
		{
			string sortName = filter.SortName;
			string sortOrder = filter.SortOrder;

			return string.Format("{0} {1}", sortName, sortOrder);
		}

		public static string GenerateWhere(FilterInfor filter)
		{
			string whereStr = "1 == 1";
			foreach (SearchItem item in filter.SearchItems)
			{
				string strOperator = string.Empty;

                switch (item.Operator)
				{
					case Operator.Equal:
						strOperator = "==";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.NotEqual:
						strOperator = "!=";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.GreaterThan:
						strOperator = ">";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.LessThan:
						strOperator = "<";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.GreaterThanOrEqual:
						strOperator = ">=";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.LessThanOrEqual:
						strOperator = "<=";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.Between:
						strOperator = "BETWEEN";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					case Operator.Like:
						strOperator = "LIKE";
						whereStr += " AND " + item.Key + ".Contains(@"+ filter.SearchItems.IndexOf(item) + ")";
						break;
					case Operator.In:
						strOperator = "In";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
					default:
						strOperator = "==";
						whereStr += " AND " + item.Key + " " + strOperator + " @" + filter.SearchItems.IndexOf(item);
						break;
				}

			}
			return whereStr;
		}

	}
}