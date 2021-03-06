// SmartAPI - .Net programmatic access to RedDot servers
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
using System.Xml;
using erminas.SmartAPI.Utils;
using erminas.SmartAPI.Utils.CachedCollections;

namespace erminas.SmartAPI.CMS.Project.Folder
{
    internal class ContentClassFolderSharing : NameIndexedRDList<IProject>, IContentClassFolderSharing
    {
        private const string SHARING =
            @"<SHAREDFOLDER shared=""{0}"" action=""save"" guid=""{1}"" ><PROJECTS>{2}</PROJECTS></SHAREDFOLDER>";

        private const string SINGLE_PROJECT = @"<PROJECT guid=""{0}"" sharedrights=""{1}"" />";
        private readonly ContentClassFolder _contentClassFolder;

        internal ContentClassFolderSharing(ContentClassFolder contentClassFolder, Caching caching) : base(caching)
        {
            _contentClassFolder = contentClassFolder;
            RetrieveFunc = GetSharedToProjects;
        }

        public void Add(IProject project)
        {
            AddRange(new[] {project});
        }

        public void AddRange(IEnumerable<IProject> projects)
        {
            const bool IS_SHARED = true;
            var projectsRql = projects.Aggregate("", (s, project) => s + SINGLE_PROJECT.RQLFormat(project, IS_SHARED));

            var query = SHARING.RQLFormat(IS_SHARED, _contentClassFolder, projectsRql);
            Project.ExecuteRQL(query, RqlType.SessionKeyInProject);

            InvalidateCache();
        }

        public IContentClassFolder ContentClassFolder
        {
            get { return _contentClassFolder; }
        }

        public IProject Project
        {
            get { return _contentClassFolder.Project; }
        }

        public void Remove(IProject project)
        {
            var isStillSharing = Count > 1 || (Count == 1 && !this.First().Equals(project));
            Project.ExecuteRQL(
                SHARING.RQLFormat(isStillSharing, _contentClassFolder, SINGLE_PROJECT.RQLFormat(project, 0)),
                RqlType.SessionKeyInProject);

            InvalidateCache();
        }

        public ISession Session
        {
            get { return _contentClassFolder.Session; }
        }

        private List<IProject> GetSharedToProjects()
        {
            const string LOAD_SHARED = @"<SHAREDFOLDER action=""load"" guid=""{0}""/>";

            var xmlDoc = Project.ExecuteRQL(LOAD_SHARED.RQLFormat(_contentClassFolder), RqlType.SessionKeyInProject);
            var projectNodes = xmlDoc.SelectNodes("//PROJECT[@sharedrights=1]");

            //server manager can access all projects, others might only be able to see the name/guid
            if (Session.CurrentUser.ModuleAssignment.IsServerManager)
            {
                return
                    (from XmlElement curProject in projectNodes
                     select Session.ServerManager.Projects.GetByGuid(curProject.GetGuid())).ToList();
            }

            return (from XmlElement curProject in projectNodes
                    select (IProject) new Project(Session, curProject.GetGuid()) {Name = curProject.GetName()}).ToList();
        }
    }

    public interface IContentClassFolderSharing : IIndexedRDList<string, IProject>, IProjectObject
    {
        void Add(IProject project);
        void AddRange(IEnumerable<IProject> project);
        IContentClassFolder ContentClassFolder { get; }
        void Remove(IProject project);
    }
}