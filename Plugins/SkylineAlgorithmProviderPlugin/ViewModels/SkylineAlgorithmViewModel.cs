using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.Core.Packing.Placement.Skyline;
using Prism.Ioc;
using Unity;

namespace PaunPacker.Plugins.ViewModels
{
    /// <summary>
    /// View model wired to the <see cref="SkylineAlgorithmProviderPlugin"/>'s view 
    /// </summary>
    class SkylineAlgorithmViewModel
    {
        /// <summary>
        /// Constructs the view model and sets the loaded types (that are later shown in view) to <paramref name="loadedISkylineRectAndPointPickerTypes"/>
        /// </summary>
        /// <param name="loadedISkylineRectAndPointPickerTypes">The types of loaded skyline rect and point pickers</param>
        /// <param name="unityContainer">The unity container</param>
        public SkylineAlgorithmViewModel(IEnumerable<Type> loadedISkylineRectAndPointPickerTypes, IUnityContainer unityContainer)
        {
            SkylineRectAndPointPickerVMs = loadedISkylineRectAndPointPickerTypes.Select(x => new RectAndPointPickerViewModel(x));
            unityContainer.RegisterFactory<ISkylineRectAndPointPicker>((_) =>
            {
                try
                {
                    var picker = (ISkylineRectAndPointPicker)unityContainer.Resolve(SelectedRectAndPointPickerVM.RectAndPointPickerType);
                    picker ??= new MinimalAreaWasteRectAndPointPicker();
                    return picker;
                }
                catch (ResolutionFailedException)
                {
                    //Return default implementation
                    return new MinimalAreaWasteRectAndPointPicker();
                }
            });
        }

        /// <summary>
        /// The types implementing <see cref="ISkylineRectAndPointPicker"/> interface that were loaded by the PaunPacker.GUI
        /// </summary>
        public IEnumerable<RectAndPointPickerViewModel> SkylineRectAndPointPickerVMs { get; private set; }

        /// <summary>
        /// The currently selected RectAndPointPicker
        /// </summary>
        public RectAndPointPickerViewModel SelectedRectAndPointPickerVM { get; set; }
    }
}
