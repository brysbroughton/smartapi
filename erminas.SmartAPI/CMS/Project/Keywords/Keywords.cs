﻿// Smart API - .Net programmatic access to RedDot servers
//  
// Copyright (C) 2013 erminas GbR
// 
// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program.
// If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using erminas.SmartAPI.CMS.Administration;
using erminas.SmartAPI.Exceptions;
using erminas.SmartAPI.Utils;
using erminas.SmartAPI.Utils.CachedCollections;

namespace erminas.SmartAPI.CMS.Project.Keywords
{
    /// <summary>
    ///     Encapsulates keyword management for a category.
    /// </summary>
    /// <remarks>
    ///     We don't subclass NameIndexedRDList, because renaming to existing names is allowed and could lead to duplicate keyword names.
    /// </remarks>
    public class Keywords : RDList<Keyword>, IProjectObject
    {
        public readonly Category Category;

        internal Keywords(Category category) : base(Caching.Enabled)
        {
            Project = category.Project;
            Category = category;
            RetrieveFunc = GetKeywords;
        }

        public Keyword CreateOrGet(string keywordName)
        {
            const string SAVE_KEYWORD = @"<CATEGORY guid=""{0}""><KEYWORD action=""save"" value=""{1}""/></CATEGORY>";

            var xmlDoc =
                Category.Project.ExecuteRQL(SAVE_KEYWORD.RQLFormat(Category, HttpUtility.HtmlEncode(keywordName)),
                                            Project.RqlType.SessionKeyInProject);
            var keyword = (XmlElement) xmlDoc.SelectSingleNode("/IODATA/KEYWORD");
            if (keyword == null)
            {
                throw new SmartAPIException(Session.ServerLogin,
                                            string.Format("Could not create the keyword '{0}'", keywordName));
            }

            InvalidateCache();
            return new Keyword(Category.Project, keyword);
        }

        public void Delete(string keywordName)
        {
            Keyword keyword;
            if (!TryGetByName(keywordName, out keyword))
            {
                return;
            }

            keyword.Delete();
            InvalidateCache();
        }

        public void DeleteForcibly(string keywordName)
        {
            Keyword keyword;
            if (!TryGetByName(keywordName, out keyword))
            {
                return;
            }

            keyword.DeleteForcibly();
            InvalidateCache();
        }

        private List<Keyword> GetKeywords()
        {
            const string LIST_KEYWORDS =
                @"<PROJECT><CATEGORY guid=""{0}""><KEYWORDS action=""load"" /></CATEGORY></PROJECT>";
            XmlDocument xmlDoc = Category.Project.ExecuteRQL(LIST_KEYWORDS.RQLFormat(Category));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("KEYWORD");

            var kategoryKeyword = new List<Keyword>
                {
                    new Keyword(Category.Project, Category.Guid) {Name = "[category]", Category = Category}
                };
            return
                (from XmlElement curNode in xmlNodes select new Keyword(Category.Project, curNode) {Category = Category})
                    .Union(kategoryKeyword).ToList();
        }

        public Session Session { get { return Project.Session; } }
        public Project Project { get; private set; }
    }
}