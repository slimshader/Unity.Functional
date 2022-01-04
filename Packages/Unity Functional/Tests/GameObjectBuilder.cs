using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bravasoft.Tests
{
    internal sealed class GameObjectBuilder
    {
        private readonly Option<GameObjectBuilder> _parent;
        private string _name;
        private readonly List<Type> _components = new List<Type>();

        public Option<GameObjectBuilder> Parent => _parent;

        public GameObjectBuilder(GameObjectBuilder parent = null)
        {
            _parent = Option.Of(parent);
        }

        public GameObjectBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public GameObjectBuilder WithComponent<T>()
        {
            _components.Add(typeof(T));
            return this;
        }

        public GameObject Build()
        {
            return new GameObject(_name ?? string.Empty, _components.ToArray());
        }
    }
}
