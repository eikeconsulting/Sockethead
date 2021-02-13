﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Sockethead.Razor.Grid
{
    public static class Extensions
    {
        public static SimpleGrid<T> SimpleGrid<T>(
            this IHtmlHelper html, 
            IQueryable<T> source) where T : class
                => new SimpleGrid<T>(html, source);
    }
}