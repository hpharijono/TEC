using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;

namespace TemporaryEmploymentCorp.DataAccess
{
    public class EfDataService<T> : IDataService<T> where T : class, new()
    {
        public void Add(T record)
        {
            using (var context = new teCorpContext())
            {
                context.Entry(record).State = EntityState.Added;
                context.SaveChanges();
            }
        }

        public Task AddAsync(T record)
        {
            return AddAsync(record, CancellationToken.None);
        }

        public async Task AddAsync(T record, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    context.Set<T>().Add(record);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public void AddRange(List<T> records)
        {
            using (var context = new teCorpContext())
            {
                foreach (var record in records)
                {
                    context.Entry(record).State = EntityState.Added;
                }
                context.SaveChanges();
            }
        }

        public Task AddRangeAsync(List<T> records)
        {
            return AddRangeAsync(records, CancellationToken.None);
        }

        public async Task AddRangeAsync(List<T> records, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    context.Set<T>().AddRange(records);
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public void Remove(T record)
        {
            using (var context = new teCorpContext())
            {
                context.Entry(record).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public Task RemoveAsync(T record)
        {
            return RemoveAsync(record, CancellationToken.None);
        }

        public async Task RemoveAsync(T record, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    context.Entry(record).State = EntityState.Deleted;
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public void RemoveRange(List<T> records)
        {
            using (var context = new teCorpContext())
            {
                foreach (var record in records)
                {
                    context.Entry(record).State = EntityState.Deleted;
                }
                context.SaveChanges();
            }
        }

        public Task RemoveRangeAsync(List<T> records)
        {
            return RemoveRangeAsync(records, CancellationToken.None);
        }

        public async Task RemoveRangeAsync(List<T> records, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    foreach (var record in records)
                    {
                        context.Entry(record).State = EntityState.Deleted;

                    }
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public void Update(T record)
        {
            using (var context = new teCorpContext())
            {
                context.Entry(record).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public Task UpdateAsync(T record)
        {
            return UpdateAsync(record, CancellationToken.None);
        }

        public async Task UpdateAsync(T record, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public void UpdateRange(List<T> records)
        {
            using (var context = new teCorpContext())
            {
                foreach (var record in records)
                {
                    context.Entry(record).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        public Task UpdateRangeAsync(List<T> records)
        {
            return UpdateRangeAsync(records, CancellationToken.None);
        }

        public async Task UpdateRangeAsync(List<T> records, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    foreach (var record in records)
                    {
                        context.Entry(record).State = EntityState.Modified;

                    }
                    await context.SaveChangesAsync(token).ConfigureAwait(false);
                }
            }, token);
        }

        public T Get(Expression<Func<T, bool>> condition)
        {
            using (var context = new teCorpContext())
            {
                var record = context.Set<T>().FirstOrDefault(condition);
                return record;
            }


        }

        public Task<T> GetAsync(Expression<Func<T, bool>> condition)
        {
            return GetAsync(condition, CancellationToken.None);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> condition, CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    return await context.Set<T>().FirstOrDefaultAsync(token).ConfigureAwait(false);
                }
            });
        }

        public List<T> GetRange(Expression<Func<T, bool>> condition)
        {
            using (var context = new teCorpContext())
            {
                var records = context.Set<T>().Where(condition).ToList();
                return records;
            }
        }

        public async Task<List<T>> GetRangeAsync(Expression<Func<T, bool>> condition)
        {
            //            return await Task.Run(async () =>
            //            {
            //                using (var context = new GoodReadingBookstoreContext())
            //                {
            //                    return await context.Set<T>().ToListAsync().ConfigureAwait(false);
            //                }
            //            });



            return await GetRangeAsync(condition,CancellationToken.None);
        }


        public async Task<List<T>> GetRangeAsync(Expression<Func<T, bool>> condition, CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    return await context.Set<T>().Where(condition).ToListAsync(token).ConfigureAwait(false);
                }
            });
        }

        public List<T> GetRange()
        {
            using (var context = new teCorpContext())
            {
                var records = context.Set<T>().ToList();
                return records;
            }
        }

        public async Task<List<T>> GetRangeAsync()
        {
            return await GetRangeAsync(CancellationToken.None);
        }

        public async Task<List<T>> GetRangeAsync(CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                using (var context = new teCorpContext())
                {
                    return await context.Set<T>().ToListAsync(token).ConfigureAwait(false);
                }
            });
        }
    }
}
