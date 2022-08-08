using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VigilantMeerkat.Micro.Base.Db
{
    public class EFContextStore
    {
        private Dictionary<string, DbContext> store = new Dictionary<string, DbContext>();

        public void Add<T>(T value) where T : DbContext
        {
            store.Add(nameof(T), value);
        }

        public T Get<T>() where T : DbContext
        {
            return store.FirstOrDefault(x => x.Value.GetType() == typeof(T)).Value as T;
        }
    }
}
