using System;
using System.Collections.Generic;

namespace SolutionForGraphNavigator.Interfaces
{
  public interface INodeGraph<T>
  {
    Dictionary<T, INode<T>> NodesRegister { get; }
    IObservable<IEnumerable<string>> Navigate(INode<T> source, INode<T> target);
    void RegisterNode(INode<T> source, INode<T> target);
  }
}
