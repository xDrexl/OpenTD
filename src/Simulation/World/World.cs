using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTD.Simulation.World;

public sealed class World
{
    private readonly HashSet<Entity> _entities = [];
    private readonly Dictionary<Type, IComponentStore> _componentStores = [];
    private int _nextEntityId;

    public Entity CreateEntity()
    {
        var entity = new Entity(_nextEntityId++);
        _entities.Add(entity);
        return entity;
    }

    public bool DestroyEntity(Entity entity)
    {
        if (!_entities.Remove(entity))
        {
            return false;
        }

        foreach (var store in _componentStores.Values)
        {
            store.Remove(entity);
        }

        return true;
    }

    public bool IsAlive(Entity entity) => _entities.Contains(entity);

    public void SetComponent<T>(Entity entity, T component)
        where T : notnull
    {
        EnsureAlive(entity);
        GetOrCreateStore<T>().Set(entity, component);
    }

    public bool RemoveComponent<T>(Entity entity)
        where T : notnull
    {
        return TryGetStore<T>(out var store) && store.Remove(entity);
    }

    public bool TryGetComponent<T>(Entity entity, out T component)
        where T : notnull
    {
        if (TryGetStore<T>(out var store) && store.TryGet(entity, out component))
        {
            return true;
        }

        component = default!;
        return false;
    }

    public T GetComponent<T>(Entity entity)
        where T : notnull
    {
        if (!TryGetComponent<T>(entity, out var component))
        {
            throw new KeyNotFoundException($"Entity {entity.Id} does not have component {typeof(T).Name}.");
        }

        return component;
    }

    public IEnumerable<Entity> Query<T>()
        where T : notnull
    {
        return TryGetStore<T>(out var store) ? store.Entities : [];
    }

    public IEnumerable<Entity> Query<TFirst, TSecond>()
        where TFirst : notnull
        where TSecond : notnull
    {
        if (!TryGetStore<TFirst>(out var first) || !TryGetStore<TSecond>(out var second))
        {
            return [];
        }

        return first.Entities.Where(second.Contains);
    }

    private ComponentStore<T> GetOrCreateStore<T>()
        where T : notnull
    {
        if (TryGetStore<T>(out var store))
        {
            return store;
        }

        store = new ComponentStore<T>();
        _componentStores.Add(typeof(T), store);
        return store;
    }

    private bool TryGetStore<T>(out ComponentStore<T> store)
        where T : notnull
    {
        if (_componentStores.TryGetValue(typeof(T), out var untypedStore))
        {
            store = (ComponentStore<T>)untypedStore;
            return true;
        }

        store = null!;
        return false;
    }

    private void EnsureAlive(Entity entity)
    {
        if (!IsAlive(entity))
        {
            throw new InvalidOperationException($"Entity {entity.Id} is not alive.");
        }
    }

    private interface IComponentStore
    {
        void Remove(Entity entity);
    }

    private sealed class ComponentStore<T> : IComponentStore
        where T : notnull
    {
        private readonly Dictionary<Entity, T> _components = [];

        public IEnumerable<Entity> Entities => _components.Keys;

        public void Set(Entity entity, T component) => _components[entity] = component;

        public bool TryGet(Entity entity, out T component) =>
            _components.TryGetValue(entity, out component!);

        public bool Contains(Entity entity) => _components.ContainsKey(entity);

        public bool Remove(Entity entity) => _components.Remove(entity);

        void IComponentStore.Remove(Entity entity) => Remove(entity);
    }
}
