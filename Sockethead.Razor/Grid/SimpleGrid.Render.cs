using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Grid
{
    public partial class SimpleGrid<T>
    {
        private IQueryable<T> BuildQuery()
        {
            IQueryable<T> query = Source;

            // apply search to query
            if (!string.IsNullOrEmpty(State.SearchQuery) &&
                State.SearchNdx > 0 &&
                State.SearchNdx <= SimpleGridSearches.Count)
                query = SimpleGridSearches[State.SearchNdx - 1].SearchFilter(query, State.SearchQuery);

            // apply sort(s) to query
            Column<T> sortColumn = State.SortColumn > 0 && State.SortColumn <= Columns.Count
                ? Columns[State.SortColumn - 1]
                : null;
            List<Sort<T>> sorts = new();
            if (sortColumn != null && IsSortable && sortColumn.Sort.IsActive)
            {
                sortColumn.Sort.SortOrder = State.SortOrder; // kludge
                sorts.Add(sortColumn.Sort);
            }

            sorts.Add(Sort);
            return Sort<T>.ApplySorts(sorts, query);
        }

        /// <summary>
        /// Render the Grid!
        /// </summary>
        public SimpleGridViewModel PrepareRender()
        {
            IQueryable<T> query = BuildQuery() ?? new List<T>().AsQueryable();

            int totalRecords = query.Count();
            PagerOptions.Enabled = PagerOptions.Enabled &&
                                   (PagerOptions.RowsPerPage < totalRecords || !PagerOptions.HideIfTooFewRows);

            SimpleGridViewModel vm = new()
            {
                Css = new GridCssViewModel
                {
                    TableCss = CssOptions.Table.ToString(),
                    HeaderCss = CssOptions.Header.ToString(),
                    RowCss = CssOptions.Row.ToString(),
                },

                FooterHtml = FooterHtml,

                GetRowCss = row => RowModifiers.FirstOrDefault(rc => rc.RowFilter(row as T))?.CssBuilder.ToString(),

                Options = SimpleGridOptions,

                // build pager view model
                PagerOptions = PagerOptions,
                PagerModel = PagerOptions.Enabled
                    ? State.BuildPagerModel(
                        totalRecords: totalRecords,
                        displayTotal: PagerOptions.DisplayTotal,
                        rowsPerPage: State.RowsPerPage ?? PagerOptions.RowsPerPage,
                        rowsPerPageOptions: PagerOptions.RowsPerPageOptions)
                    : null,

                // build search view model
                SimpleGridSearchViewModel = SimpleGridSearches.Any()
                    ? new SimpleGridSearchViewModel
                    {
                        GridId = GridId,
                        RedirectUrl = State.BuildResetUrl(),
                        SearchFilterNames = SimpleGridSearches.Select((search, i) => new SelectListItem
                        {
                            Text = search.Name,
                            Value = (i + 1).ToString(),
                            Selected = State.SearchNdx == i + 1,
                        }).ToList(),
                        Query = State.SearchQuery,
                        SearchNdx = State.SearchNdx.ToString(),
                    }
                    : null,

                IsHeaderEnabled = IsHeaderEnabled,
            };

            // build column headers
            int ndx = 0;
            foreach (Column<T> col in Columns)
            {
                ndx++;

                col.HeaderDetails.Display = col.HeaderRender();
                if (!IsSortable || !col.Sort.IsActive)
                    continue;

                SortOrder sortOrder = col.Sort.SortOrder;

                if (ndx == State.SortColumn)
                {
                    col.HeaderDetails.CurrentSortOrder = State.SortOrder;
                    sortOrder = Sort<T>.Flip(State.SortOrder);
                }

                col.HeaderDetails.SortUrl = State.BuildSortUrl(ndx, sortOrder);
            }

            // resolve the data (rows)
            // Note: we can't call ToListAsync here without a reference to Microsoft.EntityFrameworkCore
            int rowsToTake = SimpleGridOptions.MaxRows;

            if (State.RowsPerPage.HasValue)
                rowsToTake = State.RowsPerPage.Value;
            else if (PagerOptions.Enabled)
                rowsToTake = PagerOptions.RowsPerPage;

            if (rowsToTake > SimpleGridOptions.MaxRows)
                rowsToTake = SimpleGridOptions.MaxRows;

            vm.Rows = query
                .Skip((State.PageNum - 1) * rowsToTake)
                .Take(rowsToTake)
                .Cast<object>()
                .ToList();

            // build array of generic columns (not tied to the type of the model)
            vm.Columns = Columns
                .Cast<ISimpleGridColumn>()
                .ToArray();

            return vm;
        }
    }
}