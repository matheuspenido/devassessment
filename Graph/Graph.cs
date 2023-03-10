using SolutionForGraphNavigator.Implementations;
using System;
using System.Collections.Generic;

namespace Graph
{
  public interface IGraph<T>
  {
    IObservable<IEnumerable<string>> RoutesBetween(T source, T target);

    //IObservable<IEnumerable<IEnumerable<T>> RoutesBetween(T source, T target);
  }

  public class Graph<T> : IGraph<T>
  {
    private readonly NodeGraph<T> _nodeGraph;

    public Graph(IEnumerable<ILink<T>> links)
    {
      _nodeGraph = new NodeGraph<T>();

      foreach (ILink<T> link in links)
      {
        _nodeGraph.RegisterNode(new Node<T>(link.Source), new Node<T>(link.Target));
      }
    }

    //It needs to be a return of IObservable<IEnumerable<IEnumerable<T>> or IObservable<IEnumerable<string>>
    //because we can have many routes between two points, and each route could be a path of Nodes, or at least
    //a printed string of its nodes.
    //I created the two situations, and I keep one in use and the other in comments.
    public IObservable<IEnumerable<string>> RoutesBetween(T source, T target)
    {
      _nodeGraph.NodesRegister.TryGetValue(source, out var sourceFound);
      _nodeGraph.NodesRegister.TryGetValue(target, out var targetFound);

      return _nodeGraph.Navigate(sourceFound, targetFound);
    }
  }
}
