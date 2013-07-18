using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffusionCurves.Model
{
    /// <summary>
    /// Interface for PathContainer.
    /// </summary>
    public interface IPathContainer
    {
        IEnumerator<Path> GetPathsEnumerator();

        void AddPath(Path path);

        bool RemovePath(Path path);

        Path GetLastPath();

        List<Path> Paths { get; }

        int Count { get; }

        int TotalPointCount{ get; }

        IPathContainer Clone();

        bool Equals(Object obj);

        void Clear();

        int ActivePathsLayer { get; set; }

        List<int>[] LayerIndices { get; set; }
    }
}
