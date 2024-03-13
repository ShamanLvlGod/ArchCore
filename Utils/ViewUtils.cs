using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ArchCore.MVP;
using ArchCore.MVP.Utils;
using Zenject;

namespace ArchCore.Utils
{
	public class ViewUtils
	{
		public static (List<(Type view, Type presenter)> views, List<(Type view, Type presenter)> popups) AutoViewInstall(DiContainer container, Assembly assembly)
		{
			MethodInfo bindWrapper = typeof(ViewUtils).GetMethod("BindWrapper", BindingFlags.Static | BindingFlags.NonPublic);

			var viewRegistrationList = new List<(Type, Type)>();
			var popupRegistrationList = new List<(Type, Type)>();

			var bindings = AutoRegisterViewAttribute.GetViews(new []{assembly});

			foreach (var binding in bindings)
			{
				Type type = binding.view;
				Type presenterType = type.BaseType.GetGenericArguments()[0];

				MethodInfo bindingMethod = bindWrapper.MakeGenericMethod(type, presenterType);
				bindingMethod.Invoke(null, new object[] {container, binding.path});


				if (!CheckForPopup())
					viewRegistrationList.Add((type, presenterType));
				else
					popupRegistrationList.Add((type, presenterType));

				bool CheckForPopup()
				{
					Type tType = presenterType;

					do
					{
						if (tType.IsGenericType && tType.GetGenericTypeDefinition() == typeof(PopupPresenter<,,>))
							return true;

						tType = tType.BaseType;
					} while (tType != null);

					return false;
				}
			}

			return (viewRegistrationList, popupRegistrationList);
		}

		static void  BindWrapper<TView, TPresenter>(DiContainer container, string path)
		{
			container.BindViewPresenter<TView, TPresenter>(path);
		}
	}
}