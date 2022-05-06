using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Data.Helpers;

public static class EntityGraphUpdateHelper
{
    public static async ValueTask<object> UpdateGraphAsync(this DbContext context, object entity) =>
        await context.UpdateGraphAsync(await context.FindEntityAsync(entity), entity, new HashSet<IForeignKey>());

    private static async ValueTask<object> UpdateGraphAsync(this DbContext context, object dbEntity, object entity, HashSet<IForeignKey> visited)
    {
        bool isNew = dbEntity == null;
        if (isNew)
        {
            dbEntity = entity;
        }

        var dbEntry = context.Entry(dbEntity);

        if (isNew)
        {
            dbEntry.State = EntityState.Added;
        }
        else
        {
            // ensure is attached (tracked)
            if (dbEntry.State == EntityState.Detached)
            {
                dbEntry.State = EntityState.Unchanged;
            }

            // update primitive values
            dbEntry.CurrentValues.SetValues(entity);
        }

        // process navigations
        foreach (var navEntry in dbEntry.Navigations)
        {
            if (navEntry.Metadata is not INavigation navigation)
            {
                continue; // skip navigation pproperty         
            }

            if (!visited.Add(navigation.ForeignKey))
            {
                continue; // already processed
            }

            await navEntry.LoadAsync();

            if (!navigation.IsCollection)
            {
                // reference type navigation property
                var refValue = navigation.GetGetter().GetClrValue(entity);
                navEntry.CurrentValue = refValue == null ? null :
                    await context.UpdateGraphAsync(navEntry.CurrentValue, refValue, visited);
            }
            else
            {
                // collection type navigation property
                var accessor = navigation.GetCollectionAccessor();
                var items = (IEnumerable<object>)accessor.GetOrCreate(entity, false);
                var dbItems = (IEnumerable<object>)accessor.GetOrCreate(dbEntity, false);

                var itemType = navigation.TargetEntityType;

                var keyProperties = itemType.FindPrimaryKey().Properties
                    .Select((p, i) => (Index: i, Getter: p.GetGetter(), Comparer: p.GetKeyValueComparer()))
                    .ToList();

                var keyValues = new object[keyProperties.Count];

                void GetKeyValues(object sourceItem)
                {
                    foreach (var p in keyProperties)
                    {
                        keyValues[p.Index] = p.Getter.GetClrValue(sourceItem);
                    }
                }

                object FindItem(IEnumerable<object> targetCollection, object sourceItem)
                {
                    GetKeyValues(sourceItem);
                    foreach (var targetItem in targetCollection)
                    {
                        bool keyMatch = true;
                        foreach (var p in keyProperties)
                        {
                            (var keyA, var keyB) = (p.Getter.GetClrValue(targetItem), keyValues[p.Index]);
                            keyMatch = p.Comparer?.Equals(keyA, keyB) ?? object.Equals(keyA, keyB);
                            if (!keyMatch)
                            {
                                break;
                            }
                        }
                        if (keyMatch)
                        {
                            return targetItem;
                        }
                    }
                    return null;
                }

                // Remove db items missing in the current list
                foreach (var dbItem in dbItems.ToList())
                {
                    if (FindItem(items, dbItem) == null)
                    {
                        accessor.Remove(dbEntity, dbItem);
                    }
                }

                // Add current items missing in the db list, update others
                var existingItems = dbItems.ToList();

                foreach (var item in items)
                {
                    var dbItem = FindItem(existingItems, item);
                    if (dbItem == null)
                    {
                        accessor.Add(dbEntity, item, false);
                    }

                    await context.UpdateGraphAsync(dbItem, item, visited);
                }
            }
        }

        return dbEntity;
    }

    public static ValueTask<object> FindEntityAsync(this DbContext context, object entity)
    {
        var entityType = context.Model.FindRuntimeEntityType(entity.GetType());
        var keyProperties = entityType.FindPrimaryKey().Properties;

        var keyValues = new object[keyProperties.Count];

        for (int i = 0; i < keyValues.Length; i++)
        {
            keyValues[i] = keyProperties[i].GetGetter().GetClrValue(entity);
        }

        return context.FindAsync(entityType.ClrType, keyValues);
    }

    private static void UpdateNavigation(this DbContext context, object existingItem, object updatedItem)
    {
        if (existingItem != null)
        {
            context.Entry(existingItem).State = EntityState.Deleted;
        }
        if (updatedItem != null)
        {
            context.Entry(updatedItem).State = EntityState.Added;
        }

        context.SaveChanges();
    }

    private static void UpdateNavigations(this DbContext context, IEnumerable<object> existingItems, IEnumerable<object> updatedItems)
    {
        var addedItems = updatedItems.Except(existingItems, Equality<dynamic>.CreateComparer(x => x.Id));

        var deletedItems = existingItems.Except(updatedItems, Equality<dynamic>.CreateComparer(x => x.Id));

        var modifiedItems = updatedItems.Except(addedItems, Equality<dynamic>.CreateComparer(x => x.Id));

        addedItems.ToList<object>().ForEach(x => context.Entry(x).State = EntityState.Added);

        deletedItems.ToList<object>().ForEach(x => context.Entry(x).State = EntityState.Deleted);

        foreach (var item in modifiedItems)
        {
            var existingItem = context.Set<object>().Find(item.Id);

            if (existingItem != null)
            {
                var contextItem = context.Entry(existingItem);
                contextItem.CurrentValues.SetValues(item);
            }
        }

        context.SaveChanges();
    }
}
