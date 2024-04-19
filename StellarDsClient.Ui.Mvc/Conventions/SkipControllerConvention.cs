using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace StellarDsClient.Ui.Mvc.Conventions
{
    public class SkipControllerConvention(Type controllerType) : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType == controllerType)
            {
                controller.ApiExplorer.IsVisible = false;
            }
        }
    }
}