using System;
using System.Collections.Generic;
using ArchCore.MVP;
using ArchCore.SceneControl;
using ModestTree;
using Zenject;

namespace ArchCore.HelpServices
{
	/// <summary>
	/// Used by ViewCameraDisablingService.
	/// </summary>
	public class DisableCameraViewAttribute : Attribute
	{
        
	}
    
	public class ViewCameraDisablingService
	{
		private readonly ISceneManager sceneManager;
		private List<IPresenter> disablers = new List<IPresenter>();
        
		public ViewCameraDisablingService(ISceneManager sceneManager,  [InjectOptional] BaseViewManager viewManager, [InjectOptional] BasePopupManager popupManager)
		{
			this.sceneManager = sceneManager;
			if(viewManager != null) viewManager.OnViewShown += ViewOpen;
			if(popupManager != null) popupManager.OnViewShown += ViewOpen;
		}

		private void ViewOpen(Presenter presenter)
		{
			if (presenter.GetType().HasAttribute<DisableCameraViewAttribute>())
			{
				disablers.Add(presenter);
				presenter.OnClose += ViewClose;
				UpdateCamera();
			}
		}

		private void ViewClose(IPresenter presenter)
		{
			disablers.Remove(presenter);
			UpdateCamera();
		}

		void UpdateCamera()
		{
			sceneManager.CurrentScene.Camera.SetActive(disablers.Count == 0);
		}
	}
}