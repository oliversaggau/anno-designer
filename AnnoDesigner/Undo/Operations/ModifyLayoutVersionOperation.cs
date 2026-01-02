using System;
using AnnoDesigner.ViewModels;

namespace AnnoDesigner.Undo.Operations
{
    public class ModifyLayoutVersionOperation : BaseOperation
    {
        public LayoutViewModel LayoutViewModel { get; set; }

        public Version OldValue { get; set; }

        public Version NewValue { get; set; }

        protected override void UndoOperation()
        {
            LayoutViewModel.LayoutVersion = OldValue;
        }

        protected override void RedoOperation()
        {
            LayoutViewModel.LayoutVersion = NewValue;
        }
    }
}
