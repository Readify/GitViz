using System;
using System.Windows;
using System.Windows.Controls;
using GitViz.Logic;

namespace UI
{
    public class VertexTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommitTemplate { get; set; }
        public DataTemplate ReferenceTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var vertex = item as Vertex;
            if (vertex == null) return base.SelectTemplate(item, container);

            if (vertex.Commit != null) return CommitTemplate;
            if (vertex.Reference != null) return ReferenceTemplate;

            throw new NotSupportedException();
        }
    }
}
