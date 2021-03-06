﻿using Protogame.Editor.Api.Version1.Menu;
using Protogame.Editor.Api.Version1.ProjectManagement;
using Protogame.Editor.ProjectManagement;
using System.Collections.Generic;

namespace Protogame.Editor.Menu
{
    public class ProjectManagerMenuProvider : IMenuProvider
    {
        private readonly IProjectManager _projectManager;
        private readonly IProjectManagerUi _projectManagerUi;
        private readonly ICoroutine _coroutine;

        public ProjectManagerMenuProvider(
            IProjectManager projectManager,
            IProjectManagerUi projectManagerUi,
            ICoroutine coroutine)
        {
            _projectManager = projectManager;
            _projectManagerUi = projectManagerUi;
            _coroutine = coroutine;
        }

        public IEnumerable<MenuEntry> GetMenuItems()
        {
            yield return new MenuEntry("File/New Project...", true, 0, OnNewProject, null);
            yield return new MenuEntry("File/Open Project...", true, 1, OnOpenProject, null);
            yield return new MenuEntry("File/Save Project", true, 2, OnSaveProject, null);
            yield return new MenuEntry("File/Exit", true, 400, OnExitApplication, null);
        }

        private void OnNewProject(MenuEntry obj)
        {
        }

        private void OnSaveProject(MenuEntry obj)
        {
        }

        private void OnOpenProject(MenuEntry obj)
        {
            _coroutine.Run(() => _projectManagerUi.LoadProject());
        }

        private void OnExitApplication(MenuEntry obj)
        {
            //gameContext.Game.Exit();
        }
    }
}
