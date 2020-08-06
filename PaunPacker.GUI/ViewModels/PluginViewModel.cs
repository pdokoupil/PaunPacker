using System;
using System.Collections.Generic;
using System.Linq;
using PaunPacker.GUI.Dialogs;
using PaunPacker.GUI.WPF.Common.Attributes;
using Prism.Services.Dialogs;

namespace PaunPacker.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for a View that shows information about a plugin
    /// </summary>
    class PluginViewModel : DialogViewModelBase
    {
        /// <summary>
        /// Constructs the view model from the <see cref="Type"/> of the plugin
        /// </summary>
        /// <param name="pluginType">The type of the plugin</param>
        public PluginViewModel(Type pluginType)
        {
            if (pluginType.GetCustomAttributes(typeof(PluginMetadataAttribute), false).FirstOrDefault() is PluginMetadataAttribute pluginMetadata)
            {
                PluginType = pluginType;
                Name = pluginMetadata.Name;
                Description = pluginMetadata.Description;
                Author = pluginMetadata.Author;
                Version = pluginMetadata.Version;
                ExportedTypes = pluginMetadata.ExportedTypes;
            }

            ExportedTypesMetadata = (ExportedTypes?.Select(x => 
                GetDecoratingAttributes<ExportedTypeMetadataAttribute>(x).FirstOrDefault())
                ?? Enumerable.Empty<ExportedTypeMetadataAttribute>())
                .Where(x => x != null)
                .Concat(GetDecoratingAttributes<ExportedTypeMetadataAttribute>(PluginType))
                .Distinct();
        }

        /// <summary>
        /// Returns all the attributes of type <typeparamref name="T"/> that are decorating the type <paramref name="type"/>
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="type">Type instance</param>
        /// <returns>All the attributes of type <typeparamref name="T"/> that are decorating the type <paramref name="type"/></returns>
        private IEnumerable<T> GetDecoratingAttributes<T>(Type type) 
            where T : class
        {
            if (type == null)
            {
                return Enumerable.Empty<T>();
            }

            return type.GetCustomAttributes(typeof(T), true)
                .Select(x => x as T)
                .Where(x => x != null);
        }

        /// <summary>
        /// Constructs the view model
        /// </summary>
        public PluginViewModel()
        {
            
        }

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public string Name
        {
            get => name;
            private set => SetProperty(ref name, value);
        }

        /// <summary>
        /// The description of the plugin
        /// </summary>
        public string Description
        {
            get => description;
            private set => SetProperty(ref description, value);
        }

        /// <summary>
        /// The author of the plugin
        /// </summary>
        public string Author
        {
            get => author;
            private set => SetProperty(ref author, value);
        }

        /// <summary>
        /// The version of the plugin
        /// </summary>
        public string Version
        {
            get => version;
            private set => SetProperty(ref version, value);
        }

        /// <summary>
        /// The type of the plugin
        /// </summary>
        public Type PluginType
        {
            get => pluginType;
            private set => SetProperty(ref pluginType, value);
        }

        /// <summary>
        /// The types that are exported from the plugin
        /// </summary>
        public IEnumerable<Type> ExportedTypes
        {
            get => exportedTypes;
            private set => SetProperty(ref exportedTypes, value);
        }

        /// <summary>
        /// Metadata about the types that the plugin exports
        /// </summary>
        public IEnumerable<ExportedTypeMetadataAttribute> ExportedTypesMetadata
        {
            get => exportedTypesMetadata;
            private set => SetProperty(ref exportedTypesMetadata, value);
        }

        /// <summary>
        /// Handles the openning of the dialog
        /// </summary>
        /// <param name="parameters"></param>
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var title = parameters.GetValue<string>("title");
            if (title != null)
            {
                Title = title;
            }
            var pluginViewModel = parameters.GetValue<PluginViewModel>("pluginViewModel");
            if (pluginViewModel != null)
            {
                PluginType = pluginViewModel.PluginType;
                Name = pluginViewModel.Name;
                Description = pluginViewModel.Description;
                Author = pluginViewModel.Author;
                Version = pluginViewModel.Version;
                ExportedTypes = pluginViewModel.ExportedTypes;

                ExportedTypesMetadata = (ExportedTypes?.Select(x =>
                    GetDecoratingAttributes<ExportedTypeMetadataAttribute>(x).FirstOrDefault())
                    ?? Enumerable.Empty<ExportedTypeMetadataAttribute>())
                    .Where(x => x != null)
                    .Concat(GetDecoratingAttributes<ExportedTypeMetadataAttribute>(PluginType))
                    .Distinct();
            }
        }

        /// <see cref="Name"/>
        private string name;

        /// <see cref="Description"/>
        private string description;

        /// <see cref="Author"/>
        private string author;

        /// <see cref="Version"/>
        private string version;

        /// <see cref="PluginType"/>
        private Type pluginType;

        /// <see cref="ExportedTypes"/>
        private IEnumerable<Type> exportedTypes;

        /// <see cref="ExportedTypesMetadata"/>
        private IEnumerable<ExportedTypeMetadataAttribute> exportedTypesMetadata;
    }
}
