using System;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.ViewModels
{
    public class LayoutViewModel : Notify
    {
        public LayoutViewModel()
        {
            _layoutVersion = new Version(1, 0, 0, 0);
        }

        private Version _layoutVersion;

        public Version LayoutVersion
        {
            get { return _layoutVersion; }
            set
            {
                if (value is null)
                {
                    return;
                }

                UpdateProperty(ref _layoutVersion, value);
                OnPropertyChanged(nameof(LayoutVersionDisplayValue));
            }
        }

        public string LayoutVersionDisplayValue
        {
            get { return _layoutVersion.ToString(); }
            set
            {
                if (Version.TryParse(value, out var parsedVersion))
                {
                    LayoutVersion = parsedVersion;
                }
            }
        }

        private LayoutFile _layoutFile;

        public LayoutFile LayoutFile
        {
            get { return _layoutFile; }
            set { UpdateProperty(ref _layoutFile, value); }
        }

        private SessionLayout _selectedSession;

        public SessionLayout SelectedSession
        {
            get { return _selectedSession; }
            set { UpdateProperty(ref _selectedSession, value); }
        }

        private IslandLayout _selectedIsland;

        public IslandLayout SelectedIsland
        {
            get { return _selectedIsland; }
            set { UpdateProperty(ref _selectedIsland, value); }
        }
    }
}
