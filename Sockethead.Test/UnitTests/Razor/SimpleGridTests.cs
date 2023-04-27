using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Moq;
using Sockethead.Razor.Grid;
using Sockethead.Web.Areas.Samples.Utilities;
using Sockethead.Web.Areas.Samples.ViewModels;
using Xunit;

namespace Sockethead.Test.UnitTests.Razor
{
    public class SimpleGridTests
    {
        private static Mock<IHtmlHelper> MockIHtmlHelper()
        {
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.Setup(x => x.Query)
                .Returns(new QueryCollection(new Dictionary<string, StringValues>()));
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);
            var mockViewContext = new ViewContext { HttpContext = mockHttpContext.Object };
            var mockHtmlHelper = new Mock<IHtmlHelper>();
            mockHtmlHelper.Setup(x => x.ViewContext).Returns(mockViewContext);

            return mockHtmlHelper;
        }

        private static SimpleGrid<Feature> GetSimpleGridInstanceWithSource() =>
            new(MockIHtmlHelper().Object, SimpleGridFeatures.Features.AsQueryable());

        [Fact]
        public void AddColumn_AddsColumnToColumnsList()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumn(col => col
                    .For(feature => feature.Name)
                    .Css(css => css.Item.AddStyle("width: 150px;")));
            
            SimpleGridViewModel vm = grid.PrepareRender();
            
            Assert.Single(vm.Columns);
            Assert.Equal(vm.Rows.Count, SimpleGridFeatures.Features.Count);
        }
        
        [Fact]
        public void AddColumnFor_AddsColumnToColumnsList()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumnFor(feature => feature.Name)
                .AddColumnFor(feature => feature.Description);
            
            SimpleGridViewModel vm = grid.PrepareRender();

            vm.Columns.Length.Should().Be(2);
            Assert.Equal(vm.Rows.Count, SimpleGridFeatures.Features.Count);
        }
        
        [Fact]
        public void AddColumnForTwoColumnGrid_AddsColumnToColumnsList()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumnFor(feature => feature.Name)
                .AddColumnForTwoColumnGrid(column => column.Header("TwoColumnGrid"),
                    (gridBuilder, model) =>
                        gridBuilder
                            .AddRow("Name 1", model.Name)
                            .AddRowsForModel(model));
            
            SimpleGridViewModel vm = grid.PrepareRender();

            vm.Columns.Length.Should().Be(2);
            Assert.Equal(vm.Rows.Count, SimpleGridFeatures.Features.Count);
        }
        
        [Fact]
        public void AddColumnsForModel_AddsColumnsToColumnsList()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumnsForModel();
            
            SimpleGridViewModel vm = grid.PrepareRender();

            vm.Columns.Length.Should().Be(typeof(Feature).GetProperties().Length);
            Assert.Equal(vm.Rows.Count, SimpleGridFeatures.Features.Count);
        }
        
        [Fact]
        public void RemoveColumn_RemoveColumnToColumnsList()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumnsForModel()
                .RemoveColumn("Name")
                .RemoveColumn(0);
            
            SimpleGridViewModel vm = grid.PrepareRender();

            vm.Columns.Length.Should().Be(typeof(Feature).GetProperties().Length - 2);
        }
        
        [Fact]
        public void AddSearch_AddSearchFilterNameToGrid()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumnFor(feature => feature.Name)
                .AddColumnFor(feature => feature.Description)
                .AddSearch("", (source,query) 
                    => source.Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase)));
            
            SimpleGridViewModel vm = grid.PrepareRender();

            vm.Columns.Length.Should().Be(2);
            vm.SimpleGridSearchViewModel.SearchFilterNames.Count.Should().Be(1);
        }
        
        [Fact]
        public void AddCss_ShouldAddCssToGrid()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .AddColumn(col => col
                    .For(feature => feature.Name)
                    .Css(elements =>
                    {
                        elements.Header
                                .AddStyle("background-color: red");
                        elements.Item
                                .AddStyle("font-style: italic");
                    }))
                .AddColumnFor(feature => feature.Description)
                .AddCssClass("table-striped")
                .Css(elements =>
                {
                    elements.Table
                        .AddClass("table-bordered");

                    elements.Header
                        .AddStyle("color: blue");
                    
                    elements.Row
                        .AddStyle("color: green");
                });
            
            SimpleGridViewModel vm = grid.PrepareRender();

            // Assert Column Css
            Assert.Contains("background-color: red", vm.Columns[0].HeaderCss);
            Assert.Contains("font-style: italic", vm.Columns[0].ItemCss);
            
            // Assert Grid Css
            Assert.Contains("table-striped", vm.Css.TableCss);
            Assert.Contains("table-bordered", vm.Css.TableCss);
            Assert.Contains("color: blue", vm.Css.HeaderCss);
            Assert.Contains("color: green", vm.Css.RowCss);
        }
        
        [Fact]
        public void AddPager_AddPagerToGrid()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            const int rowsPerPage = 3;
            grid
                .AddColumnFor(movie => movie.Name)
                .AddPager(options => options.RowsPerPage = rowsPerPage);
            
            SimpleGridViewModel vm = grid.PrepareRender();

            Assert.Equal(rowsPerPage, vm.Rows.Count);
        }
        
        [Fact]
        public void AddFooter_AddFooterHtmlToGrid()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            const string footerHtml = "<div>Some HTML</div>";
            grid
                .AddColumnFor(movie => movie.Name)
                .Footer(footerHtml);
            
            SimpleGridViewModel vm = grid.PrepareRender();

            Assert.Equal(footerHtml, vm.FooterHtml);
        }
        
        [Fact]
        public void If_ShouldPerformGridOperationIfConditionIsMet()
        {
            SimpleGrid<Feature> grid = GetSimpleGridInstanceWithSource();

            grid
                .If(true, action => action.AddColumnFor(col => col.Name))
                .If(false, action => action.AddColumnFor(col => col.Description));
            
            SimpleGridViewModel vm = grid.PrepareRender();

            Assert.Single(vm.Columns);
            Assert.Equal(nameof(Feature.Name), vm.Columns[0].HeaderDetails.Display);
        }
    }
}