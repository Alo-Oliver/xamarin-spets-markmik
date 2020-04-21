using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paevik
{
    public class Andmebaas
    {
        //Tagastab DB ühenduse andmed
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.andmebaasiAsukoht, Constants.Flags);
        });

        //Määrab DB ühenduse
        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public Andmebaas()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        //Kui Model tabeleid ei ole, siis need luuakse.
        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Models.PohiVaadeModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Models.PohiVaadeModel)).ConfigureAwait(false);
                    initialized = true;
                }
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Models.KaalVaadeModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Models.KaalVaadeModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        //Need Taskid on tõenäoliselt võimalik muutujaga luua, et neid ei peaks kordama(variable = model class name). Ma ei saanud seda tööle ja tegin iga modeli jaoks eraldi taskid.
        //võimaldab PohiVaadeModeliga itemeid andmebaasi salvestada
        public Task<int> SaveItemAsync(Models.PohiVaadeModel item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        //võimaldab KaalVaadeModeliga itemeid andmebaasi salvestada
        public Task<int> SaveKaalItemAsync(Models.KaalVaadeModel item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        //annab kindla ID alusel PohiVaadeModel itemi
        public Task<Models.PohiVaadeModel> GetItemAsync(int id)
        {
            return Database.Table<Models.PohiVaadeModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //annab kindla ID alusel KaalVaadeModel itemi
        public Task<Models.KaalVaadeModel> GetKaalItemAsync(int id)
        {
            return Database.Table<Models.KaalVaadeModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        //annab kõik PohiVaadeModel itemid
        public Task<List<Models.PohiVaadeModel>> GetItemsAsync()
        {
            return Database.Table<Models.PohiVaadeModel>().ToListAsync();
        }

        //annab kõik KaalVaadeModel itemid
        public Task<List<Models.KaalVaadeModel>> GetKaalItemsAsync()
        {
            return Database.Table<Models.KaalVaadeModel>().ToListAsync();
        }

        //võimaldab kustutada valitud PohiVaadeModel itemi
        public Task<int> DeleteItemAsync(Models.PohiVaadeModel item)
        {
            return Database.DeleteAsync(item);
        }

        //võimaldab kustutada valitud PohiVaadeModel itemi
        public Task<int> DeleteKaalItemAsync(Models.KaalVaadeModel item)
        {
            return Database.DeleteAsync(item);
        }
    }


    public static class TaskExtensions //IDK what the fuck is going on here, but it's important
    {
        // NOTE: Async void is intentional here. This provides a way
        // to call an async method from the constructor while
        // communicating intent to fire and forget, and allow
        // handling of exceptions
        public static async void SafeFireAndForget(this Task task,
            bool returnToCallingContext,
            Action<Exception> onException = null)
        {
            try
            {
                await task.ConfigureAwait(returnToCallingContext);
            }

            // if the provided action is not null, catch and
            // pass the thrown exception
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }


    }
}
