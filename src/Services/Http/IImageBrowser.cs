using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.Services.Http
{
    public interface IImageBrowser
    {
        Task<IList<Uri>> Find(string query, Language queryLanguage);
    }
}