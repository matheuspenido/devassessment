using SolutionForGraphNavigator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SolutionForGraphNavigator.Implementations
{
  public class NodeGraph<T> : INodeGraph<T>
  {
    public Dictionary<T, INode<T>> NodesRegister { get; set; }

    public IObservable<IEnumerable<string>> OrderedNodesTraveled;

    public NodeGraph()
    {
      NodesRegister = new Dictionary<T, INode<T>>();
    }

    public void RegisterNode(INode<T> source, INode<T> target)
    {
      var sourceExist = NodesRegister.TryGetValue(source.Id, out var sourceRegistered);
      var targetExist = NodesRegister.TryGetValue(target.Id, out var targetRegistered);

      if (sourceExist)
        source = sourceRegistered;
      else
        NodesRegister.Add(source.Id, source);
      
      if (targetExist)
        target = targetRegistered;
      else
        NodesRegister.Add(target.Id, target);

      source.PointTo(target);
    }

    public IObservable<IEnumerable<string>> Navigate(INode<T> source, INode<T> target)
    {
      if (!NodesRegister.TryGetValue(source.Id, out INode<T> _))
        throw new InvalidOperationException("Invalid Source Node.");

      if (!NodesRegister.TryGetValue(target.Id, out INode<T> _))
        throw new InvalidOperationException("Invalid Target Node.");

      var paths = new List<string>();
      var visitedNodes = new HashSet<T>();

      Navigate(source, target, paths, visitedNodes, "");
      //Navigate(source, target, new List<T> { source.Id }, visitedNodes, paths);

      return Observable.Create<IEnumerable<string>>(o =>
      {
        o.OnNext(paths);
        o.OnCompleted();
        return Disposable.Empty;
      });

      //return Observable.Create<IEnumerable<IEnumerable<T>>>(o =>
      //{
      //  o.OnNext(paths);
      //  o.OnCompleted();
      //  return Disposable.Empty;
      //});
    }

    private void Navigate(INode<T> source, INode<T> target, List<string> paths, HashSet<T> visitedNodes, string path)
    {

      if (source.Id.Equals(target.Id))
      {
        paths.Add(path);
        return;
      }

      if (visitedNodes.Contains(source.Id))
        return;
      else
        visitedNodes.Add(source.Id);

      if (source.Links == null)
        return;

      foreach (var link in source?.Links)
      {
        var next = source.GetNodeLink(link.Key);
        var currentPath = WritePath(path, source.Id, next.Id);

        Navigate(next, target, paths, visitedNodes, currentPath);
      }

      return;
    }

    private string WritePath(string currentPath, T currentNodeId, T nextNodeId)
    {
      if (string.IsNullOrEmpty(currentPath))
        return $"{currentNodeId}-{nextNodeId}";

      else return $"{currentPath}-{nextNodeId}";
    }

    //private void Navigate(INode<T> source, INode<T> target, List<T> currentPath, HashSet<T> visitedNodes, List<IEnumerable<T>> paths)
    //{
    //  if (source.Id.Equals(target.Id))
    //  {
    //    if (currentPath.Count > 1)
    //    {
    //      var pathCopy = new List<T>(currentPath);
    //      paths.Add(pathCopy);
    //    }

    //    return;
    //  }

    //  if (visitedNodes.Contains(source.Id))
    //    return;
    //  else
    //    visitedNodes.Add(source.Id);

    //  if (source.Links == null)
    //    return;

    //  foreach (var link in source?.Links)
    //  {
    //    var next = source.GetNodeLink(link.Key);

    //    currentPath.Add(next.Id);
    //    Navigate(next, target, currentPath, visitedNodes, paths);
    //    currentPath.RemoveAt(currentPath.Count - 1);
    //  }

    //  return;
    //}
  }
}
